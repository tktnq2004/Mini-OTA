using Hotel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hotel.Controllers
{
    public class HotelController : Controller
    {
        // GET: Hotel
        private dbHotelDataContext db = new dbHotelDataContext();

        public ActionResult HotelIndex()
        {
            return View();
        }
        public JsonResult GetAllHotels()
        {
            var hotels = db.Hotels.Select(h => new
            {
                h.HotelID,
                h.HotelName,
                h.Latitude,
                h.Longitude,
                h.Address
            }).ToList();

            return Json(hotels, JsonRequestBehavior.AllowGet);
        }
            public ActionResult GetHotelDetail(int hotelId)
            {
                var hotel = db.Hotels.FirstOrDefault(h => h.HotelID == hotelId);
            return PartialView("HotelDetail", hotel);
            }

    }
}