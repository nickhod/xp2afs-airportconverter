using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.SceneryGateway.Models
{
    public class AirportListItem
    {
        public string AirportCode { get; set; }
        public string AirportName { get; set; }
        public string AirportClass { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string AcceptedSceneryCount { get; set; }
        public string ApprovedSceneryCount { get; set; }
        public string RecommendedSceneryId { get; set; }
        public string Status { get; set; }
        public string SceneryType { get; set; }
        public string ExcludeSubmissions { get; set; }
        public string SubmissionCount { get; set; }
    }
}
