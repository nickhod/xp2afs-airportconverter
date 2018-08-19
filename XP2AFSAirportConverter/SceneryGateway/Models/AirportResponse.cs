using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.SceneryGateway.Models
{
    public class AirportResponse
    {
        [JsonProperty("airport")]
        public Airport Airport { get; set; }

    }
}
