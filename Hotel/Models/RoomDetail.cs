using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hotel.Models
{
	public class RoomDetail
	{
		public int RoomID { get; set; }
        public string RoomName { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public int Percent { get; set; }
        public List<RoomImage> RoomImages { get; set; }
		public List<Review> Reviews { get; set; }
    }
}