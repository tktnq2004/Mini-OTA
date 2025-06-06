using Hotel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QRCoder;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;

namespace Hotel.Controllers
{
    public class BookingController : Controller
    {
        dbHotelBrandDataContext db = new dbHotelBrandDataContext();
        // GET: Booking
        [HttpPost]
        public ActionResult AddCart(int id)
        {
            var user = (User)Session["user"];
            if (user == null)
            {
                return new HttpStatusCodeResult(401);
            }

            var existingCartItem = db.Carts.FirstOrDefault(c => c.RoomID == id && c.UserID == user.UserID);
            if (existingCartItem != null)
            {
                return new HttpStatusCodeResult(409);
            }

            var cartItem = new Cart
            {
                RoomID = id,
                UserID = user.UserID
            };

            db.Carts.InsertOnSubmit(cartItem);
            db.SubmitChanges();

            return new HttpStatusCodeResult(200);
        }
        public ActionResult DisplayCart()
        {
            var user = (User)Session["user"];
            var cartDetail = (from a in db.Carts
                              join b in db.Rooms on a.RoomID equals b.RoomID
                              where a.UserID == user.UserID
                              select new CartDisplay
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
        public ActionResult Booking(FormCollection form)
        {
            string action = form["action"];
            if (action == "discard")
            {
                var ListRoomId = form.GetValues("roomIds")?.Select(int.Parse).ToList();
                if (ListRoomId == null || !ListRoomId.Any())
                {
                    return new HttpStatusCodeResult(400, "Không có phòng nào được chọn.");
                }
                var user = (User)Session["user"];
                if (user == null) return new HttpStatusCodeResult(401, "Chưa đăng nhập.");
                var cartItems = db.Carts.Where(c => c.UserID == user.UserID && ListRoomId.Contains(c.RoomID)).ToList();
                db.Carts.DeleteAllOnSubmit(cartItems);
                db.SubmitChanges();
                return RedirectToAction("DisplayCart", "Booking");
            }
            else
            {

                var ListRoomId = form.GetValues("roomIds")?.Select(int.Parse).ToList();
                if (ListRoomId == null || !ListRoomId.Any())
                {
                    return new HttpStatusCodeResult(400, "Không có phòng nào được chọn.");
                }
                List<RoomDetail> allRooms = new List<RoomDetail>();
                // Lấy tất cả thông tin phòng, ảnh và chi tiết đặt phòng trong 1 lần truy vấn
                foreach ( var room in ListRoomId)
                {
                    var roomDetail = db.Rooms.Where(r => r.RoomID == room)
                                             .Select(r => new RoomDetail
                                             {
                                                 RoomID = r.RoomID,
                                                 RoomName = r.RoomName,
                                                 Description = r.Description,
                                                 Price = r.Price,
                                                 Percent = db.DiscountDetails.Where(dd => dd.RoomID == r.RoomID)
                                                .Select(dd => (int?)dd.Discount.DiscountPercent)
                                                .FirstOrDefault() ?? 0,
                                                 RoomImages = db.RoomImages.Where(img => img.RoomID == r.RoomID).ToList(),
                                                 Reviews = db.Reviews.Where(rev => rev.RoomID == r.RoomID).ToList()
                                             }).FirstOrDefault();
                    if (roomDetail != null)
                    {
                        allRooms.Add(roomDetail);  // Thêm vào danh sách thay vì ghi đè
                    }
                }
                var bookings = db.BookingDetails
                                 .Where(bd => ListRoomId.Contains(bd.RoomID))
                                 .Select(bd => bd.Booking)
                                 .ToList();

                var allBookedDates = bookings
                    .SelectMany(b => Enumerable.Range(0, (b.CheckOut - b.CheckIn).Days)
                                  .Select(offset => b.CheckIn.AddDays(offset).ToString("dd/MM/yyyy")))
                    .Distinct()
                    .ToList();
                ViewBag.BookedDates = allBookedDates;
                return View(allRooms);
            }
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
                var bookingDetail = new BookingDetail
                {
                    RoomID = roomId,
                    BookingID = newBooking.BookingID,
                    Discount = percent
                };
                db.BookingDetails.InsertOnSubmit(bookingDetail);

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
            var payment = new Payment
            {
                BookingID = newBooking.BookingID,
                PaymentDate = DateTime.Now,
                Amount = tongTien,
                PaymentStatus = "Pending",
            };

            db.Payments.InsertOnSubmit(payment);
            db.SubmitChanges();

            return new HttpStatusCodeResult(202, "Đặt phòng thành công.");

        }

    }
}