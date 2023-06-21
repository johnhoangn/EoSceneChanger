using System;
using Newtonsoft.Json;

namespace EoJSON {
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class D {
        [JsonProperty("requestType")]
        public string RequestType { get; set; }

        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        public RequestData requestData { get; set; }

        public D(string requestType, String name) {
            RequestId = Guid.NewGuid().ToString();
            RequestType = requestType;
            requestData = new RequestData {
                sceneName = name
            };
        }
    }

    public class RequestData {
        public string sceneName { get; set; }
    }

    public class OBSRequest {
        [JsonProperty("op")]
        public int Operation = 6;

        [JsonProperty("d")]
        public D Data { get; set; }

        public OBSRequest(int opcode, string requestType, String sceneName) {
            Operation = opcode;
            Data = new D(requestType, sceneName);
        }
    }
}
