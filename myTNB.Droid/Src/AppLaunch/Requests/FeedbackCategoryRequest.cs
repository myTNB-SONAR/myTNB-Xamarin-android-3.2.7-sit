using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.AppLaunch.Requests
{
    public class FeedbackCategoryRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }
    }
}