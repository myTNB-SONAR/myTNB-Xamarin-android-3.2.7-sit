using Newtonsoft.Json;

namespace myTNB.Android.Src.LogoutRate.Request
{
    public class LogoutRequestV2
    {
        [JsonProperty("ApiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("DeviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("AppVersion")]
        public string AppVersion { get; set; }

        [JsonProperty("OsType")]
        public string OsType { get; set; }

        [JsonProperty("OsVersion")]
        public string OsVersion { get; set; }
    }
}