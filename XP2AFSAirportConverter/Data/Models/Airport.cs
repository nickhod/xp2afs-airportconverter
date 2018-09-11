using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.Data.Models
{
    public class Airport
    {
        [Key]
        public long AirportId { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string ICAO { get; set; }

        public string IATA { get; set; }

        public string FAACode { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        [ForeignKey("Country")]
        public long? CountryId { get; set; }

        [ForeignKey("Region")]
        public long? RegionId { get; set; }

        [ForeignKey("City")]
        public long? CityId { get; set; }

        public bool IsHelipad { get; set; }

        public int RunwaySurfaceType { get; set; }

        public bool HasPavement { get; set; }

        public int RunwayCount { get; set; }

        public string RegionCode { get; set; }

        public DateTime UpdatedOn { get; set; }

        public bool Hidden { get; set; }

        public string CheckedOutByUserId { get; set; }

        public DateTime? CheckedOutOn { get; set; }

        public string CheckOutComment { get; set; }

        public bool XP3D { get; set; }

        public virtual City City { get; set; }
        public virtual Country Country { get; set; }
        public virtual Region Region { get; set; }

    }
}
