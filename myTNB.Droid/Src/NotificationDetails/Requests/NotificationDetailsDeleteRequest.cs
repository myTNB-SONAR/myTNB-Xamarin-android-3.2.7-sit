using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.NotificationDetails.Requests
{
    public class NotificationDetailsDeleteRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("id")]
        public string NotificationId { get; set; }

    }
}