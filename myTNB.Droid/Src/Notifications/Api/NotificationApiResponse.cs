using System;
using Newtonsoft.Json;

namespace myTNB.Android.Src.Notifications.Api
{
    public class NotificationApiResponse
    {
        [JsonProperty("d")]
        public NotificationData Data { get; set; }

        public class NotificationData
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
            public string Data { get; set; }
        }
    }
}
