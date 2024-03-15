using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.NotificationSettings.Models
{
    public class SaveUserPreferenceResponse
    {
        [JsonProperty("d")]
        public SaveUserPreferenceData Data;

        public class SaveUserPreferenceData
        {
            [JsonProperty("__type")]
            public string Type { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("isError")]
            public bool IsError { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("data")]
            public UserPreferenceData Data { get; set; }
        }
    }
}