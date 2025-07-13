using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hotel.Models.HotelModel
{
	public class ProvinceModel
	{
        public int ProvinceID { get; set; }
        public string ProvinceName { get; set; }
        public List<HotelModel> Hotels { get; set; }
    }
}