using Newtonsoft.Json;

namespace myTNB.Android.Src.Base.Request
{
    public class SubmittedFeedbackRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }
    }
}