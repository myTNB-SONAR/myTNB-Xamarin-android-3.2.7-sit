using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models
{
    public class NotFoundModel
    {
        [JsonProperty("type")]
        public string Type { set; get; }

        [JsonProperty("title")]
        public string Title { set; get; }

        [JsonProperty("status")]
        public string Status { set; get; }

        [JsonProperty("traceId")]
        public string TraceID { set; get; }
    }
}