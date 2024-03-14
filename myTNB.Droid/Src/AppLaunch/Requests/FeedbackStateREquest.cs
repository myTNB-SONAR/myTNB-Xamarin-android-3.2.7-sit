using Newtonsoft.Json;

namespace myTNB.Android.Src.AppLaunch.Requests
{
    public class FeedbackStateRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }
    }
}