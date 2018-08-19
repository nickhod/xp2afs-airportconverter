using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Common;
using XP2AFSAirportConverter.SceneryGateway.Models;

namespace XP2AFSAirportConverter.SceneryGateway
{
    public class SceneryGatewayApi
    {
        private string baseUrl;

        public SceneryGatewayApi()
        {
            this.baseUrl = "http://gateway.x-plane.com/apiv1/";
        }

        public AirportList GetAllAirports()
        {
            using (var client = new BetterWebClient())
            {
                client.Encoding = Encoding.UTF8;
                var airportsString = client.DownloadString(this.baseUrl + "airports");

                var airportList = JsonConvert.DeserializeObject<AirportList>(airportsString);
                return airportList;
            }
        }

        public Airport GetAirport(string icaoCode)
        {
            using (var client = new BetterWebClient())
            {
                client.Encoding = Encoding.UTF8;
                var airportString = client.DownloadString(this.baseUrl + "airport/" + icaoCode);

                var airportResponse = JsonConvert.DeserializeObject<AirportResponse>(airportString);
                return airportResponse.Airport;
            }
        }
        public Scenery GetScenery(int sceneryPackId)
        {
            using (var client = new BetterWebClient())
            {
                client.Encoding = Encoding.UTF8;
                var sceneryScript = client.DownloadString(this.baseUrl + "scenery/" + sceneryPackId.ToString());

                var sceneryResponse = JsonConvert.DeserializeObject<SceneryResponse>(sceneryScript);
                return sceneryResponse.Scenery;
            }
        }
    }
}
