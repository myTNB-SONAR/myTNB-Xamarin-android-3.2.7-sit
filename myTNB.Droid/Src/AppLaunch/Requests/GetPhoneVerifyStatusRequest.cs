using Newtonsoft.Json;

namespace myTNB_Android.Src.AppLaunch.Requests
{
    public class GetPhoneVerifyStatusRequest
    {
        [JsonProperty("ApiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("SSPUserID")]
        public string SSPUserID { get; set; }

        [JsonProperty("DeviceID")]
        public string DeviceID { get; set; }
    }
}