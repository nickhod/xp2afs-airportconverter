using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.SceneryGateway.Models
{
    public class Scenery
    {
        [JsonProperty("sceneryId")]
        public string SceneryId { get; set; }

        [JsonProperty("parentId")]
        public string ParentId { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("dateUploaded")]
        public string DateUploaded { get; set; }

        [JsonProperty("dateAccepted")]
        public string DateAccepted { get; set; }

        [JsonProperty("dateApproved")]
        public string DateApproved { get; set; }

        [JsonProperty("dateDeclined")]
        public string DateDeclined { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("features")]
        public string Features { get; set; }

        [JsonProperty("artistComments")]
        public string ArtistComments { get; set; }

        [JsonProperty("moderatorComments")]
        public string ModeratorComments { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("masterZipBlob")]
        public string MasterZipBlob { get; set; }

        [JsonProperty("additionalMetadata")]
        public Metadata AdditionalMetadata { get; set; }
    }
}
