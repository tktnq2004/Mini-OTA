using Hotel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hotel.Controllers
{
    public class PaymentController : Controller
    {
        dbHotelDataContext db = new dbHotelDataContext();
        // GET: Payment
        public ActionResult History()
        {
            var user = (User)Session["user"];

            var paymentDisplay = (from p in db.Payments
                                  join b in db.Bookings on p.BookingID equals b.BookingID
                                  where b.UserID == user.UserID
                                  select new PaymentDisplay
                                  {
                                      PaymentId = p.PaymentID,
                                      Amount = p.Amount,
                                      Status = p.PaymentStatus,
                                      CheckIn = b.CheckIn,
                                      CheckOut = b.CheckOut,
                                      SoDem = (b.CheckOut - b.CheckIn).Days,
                                      LastUpdate = p.PaymentDate,
                                      RoomDetails = (from bd in db.BookingDetails
                                                     join r in db.Rooms on bd.RoomID equals r.RoomID
                                                     where bd.BookingID == b.BookingID
                                                     select new RoomDetail
                                                     {
                                                         RoomID = r.RoomID,
                                                         RoomName = r.RoomName,
                                                         Price = r.Price,
                                                         Percent = bd.Discount,
                                                         RoomImages = db.RoomImages
                                                             .Where(img => img.RoomID == r.RoomID).ToList()
                                                     }).ToList(),
                                        QRCode = new GenQRCode().QRCode(p.Amount, p.PaymentID)
                                  }).ToList();
            return View("History", paymentDisplay);
        }

    }
}