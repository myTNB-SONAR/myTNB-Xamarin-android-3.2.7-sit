using Newtonsoft.Json;

namespace myTNB_Android.Src.AppLaunch.Requests
{
    public class FeedbackTypeRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }
    }
}