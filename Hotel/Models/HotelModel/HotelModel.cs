using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hotel.Models.HotelModel
{
    public class HotelModel
    {
        public int HotelID { get; set; }
        public string HotelName { get; set; }
        public string Address { get; set; }
        public string HotelImage { get; set; }
        public decimal Latitude { get; set; }
        public List<Amenity> Amenities { get; set; }
        public List<View> Views { get; set; }
        public decimal Longitude { get; set; }
    }
}
