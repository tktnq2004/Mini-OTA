using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hotel.Models
{
	public class WishlistDisplay
	{
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Percent { get; set; }
        public decimal DiscountPrice { get; set; }
        public int Capacity { get; set; }
        public List<RoomImage> RoomImages { get; set; }

    }
}