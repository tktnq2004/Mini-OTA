using Hotel.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Globalization;

namespace Hotel.Controllers
{
    public class RoomController : Controller
    {
        dbHotelDataContext db = new dbHotelDataContext();
        // GET: Room
        private List<RoomDisplay> GetAvailableRooms(int hotelID, DateTime? fromDate, DateTime? toDate)
        {
            var bookedRoomIds = db.BookingDetails
                .Where(bd => db.Bookings.Any(b =>
                    b.BookingID == bd.BookingID &&
                    b.CheckIn < toDate && b.CheckOut > fromDate))
                .Select(bd => bd.RoomID)
                .Distinct()
                .ToList();

            var rooms = (from r in db.Rooms
                         where r.HotelID == hotelID
                         && !bookedRoomIds.Contains(r.RoomID)
                         join dd in db.DiscountDetails on r.RoomID equals dd.RoomID into discountGroup
                         from dd in discountGroup.DefaultIfEmpty()
                         join d in db.Discounts on dd.DiscountID equals d.DiscountID into discountInfo
                         from d in discountInfo.DefaultIfEmpty()
                         select new RoomDisplay
                         {
                             ID = r.RoomID,
                             RoomTypeId = r.RoomTypeId,
                             RoomName = r.RoomName,
                             Capacity = r.Capacity,
                             Price = r.Price,
                             Description = r.Description,
                             Percent = d != null ? d.DiscountPercent : 0
                         }).ToList();

            return rooms;
        }

        public ActionResult RoomIndex(int hotelID, DateTime? fromDate, DateTime? toDate, int? rooms, int? adults, int? children)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.Rooms = rooms;
            ViewBag.Adults = adults;
            ViewBag.Children = children;
            ViewBag.HotelID = hotelID;

            ViewBag.RoomTypes = db.RoomTypes.ToList();
            var availableRooms = GetAvailableRooms(hotelID, fromDate, toDate);
            return View(availableRooms);
        }

        public ActionResult UpdateFilter(int hotelID, DateTime? fromDate, DateTime? toDate, int? rooms, int? adults, int? children)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.Rooms = rooms;
            ViewBag.Adults = adults;
            ViewBag.Children = children;
            ViewBag.HotelID = hotelID;

            ViewBag.RoomTypes = db.RoomTypes.ToList();
            var availableRooms = GetAvailableRooms(hotelID, fromDate, toDate);
            return View("RoomIndex", availableRooms);
        }

        public ActionResult FilterRooms(int? roomTypeId, string dateRange, string sortByPrice = "asc", int page = 1)
        {
            try
            {
                int pageSize = 6; // Số phòng mỗi trang

                var rooms = (from r in db.Rooms
                             join dd in db.DiscountDetails on r.RoomID equals dd.RoomID into discountGroup
                             from dd in discountGroup.DefaultIfEmpty()
                             join d in db.Discounts on dd.DiscountID equals d.DiscountID into discountInfo
                             from d in discountInfo.DefaultIfEmpty()
                             select new RoomDisplay
                             {
                                 ID = r.RoomID,
                                 RoomTypeId = r.RoomTypeId,
                                 RoomName = r.RoomName,
                                 Capacity = r.Capacity,
                                 Price = r.Price,
                                 Description = r.Description,
                                 Percent = d != null ? d.DiscountPercent : 0,
                             });

                if (roomTypeId.HasValue)
                {
                    rooms = rooms.Where(r => r.RoomTypeId == roomTypeId);
                }

                if (!string.IsNullOrEmpty(dateRange))
                {
                    var dates = dateRange.Split(new[] { " - ", " to " }, StringSplitOptions.RemoveEmptyEntries);
                    if (dates.Length == 2
                        && DateTime.TryParse(dates[0], out DateTime checkIn)
                        && DateTime.TryParse(dates[1], out DateTime checkOut))
                    {
                        var bookedRooms = db.BookingDetails
                            .Where(bd => db.Bookings
                                .Any(b => b.BookingID == bd.BookingID &&
                                          b.CheckIn < checkOut && b.CheckOut > checkIn))
                            .Select(bd => bd.RoomID)
                            .Distinct()
                            .ToList(); // Lấy danh sách RoomID đã đặt

                        rooms = rooms.Where(r => !bookedRooms.Contains(r.ID)); // Loại bỏ các phòng đã đặt
                    }
                }

                if (sortByPrice == "asc")
                {
                    rooms = rooms.OrderBy(r => r.Price);
                }
                else
                {
                    rooms = rooms.OrderByDescending(r => r.Price);
                }

                var pagedRooms = rooms.ToPagedList(page, pageSize);
                return PartialView("Display", pagedRooms);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi Controller: " + ex.Message);
                return new HttpStatusCodeResult(500, "Lỗi xử lý dữ liệu");
            }
        }

        public ActionResult GetRoomImage(int? page, int roomId)
        {
            var image = db.RoomImages.FirstOrDefault(i => i.RoomID == roomId);
            if (image != null && !string.IsNullOrEmpty(image.ImageURL))
            {
                return Redirect(image.ImageURL);
            }
            return new EmptyResult();
        }
        public ActionResult GetRoomDetail(int roomId)
        {
            var images = db.RoomImages
                           .Where(i => i.RoomID == roomId)
                           .ToList();

            return PartialView("RoomDetail", images);
        }
    }
}