using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.SceneryGateway.Models
{
    public class AirportList
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("airports")]
        public List<AirportListItem> Airports { get; set; }
    }
}
