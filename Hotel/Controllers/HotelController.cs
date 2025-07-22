using Hotel.Models;
using Hotel.Models.HotelModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Windows.Media.Animation;

namespace Hotel.Controllers
{
    public class HotelController : Controller
    {
        // GET: Hotel
        private dbHotelDataContext db = new dbHotelDataContext();
        public ActionResult HotelMain()
        {

            return View();
        }
        public ActionResult HotelIndex()
        {
            var data = db.Regions
                .Select(r => new RegionModel
                {
                    RegionID = r.RegionID,
                    RegionName = r.RegionName,
                    Provinces = r.Provinces
                        .Where(p => p.Hotels.Any())
                        .Select(p => new ProvinceModel
                        {
                            ProvinceID = p.ProvinceID,
                            ProvinceName = p.ProvinceName,
                            Hotels = p.Hotels.Select(h => new HotelModel
                            {
                                HotelID = h.HotelID,
                                HotelName = h.HotelName,
                                Address = h.Address,
                                HotelImage = h.HotelImage,
                                Latitude = h.Latitude,
                                Longitude = h.Longitude
                            }).ToList()
                        }).ToList()
                }).ToList();

            return View(data);
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
        public JsonResult GetProvincesByRegionId(int regionId)
        {
            var provinces = db.Provinces
                              .Where(p => p.RegionID == regionId && db.Hotels.Any(h => h.ProvinceID == p.ProvinceID))
                              .Select(p => new { p.ProvinceID, p.ProvinceName })
                              .ToList();

            return Json(provinces, JsonRequestBehavior.AllowGet);
        }
        
        public List<HotelModel> GetHotelsByProvinceId(int provinceId)
        {
            var hotels = db.Hotels
                .Where(h => h.ProvinceID == provinceId)
                .Select(h => new HotelModel
                {
                    HotelID = h.HotelID,
                    HotelName = h.HotelName,
                    Address = h.Address,
                    HotelImage = h.HotelImage,
                    Latitude = h.Latitude,
                    Longitude = h.Longitude,
                    Amenities = db.RoomAmenities
                        .Where(ra => ra.Room.HotelID == h.HotelID)
                        .Select(ra => ra.Amenity)
                        .Distinct()
                        .ToList(),
                    Views = db.RoomViews
                        .Where(rv => rv.Room.HotelID == h.HotelID)
                        .Select(rv => rv.View)
                        .Distinct()
                        .ToList()
                }).ToList();
            return hotels;
        }
        public ActionResult GetHotelDetail(int hotelID)
        {
            var currentHotel = db.Hotels.FirstOrDefault(h => h.HotelID == hotelID);

            var selectedHotelModel = new HotelModel
            {
                HotelID = currentHotel.HotelID,
                HotelName = currentHotel.HotelName,
                Address = currentHotel.Address,
                HotelImage = currentHotel.HotelImage,
                Latitude = currentHotel.Latitude,
                Longitude = currentHotel.Longitude,
                Amenities = db.RoomAmenities
                    .Where(ra => ra.Room.HotelID == currentHotel.HotelID)
                    .Select(ra => ra.Amenity)
                    .Distinct()
                    .ToList(),
                Views = db.RoomViews
                    .Where(rv => rv.Room.HotelID == currentHotel.HotelID)
                    .Select(rv => rv.View)
                    .Distinct()
                    .ToList()
            };

            var otherHotels = GetHotelsByProvinceId(currentHotel.ProvinceID);

            Session["CurrentHotel"] = selectedHotelModel;
            Session["OtherHotels"] = otherHotels;

            return PartialView("HotelDetail");
        }

        public ActionResult SearchHotel(string keyWord)
        {
            var allHotels = Session["OtherHotels"] as List<HotelModel>;
            var filtered = allHotels
                             .Where(h => h.HotelName.ToLower().Contains(keyWord.ToLower()))
                             .ToList();
            return PartialView("OtherHotel", filtered);
        }
        [HttpGet]
        public ActionResult GetNearbyHotels(decimal latitude, decimal longitude, decimal radiusKm = 10)
        {
            try
            {
                var allHotels = db.Hotels
                    .Where(h => h.Latitude != 0 && h.Longitude != 0)
                    .ToList();

                var nearbyHotels = allHotels
                    .Where(h => GetDistance((double)latitude, (double)longitude, (double)h.Latitude, (double)h.Longitude) <= (double)radiusKm)
                    .Select(h => new {
                        h.HotelID,
                        h.HotelName,
                        h.Latitude,
                        h.Longitude,
                        h.Address
                    }).ToList();

                return Json(nearbyHotels, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message); // Trả lỗi dễ debug
            }
        }


        // Haversine sử dụng double
        private double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371.0;
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        private double ToRad(double deg) => deg * (Math.PI / 180);


    }
}