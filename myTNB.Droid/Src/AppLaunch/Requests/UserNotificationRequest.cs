using Newtonsoft.Json;
using Refit;

namespace myTNB.AndroidApp.Src.AppLaunch.Requests
{
    public class UserNotificationRequest
    {
        [JsonProperty("apiKeyID")]
        [AliasAs("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("email")]
        [AliasAs("email")]
        public string Email { get; set; }

        [JsonProperty("deviceId")]
        [AliasAs("deviceId")]
        public string DeviceId { get; set; }


    }
}