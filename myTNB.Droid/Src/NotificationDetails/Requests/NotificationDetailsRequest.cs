using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.NotificationDetails.Requests
{
    public class NotificationDetailsRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyID { get; set; }

        [JsonProperty("notificationId")]
        public string NotificationId { get; set; }
    }
}