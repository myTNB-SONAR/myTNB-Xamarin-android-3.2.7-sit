using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.AppLaunch.Requests
{
    public class FeedbackStateRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }
    }
}