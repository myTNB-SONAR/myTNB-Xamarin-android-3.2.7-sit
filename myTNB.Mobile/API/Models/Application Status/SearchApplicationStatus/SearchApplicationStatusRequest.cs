using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public class SearchApplicationStatusRequest : BaseRequest
    {
        [JsonProperty("typeCode")]
        public object TypeCode { get; set; }
        [JsonProperty("refCode")]
        public object RefCode { get; set; }
        [JsonProperty("id")]
        public object ID { get; set; }
    }
}