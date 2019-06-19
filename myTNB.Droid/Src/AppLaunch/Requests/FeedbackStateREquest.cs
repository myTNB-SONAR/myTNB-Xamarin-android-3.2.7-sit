using Newtonsoft.Json;

namespace myTNB_Android.Src.AppLaunch.Requests
{
    public class FeedbackStateRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }
    }
}