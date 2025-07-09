using Hotel.Models;
using Hotel.Models.HotelModel;
using System;
using System.Collections.Generic;
using System.Drawing;
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
                            Latitude = p.Latitude,
                            Longitude = p.Longitude,
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

        public JsonResult GetProvincesByRegionId(int regionId)
        {
            var provinces = db.Provinces
                              .Where(p => p.RegionID == regionId && db.Hotels.Any(h => h.ProvinceID == p.ProvinceID))
                              .Select(p => new { p.ProvinceID, p.ProvinceName })
                              .ToList();

            return Json(provinces, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHotelsByProvince(int provinceId)
        {
            var hotels = db.Hotels
                           .Where(h => h.ProvinceID == provinceId)
                           .Select(h => new
                           {
                               h.HotelID,
                               h.HotelName,
                               h.Latitude,
                               h.Longitude
                           })
                           .ToList();

            return Json(hotels, JsonRequestBehavior.AllowGet);
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