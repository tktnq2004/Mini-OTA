using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hotel.Models
{
    public class PaymentDisplay
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int SoDem { get; set; }
        public DateTime LastUpdate { get; set; }
        public List<RoomDetail> RoomDetails { get; set; }
        public string QRCode { get; set; }
    }
}