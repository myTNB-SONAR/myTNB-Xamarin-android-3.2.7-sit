using Newtonsoft.Json;

namespace myTNB.Android.Src.AppLaunch.Requests
{
    public class FeedbackCategoryRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }
    }
}