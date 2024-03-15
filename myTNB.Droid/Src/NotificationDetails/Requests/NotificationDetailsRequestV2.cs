using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.NotificationDetails.Requests
{
    public class NotificationDetailsRequestV2
    {
        [JsonProperty("ApiKeyID")]
        public string ApiKeyID { get; set; }

        [JsonProperty("NotificationId")]
        public string NotificationId { get; set; }

        [JsonProperty("NotificationType")]
        public string NotificationType { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("DeviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("SSPUserId")]
        public string SSPUserId { get; set; }
    }
}