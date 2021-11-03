using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EoJSON
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class OBSRequest
    {
        [JsonProperty("request-type")]
        public string RequestType { get; set; }

        [JsonProperty("message-id")]
        public string MessageId { get; set; }

        [JsonProperty("scene-name")]
        public string SceneName { get; set; }
    }


}
