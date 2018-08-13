using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.SceneryGateway.Models
{
    public class Metadata
    {
        [JsonProperty("icao_code")]
        public string IcaoCode { get; set; }

        [JsonProperty("iata_code")]
        public string IataCode { get; set; }

        [JsonProperty("faa_code")]
        public string FaaCode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("region_code")]
        public string RegionCode { get; set; }

        [JsonProperty("datum_lat")]
        public string DatumLat { get; set; }

        [JsonProperty("datum_lon")]
        public string DatumLon { get; set; }

        [JsonProperty("transition_alt")]
        public string TransitionAlt { get; set; }

        [JsonProperty("transition_level")]
        public string TransitionLevel { get; set; }
    }
}
