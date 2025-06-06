using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hotel.Models
{
	public class RoomDisplay
	{
		public int ID { get; set; }
		public int RoomTypeId { get; set; }
		public string RoomName { get; set; }
		public int Capacity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
		public int Percent { get; set; }
    }
}