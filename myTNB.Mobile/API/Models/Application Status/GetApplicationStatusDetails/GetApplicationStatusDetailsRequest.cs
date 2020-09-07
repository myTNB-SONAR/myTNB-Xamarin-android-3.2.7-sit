using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public class GetApplicationStatusDetailsRequest : BaseRequest
    {
        [JsonProperty("refCode")]
        public object RefCode { get; set; }
        [JsonProperty("id")]
        public object ID { get; set; }
    }
}