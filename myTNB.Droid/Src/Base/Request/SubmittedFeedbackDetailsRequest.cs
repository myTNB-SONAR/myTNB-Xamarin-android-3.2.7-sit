using Newtonsoft.Json;

namespace myTNB_Android.Src.Base.Request
{
    public class SubmittedFeedbackDetailsRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("serviceReqNo")]
        public string ServiceReqNo { get; set; }
    }
}