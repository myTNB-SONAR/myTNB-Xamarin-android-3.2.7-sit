using Newtonsoft.Json;

namespace myTNB.Android.Src.NotificationDetails.Models
{
    public class NotificationDetailsResponse
    {
        [JsonProperty("d")]
        public NotificationDetailsData Data { get; set; }

        public class NotificationDetailsData
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
            public NotificationDetails Data { get; set; }
        }
    }
}