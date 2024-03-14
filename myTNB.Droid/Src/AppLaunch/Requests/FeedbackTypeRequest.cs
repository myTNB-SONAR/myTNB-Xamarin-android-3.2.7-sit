using Newtonsoft.Json;

namespace myTNB.Android.Src.AppLaunch.Requests
{
    public class FeedbackTypeRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }
    }
}