using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hotel.Models.HotelModel
{
	public class RegionModel
	{
        public int RegionID { get; set; }
        public string RegionName { get; set; }
        public List<ProvinceModel> Provinces { get; set; }
    }
}