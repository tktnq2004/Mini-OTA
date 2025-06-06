using Hotel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows.Documents;

namespace Hotel.Controllers
{
    public class AdminController : Controller
    {
        dbHotelBrandDataContext db = new dbHotelBrandDataContext();

        public ActionResult Index()
        {
            var rooms = db.Rooms.ToList();
            var roomTypes = db.RoomTypes.ToList();
            ViewBag.RoomTypes = roomTypes;
            return View(rooms);
        }
        public ActionResult ListRoom(int roomTypeId)
        {
            List<Room> rooms;
            List<RoomType> roomTypes = db.RoomTypes.ToList();

            if (roomTypeId != 0)
            {
                rooms = db.Rooms.Where(r => r.RoomTypeId == roomTypeId).ToList();
            }
            else
            {
                rooms = db.Rooms.ToList();
            }

            ViewBag.RoomTypes = roomTypes;
            return View("Index", rooms); 
        }

        public ActionResult Add()
        {
            ViewBag.RoomTypes = db.RoomTypes.ToList(); 
            return View(new Room());
        }

        // POST: Admin/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Room room)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Rooms.InsertOnSubmit(room);
                    db.SubmitChanges();
                    TempData["SuccessMessage"] = "Thêm phòng thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi thêm phòng: " + ex.Message);
                }
            }

            ViewBag.RoomTypes = db.RoomTypes.ToList();
            return View(room);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var room = db.Rooms.FirstOrDefault(r => r.RoomID == id);
            if (room == null)
            {
                return HttpNotFound();
            }
            db.Rooms.DeleteOnSubmit(room);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }

        // GET: Lấy thông tin phòng để sửa
        [HttpGet]
        public ActionResult Update(int id)
        {
            var rommTypes = db.RoomTypes.ToList();
            var room = db.Rooms.FirstOrDefault(r => r.RoomID == id);
            if (room == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoomTypes = rommTypes;
            return View(room);
        }

        // POST: Cập nhật phòng
        [HttpPost]
        public ActionResult Update(Room room)
        {
            if (ModelState.IsValid)
            {
                var existingRoom = db.Rooms.FirstOrDefault(r => r.RoomID == room.RoomID);
                if (existingRoom == null)
                {
                    return HttpNotFound();
                }
                existingRoom.RoomName = room.RoomName;
                existingRoom.RoomTypeId = room.RoomTypeId;
                existingRoom.Price = room.Price;
                existingRoom.Capacity = room.Capacity;
                existingRoom.Description = room.Description;
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View(room);
        }
        public ActionResult Images(int id)
        {
            var room = db.Rooms.FirstOrDefault(r => r.RoomID == id);
            if (room == null)
            {
                return HttpNotFound();
            }
            ViewBag.Room = room;
            return View(db.RoomImages.Where(i => i.RoomID == id).ToList());
        }
        [HttpPost]
        public ActionResult ProcessUpload(HttpPostedFileBase file, int roomId)
        {
            if (file == null || roomId <= 0)
            {
                return Json(new { success = false, message = "Không có file hoặc RoomID không hợp lệ" });
            }

            try
            {
                // Tạo đường dẫn lưu file
                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(Server.MapPath("~/Content/images/"), fileName);

                // Lưu file vào thư mục
                file.SaveAs(filePath);

                // Lưu đường dẫn vào database
                string imageUrl = "/Content/images/" + fileName;

                RoomImage newImage = new RoomImage
                {
                    RoomID = roomId,
                    ImageURL = imageUrl
                };
                db.RoomImages.InsertOnSubmit(newImage);
                db.SubmitChanges();
                // Trả về URL ảnh
                return Json(new { success = true, imageUrl = imageUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi upload ảnh: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult DeleteImage(int id)
        {
            var image = db.RoomImages.FirstOrDefault(r => r.ImageID == id);
            if (image != null)
            {
                db.RoomImages.DeleteOnSubmit(image);
                db.SubmitChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public ActionResult BookingStatistic(DateTime? startDate, DateTime? endDate, int? bookingId)
        {
            var query = db.Bookings.AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(b => b.BookingDate >= startDate.Value && b.BookingDate <= endDate);
            }
            if (bookingId.HasValue)
            {
                query = query.Where(b => b.BookingID == bookingId.Value);
            }

            decimal SumOfAmount = 0;
            int SumOfRoom = 0;
            var bookingStats = query.Select(b => new BookingDetailView
            {
                BookingID = b.BookingID,
                UserName = b.User.FullName,
                CheckIn = b.CheckIn,
                CheckOut = b.CheckOut,
                BookingDate = b.BookingDate,
                TotalAmount = db.BookingDetails
                    .Where(d => d.BookingID == b.BookingID)
                    .Sum(d => d.Room.Price),
                Rooms = db.BookingDetails
                    .Where(d => d.BookingID == b.BookingID)
                    .Select(d => new RoomViewModel
                    {
                        RoomName = d.Room.RoomName,
                        Price = d.Room.Price
                    }).ToList(),
                Status = db.Payments.FirstOrDefault(p => p.BookingID == b.BookingID).PaymentStatus,
            }).ToList();
            foreach (var item in bookingStats)
            {
                SumOfAmount += item.TotalAmount;
                SumOfRoom += item.Rooms.Count();
            }
            ViewBag.SumOfBooking = bookingStats.Count();
            ViewBag.SumOfAmount = SumOfAmount;
            ViewBag.SumOfRoom = SumOfRoom;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.Status = new List<string> { "Pending", "Completed", "Cancelled" };
            return View(bookingStats);
        }
        public void UpdateStatus(int bookingId, string status)
        {
            var payment = db.Payments.FirstOrDefault(b => b.BookingID == bookingId);
            if (payment != null)
            {
                payment.PaymentStatus = status;
                db.SubmitChanges();
            }
        }

        public ActionResult BookingDelete(int id)
        {
            var booking = db.Bookings.FirstOrDefault(b => b.BookingID == id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            db.Bookings.DeleteOnSubmit(booking);
            db.SubmitChanges();
            return RedirectToAction("BookingStatistic");
        }
        public ActionResult DiscountStatistic(DateTime? startDate, DateTime? endDate)
        {
            var discounts = db.Discounts.AsQueryable();

            if (startDate.HasValue)
            {
                discounts = discounts.Where(d => d.StartDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                discounts = discounts.Where(d => d.EndDate <= endDate.Value);
            }

            var discountDetails = db.DiscountDetails.ToList();
            var rooms = db.Rooms
              .Where(r => !db.DiscountDetails.Any(dd => dd.RoomID == r.RoomID))
              .ToList();

            var viewModel = (from d in db.Discounts
                             join dd in db.DiscountDetails on d.DiscountID equals dd.DiscountID into discountGroup
                             select new DiscountView
                             {
                                 DiscountID = d.DiscountID,
                                 DiscountPercent = d.DiscountPercent,
                                 StartDate = d.StartDate,
                                 EndDate = d.EndDate,
                                 Rooms = (from dd in discountGroup
                                          join r in db.Rooms on dd.RoomID equals r.RoomID
                                          select r).ToList()
                             }).ToList();

            ViewBag.Rooms = rooms;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(viewModel);
        }
        public void DeleteDiscount(int id)
        {
            var discount = db.Discounts.FirstOrDefault(d => d.DiscountID == id);
            if (discount != null)
            {
                db.Discounts.DeleteOnSubmit(discount);
            }
        }
        public ActionResult AddRoomToDiscount(int discountId, int roomId)
        {
            
            var existingDetail = db.DiscountDetails.FirstOrDefault(d => d.DiscountID == discountId && d.RoomID == roomId);
            if (existingDetail != null)
            {
                return Json(new { success = false, message = "Phòng đã có trong chương trình giảm giá!" });
            }

            var discount = db.Discounts.FirstOrDefault(d => d.DiscountID == discountId);
            var room = db.Rooms.FirstOrDefault(r => r.RoomID == roomId);

            if (discount != null && room != null)
            {
                var newDetail = new DiscountDetail()
                {
                    DiscountID = discountId,
                    RoomID = roomId
                };

                db.DiscountDetails.InsertOnSubmit(newDetail);
                db.SubmitChanges();

                return Json(new { success = true});
            }
            return Json(new { success = false, message = "Không tìm thấy Discount hoặc Phòng!" });
        }

        [HttpPost]
        public ActionResult RemoveRoomFromDiscount(int discountId, int roomId)
        {
            var discountDetail = db.DiscountDetails.FirstOrDefault(d => d.DiscountID == discountId && d.RoomID == roomId);

            if (discountDetail != null)
            {
                db.DiscountDetails.DeleteOnSubmit(discountDetail);
                db.SubmitChanges();

                return Json(new { success = true});
            }

            return Json(new { success = false, message = "Không tìm thấy phòng trong chương trình giảm giá!" });
        }

    }
}