using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.NotificationSettings.Models
{
    public class UserPreferenceData
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Email")]
        public string Email { get; set; }
        [JsonProperty("DeviceId")]
        public string DeviceId { get; set; }
        [JsonProperty("NotificationTypeId")]
        public string NotificationTypeId { get; set; }
        [JsonProperty("IsOpted")]
        public bool IsOpted { get; set; }
    }
}