using Hotel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Globalization;
using System.Threading.Tasks;

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
        public ActionResult Booking(int roomID,string fromDate,string toDate)
        {
            var user = Session["user"] as User;
            if (user == null)
                return new HttpStatusCodeResult(401, "Chưa đăng nhập.");
            var room = db.Rooms.FirstOrDefault(r => r.RoomID == roomID);
            int percent = db.DiscountDetails.Where(dd => dd.RoomID == roomID)
                                            .Select(dd => (int?)dd.Discount.DiscountPercent)
                                            .FirstOrDefault() ?? 0;
             
            var booking = new Booking
            {
                RoomID = roomID,
                UserID = user.UserID,
                BookingDate = DateTime.Now,
                ExpirationTime = DateTime.Now.AddMinutes(20),
                Amount = (room.Price - (room.Price * ((decimal)percent / 100m))) 
                        * 
                        (DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture) 
                        - DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)).Days,
                Discount = percent,
                CheckIn = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                CheckOut = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                PaymentStatus = "Pending",
            };
            db.Bookings.InsertOnSubmit(booking);
            db.SubmitChanges();
            return View(booking);
        }

        [HttpPost]
        public async Task<ActionResult> ConfirmBooking(FormCollection form)
        {
            var user = (User)Session["user"];
            if (user == null) return new HttpStatusCodeResult(401, "Chưa đăng nhập.");

            var ListRoomId = form.GetValues("roomIds")?.Select(int.Parse).ToList();
            if (ListRoomId == null || !ListRoomId.Any())
            {
                return new HttpStatusCodeResult(400, "Không có phòng nào được chọn.");
            }

            string dateRange = form["dateRange"];
            if (string.IsNullOrEmpty(dateRange))
            {
                return new HttpStatusCodeResult(402, "Không có ngày đặt phòng.");
            }

            var dates = dateRange.Split(new[] { " - ", " to " }, StringSplitOptions.RemoveEmptyEntries);
            if (dates.Length != 2)
            {
                return new HttpStatusCodeResult(403, "Định dạng ngày không hợp lệ.");
            }

            DateTime checkIn, checkOut;
            string[] formats = { "dd-MM-yyyy" };

            bool isCheckInValid = DateTime.TryParseExact(dates[0].Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out checkIn);
            bool isCheckOutValid = DateTime.TryParseExact(dates[1].Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out checkOut);

            if (!isCheckInValid || !isCheckOutValid)
            {
                return new HttpStatusCodeResult(404, "Lỗi chuyển đổi ngày.");
            }

            if (checkIn >= checkOut)
            {
                return new HttpStatusCodeResult(405, "Check-in phải trước Check-out.");
            }

            // Tạo Booking mới
            Booking newBooking = new Booking
            {
                UserID = user.UserID,
                CheckIn = checkIn,
                CheckOut = checkOut,
                BookingDate = DateTime.Now
            };

            db.Bookings.InsertOnSubmit(newBooking);
            db.SubmitChanges(); // Lưu lại để lấy BookingID

            decimal tongTien = 0;
            string danhSachPhong = "";
            int span = ListRoomId.Count;
            foreach (var roomId in ListRoomId)
            {

                var room = db.Rooms.FirstOrDefault(r => r.RoomID == roomId);
                int percent = db.DiscountDetails.Where(dd => dd.RoomID == roomId)
                                                .Select(dd => (int?)dd.Discount.DiscountPercent)
                                                .FirstOrDefault() ?? 0;
                if (room == null) continue; // Nếu phòng không tồn tại, bỏ qua

                // Thêm vào BookingDetail
                var bookingDetail = new Booking
                {
                    RoomID = roomId,
                    BookingID = newBooking.BookingID,
                    Discount = percent
                };
                db.Bookings.InsertOnSubmit(bookingDetail);

                // Tính tiền
                tongTien += ( room.Price - ( room.Price * ((decimal)percent /100m) )) * (newBooking.CheckOut - newBooking.CheckIn).Days;
                
                if (span == ListRoomId.Count)
                {
                    danhSachPhong += $@"
                <tr>
                    <td>{room.RoomName}</td>
                    <td>{room.RoomType.RoomTypeName}</td>
                    <td>{(room.Price - (room.Price * (percent / 100m))):N0} VND</td>
                    <td rowspan=""{span}""> {newBooking.CheckIn:dd/MM/yyyy}</td>
                    <td rowspan=""{span}""> {newBooking.CheckOut:dd/MM/yyyy}</td>
                    <td rowspan=""{span}""> {(newBooking.CheckOut - newBooking.CheckIn).Days} đêm</td>
                </tr>";
                    span = 0;
                }
                else
                {
                    danhSachPhong += $@"
                <tr>
                    <td>{room.RoomName}</td>
                    <td>{room.RoomType.RoomTypeName}</td>
                    <td>{room.Price:N0} VND</td>
                </tr>";
                }
            }

            db.SubmitChanges(); // Lưu BookingDetails

            // Tạo QR Code
            string qrUrl = new GenQRCode().QRCode(tongTien, newBooking.BookingID);

            // Load template email
            string templatePath = Server.MapPath("~/Content/templates/send3.html");
            if (!System.IO.File.Exists(templatePath))
            {
                return new HttpStatusCodeResult(500, "Không tìm thấy template email.");
            }

            string contentCustomer = System.IO.File.ReadAllText(templatePath);

            contentCustomer = contentCustomer.Replace("{{MaDon}}", newBooking.BookingID.ToString())
                                             .Replace("{{NgayDatHang}}", newBooking.BookingDate.ToString("dd/MM/yyyy"))
                                             .Replace("{{DanhSachPhong}}", danhSachPhong)
                                             .Replace("{{TenKhachHang}}", newBooking.User.FullName)
                                             .Replace("{{Phone}}", newBooking.User.Phone)
                                             .Replace("{{Email}}", newBooking.User.Email)
                                             .Replace("{{TongTien}}", tongTien.ToString("N0"))
                                             .Replace("{{QRCode}}", qrUrl);

            // Gửi email bất đồng bộ
            _ = Task.Run(() =>
            {
                try
                {
                    Common.Common.sendEmail("Wengg Hotel", $"Booking Confirmation #{newBooking.BookingID}", contentCustomer, newBooking.User.Email);
                }
                catch (Exception ex)
                {
                    System.IO.File.AppendAllText(Server.MapPath("~/Logs/email_error.log"), ex.ToString());
                }
            });

            // Tạo thanh toán
            var payment = new Booking
            {
                BookingID = newBooking.BookingID,
                BookingDate = DateTime.Now,
                Amount = tongTien,
                PaymentStatus = "Pending",
            };

            db.Bookings.InsertOnSubmit(payment);
            db.SubmitChanges();

            return new HttpStatusCodeResult(202, "Đặt phòng thành công.");

        }
        public ActionResult History()
        {
            var user = (User)Session["user"];
            if (user == null)
                return RedirectToAction("Login", "User");

            var paymentDisplay = (from b in db.Bookings
                                  where b.UserID == user.UserID
                                  join r in db.Rooms on b.RoomID equals r.RoomID
                                  select new PaymentDisplay
                                  {
                                      PaymentId = b.BookingID, // Dùng BookingID làm mã thanh toán
                                      Amount = b.Amount,
                                      Status = b.PaymentStatus,
                                      CheckIn = b.CheckIn,
                                      CheckOut = b.CheckOut,
                                      SoDem = (b.CheckOut - b.CheckIn).Days,
                                      LastUpdate = b.BookingDate,
                                      RoomDetails = new List<RoomDetail> {
                                  new RoomDetail {
                                      RoomID = r.RoomID,
                                      RoomName = r.RoomName,
                                      Price = r.Price,
                                      Percent = b.Discount,
                                      RoomImages = db.RoomImages
                                          .Where(img => img.RoomID == r.RoomID).ToList()
                                  }
                              },
                                      QRCode = new GenQRCode().QRCode(b.Amount, b.BookingID)
                                  }).ToList();

            return View("History", paymentDisplay);
        }


    }
}