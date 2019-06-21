using Newtonsoft.Json;

namespace myTNB_Android.Src.ManageSupplyAccount.Request
{
    public class RemoveTNBAccountForUserFavRequest
    {
        [JsonProperty("userID")]
        public string UserID { get; set; }

        [JsonProperty("accNum")]
        public string AccountNumber { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

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