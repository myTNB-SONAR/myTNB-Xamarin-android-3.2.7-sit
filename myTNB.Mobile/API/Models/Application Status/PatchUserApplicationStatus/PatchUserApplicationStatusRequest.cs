using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public class PatchUserApplicationStatusRequest : BaseRequest
    {
        [JsonProperty("type")]
        public object Type { get; set; }
        [JsonProperty("idTypeCode")]
        public object IdTypeCode { get; set; }
        [JsonProperty("id")]
        public object ID { get; set; }
    }
}