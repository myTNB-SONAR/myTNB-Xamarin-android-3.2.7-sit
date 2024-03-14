using Newtonsoft.Json;

namespace myTNB.Android.Src.NotificationDetails.Models
{
    public class NotificationDetailsDeleteResponse
    {
        [JsonProperty("d")]
        public NotificationDetailsDeleteData Data { get; set; }

        public class NotificationDetailsDeleteData
        {
            [JsonProperty("__type")]
            public string Type { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("isError")]
            public bool IsError { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }
        }
    }
}