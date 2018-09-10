using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.Models
{
    public class AirportCsvListItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public bool IsHelipad { get; set; }
        public bool Is3D { get; set; }
        public bool HasPavements { get; set; }
        public int RunwaySurfaceType { get; set; }
        public string ICAO { get; set; }
        public string IATA { get; set; }
        public string FAACode { get; set; }
        public string RegionCode { get; set; }
        public int RunwayCount { get; set; }
    }
}
