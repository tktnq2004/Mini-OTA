using Hotel.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Hotel.Controllers
{
    public class BookingController : Controller
    {
        dbHotelDataContext db = new dbHotelDataContext();
        // GET: Booking
        
        public ActionResult DisplayWishlist()
        {
            var user = (User)Session["user"];
            var cartDetail = (from a in db.Wishlists
                              join b in db.Rooms on a.RoomID equals b.RoomID
                              where a.UserID == user.UserID
                              select new WishlistDisplay
                              {
                                  RoomID = b.RoomID,
                                  RoomName = b.RoomName,
                                  Capacity = b.Capacity,
                                  Description = b.Description,
                                  Price = b.Price,
                                  Percent = db.DiscountDetails.Where(dd => dd.RoomID == a.RoomID)
                                 .Select(dd => (int?)dd.Discount.DiscountPercent)
                                 .FirstOrDefault() ?? 0,
                                  RoomImages = db.RoomImages.Where(img => img.RoomID == b.RoomID).ToList()
                              }).ToList();
            return View("DisplayCart", cartDetail);
        }
        public ActionResult Booking(int roomID,string fromDate,string toDate,int rooms,int adults,int children)
        {
            var room = db.Rooms.FirstOrDefault(r => r.RoomID == roomID);
            int percent = db.DiscountDetails.Where(dd => dd.RoomID == roomID)
                                            .Select(dd => (int?)dd.Discount.DiscountPercent)
                                            .FirstOrDefault() ?? 0;
             
            var booking = new Booking
            {
                RoomID = roomID,
                BookingDate = DateTime.Now,
                ExpirationTime = DateTime.Now.AddMinutes(20),
                Amount = ((room.Price - (room.Price * ((decimal)percent / 100m))) 
                        * 
                        (DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture) 
                        - DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)).Days) * rooms,
                Discount = percent,
                CheckIn = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                CheckOut = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                PaymentStatus = "Pending",
                Quantity = rooms,
                Adults = adults,
                Children = children,
            };
            db.Bookings.InsertOnSubmit(booking);
            db.SubmitChanges();
            return View(booking);
        }
        private string GenerateRandomPassword(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public ActionResult AddUserToBooking(int bookingID, string FirstName, string LastName, string Email, string Phone)
        {
            string rawPassword = GenerateRandomPassword(10);
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);
            var user = db.Users.FirstOrDefault(u => u.Email == Email);
            if (user == null)
            {
                var newUser = new User
                {
                    FullName = $"{FirstName} {LastName}",
                    Username = Email,
                    Email = Email,
                    Phone = Phone,
                    Password = hashedPassword,
                    Role = "Customer",
                };

                db.Users.InsertOnSubmit(newUser);
                db.SubmitChanges();

                var booking = db.Bookings.FirstOrDefault(b => b.BookingID == bookingID);
                if (booking != null)
                {
                    booking.UserID = user.UserID;
                    db.SubmitChanges();
                }
            }
            return Json(new { success = true });
        }

        public ActionResult LoadPaymentPartial(string method,int bookingID,decimal amount)
        {
            switch (method)
            {
                case "CreditCard":
                    return PartialView("CreditCard");
                case "Banking":
                    ViewBag.BookingID = bookingID;
                    ViewBag.Amount = amount;
                    var QRcode = new GenQRCode().QRCode(amount, bookingID);
                    return PartialView("Banking",QRcode);
                default:
                    ViewBag.BookingID = bookingID;
                    ViewBag.Amount = amount;
                    return PartialView("VNPay");
            }
        }

        public JsonResult CreateVnPayUrl(int bookingID, decimal amount)
        {
            try
            {
                var booking = db.Bookings.FirstOrDefault(b => b.BookingID == bookingID);
                if (booking == null)
                    return Json(new { success = false, message = "Không tìm thấy booking." });

                var vnPay = new VNPAY_CS_ASPX.VnPayLibrary();

                string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"];
                string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"];
                string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];

                string orderId = booking.BookingID.ToString();
                string amountStr = ((long)(amount * 100)).ToString(); // VNPay yêu cầu nhân 100
                string bankCode = ""; // Tùy chọn

                vnPay.AddRequestData("vnp_Version", "2.1.0");
                vnPay.AddRequestData("vnp_Command", "pay");
                vnPay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                vnPay.AddRequestData("vnp_Amount", amountStr);
                vnPay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                vnPay.AddRequestData("vnp_CurrCode", "VND");
                vnPay.AddRequestData("vnp_IpAddr", Request.UserHostAddress);
                vnPay.AddRequestData("vnp_Locale", "vn");
                vnPay.AddRequestData("vnp_OrderInfo", $"Thanh toán đơn #{orderId}");
                vnPay.AddRequestData("vnp_OrderType", "other");
                vnPay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                vnPay.AddRequestData("vnp_TxnRef", orderId);

                if (!string.IsNullOrEmpty(bankCode))
                {
                    vnPay.AddRequestData("vnp_BankCode", bankCode);
                }

                string paymentUrl = vnPay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
                return Json(new { success = true, vnpUrl = paymentUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public ActionResult VnPayReturn()
        {
            try
            {
                var responseData = Request.QueryString;
                var vnpay = new VNPAY_CS_ASPX.VnPayLibrary();

                string responseLog = string.Join("\n", responseData.AllKeys.Select(k => $"{k}: {responseData[k]}"));
                System.IO.File.AppendAllText(Server.MapPath("~/Logs/vnpay_response.log"),
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] VNPay Response:\n{responseLog}\n");

                foreach (string key in responseData.AllKeys)
                {
                    vnpay.AddResponseData(key, responseData[key]);
                }

                string hashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
                string vnp_SecureHash = responseData["vnp_SecureHash"];
                bool isValidSignature = vnpay.ValidateSignature(vnp_SecureHash, hashSecret);


                string responseCode = responseData["vnp_ResponseCode"];
                string bookingId = responseData["vnp_TxnRef"];

                if (isValidSignature)
                {
                    System.IO.File.AppendAllText(Server.MapPath("~/Logs/vnpay_response.log"),
                        $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Signature Valid, ResponseCode: {responseCode}, BookingID: {bookingId}\n");

                    if (responseCode == "00") // Thanh toán thành công
                    {
                        var booking = db.Bookings.FirstOrDefault(b => b.BookingID == int.Parse(bookingId));
                        if (booking != null)
                        {
                            booking.PaymentStatus = "Paid";
                            db.SubmitChanges();

                            var user = booking.User;
                            string templatePath = Server.MapPath("~/Content/templates/send3.html");
                            if (System.IO.File.Exists(templatePath))
                            {
                                var room = db.Rooms.FirstOrDefault(r => r.RoomID == booking.RoomID);
                                if (room != null)
                                {
                                    var percent = booking.Discount;
                                    var priceAfterDiscount = room.Price - (room.Price * percent / 100m);

                                    string danhSachPhong = $@"
                            <tr>
                                <td>{room.RoomName}</td>
                                <td>{room.RoomType.RoomTypeName}</td>
                                <td>{priceAfterDiscount:N0} VND</td>
                                <td>{booking.CheckIn:dd/MM/yyyy}</td>
                                <td>{booking.CheckOut:dd/MM/yyyy}</td>
                                <td>{(booking.CheckOut - booking.CheckIn).Days} đêm</td>
                            </tr>";

                                    string qrUrl = new GenQRCode().QRCode(booking.Amount, booking.BookingID);

                                    string content = System.IO.File.ReadAllText(templatePath)
                                        .Replace("{{MaDon}}", booking.BookingID.ToString())
                                        .Replace("{{NgayDatHang}}", booking.BookingDate.ToString("dd/MM/yyyy"))
                                        .Replace("{{DanhSachPhong}}", danhSachPhong)
                                        .Replace("{{TenKhachHang}}", user.FullName)
                                        .Replace("{{Phone}}", user.Phone)
                                        .Replace("{{Email}}", user.Email)
                                        .Replace("{{TongTien}}", booking.Amount.ToString("N0"))
                                        .Replace("{{QRCode}}", qrUrl);

                                    _ = Task.Run(() =>
                                    {
                                        try
                                        {
                                            Common.Common.sendEmail("Wengg Hotel", $"[Thanh toán thành công] Đơn #{booking.BookingID}", content, user.Email);
                                        }
                                        catch (Exception ex)
                                        {
                                            System.IO.File.AppendAllText(Server.MapPath("~/Logs/email_error.log"),
                                                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Email Error: {ex.Message}\n{ex.StackTrace}\n");
                                        }
                                    });
                                }
                            }
                            return View("Success");
                        }
                        return Content("Không tìm thấy booking.");
                    }
                    else
                    {
                        System.IO.File.AppendAllText(Server.MapPath("~/Logs/vnpay_response.log"),
                            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Payment Failed, ResponseCode: {responseCode}, BookingID: {bookingId}\n");
                        return View("Fail");
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(Server.MapPath("~/Logs/vnpay_response.log"),
                        $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Invalid Signature\n");
                    return Content($"Sai chữ ký VNPay! ResponseCode: {responseData["vnp_ResponseCode"]}, TxnRef: {responseData["vnp_TxnRef"]}");
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Server.MapPath("~/Logs/vnpay_error.log"),
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error in VnPayReturn: {ex.Message}\n{ex.StackTrace}\n");
                return Content($"Lỗi xử lý phản hồi VNPay: {ex.Message}");
            }
        }






    }
}