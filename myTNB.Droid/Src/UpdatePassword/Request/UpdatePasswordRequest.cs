using Newtonsoft.Json;

namespace myTNB.Android.Src.UpdatePassword.Request
{
    public class UpdatePasswordRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("currentPassword")]
        public string CurrentPassword { get; set; }

        [JsonProperty("newPassword")]
        public string NewPassword { get; set; }

        [JsonProperty("confirmNewPassword")]
        public string ConfirmNewPassword { get; set; }

        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("clientType")]
        public string ClientType { get; set; }

        [JsonProperty("activeUserName")]
        public string ActiveUserName { get; set; }

        [JsonProperty("devicePlatform")]
        public string DevicePlatform { get; set; }

        [JsonProperty("deviceVersion")]
        public string DeviceVersion { get; set; }

        [JsonProperty("deviceCordova")]
        public string DeviceCordova { get; set; }


    }
}