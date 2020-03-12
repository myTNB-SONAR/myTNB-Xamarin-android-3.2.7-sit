using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.AppLaunch.Requests
{
    public class NotificationRequest
    {
        [JsonProperty("apiKeyID")]
        [AliasAs("apiKeyID")]
        public string ApiKeyId { get; set; }
    }
}