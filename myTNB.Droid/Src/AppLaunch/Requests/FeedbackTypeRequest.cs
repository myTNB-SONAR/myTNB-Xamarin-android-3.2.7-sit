using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.AppLaunch.Requests
{
    public class FeedbackTypeRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }
    }
}