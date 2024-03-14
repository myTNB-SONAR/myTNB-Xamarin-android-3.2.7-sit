using Newtonsoft.Json;
using Refit;

namespace myTNB.Android.Src.NotificationSettings.Requests
{
    public class SaveUserNotificationChannelPreferenceRequest
    {
        [JsonProperty("apiKeyID")]
        [AliasAs("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("id")]
        [AliasAs("id")]
        public string Id { get; set; }

        [JsonProperty("email")]
        [AliasAs("email")]
        public string Email { get; set; }

        [JsonProperty("channelTypeId")]
        [AliasAs("channelTypeId")]
        public string ChannelTypeId { get; set; }

        [JsonProperty("isOpted")]
        [AliasAs("isOpted")]
        public bool IsOpted { get; set; }
    }
}