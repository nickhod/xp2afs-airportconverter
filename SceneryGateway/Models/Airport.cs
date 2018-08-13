using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.SceneryGateway.Models
{
    public class Airport
    {
        [JsonProperty("icao")]
        public string Icao { get; set; }

        [JsonProperty("airportName")]
        public string AirportName { get; set; }

        [JsonProperty("airportClass")]
        public string AirportClass { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("elevation")]
        public string Elevation { get; set; }

        [JsonProperty("acceptedSceneryCount")]
        public int AcceptedSceneryCount { get; set; }

        [JsonProperty("approvedSceneryCount")]
        public int ApprovedSceneryCount { get; set; }

        [JsonProperty("recommendedSceneryId")]
        public int? RecommendedSceneryId { get; set; }

        [JsonProperty("scenery")]
        public List<Scenery> Scenery { get; set; }
    }
}
