using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hotel.Models
{
	public class BookingDetailView
	{
        public int BookingID { get; set; }
        public string UserName { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public List<RoomViewModel> Rooms { get; set; }
    }
}