using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.AppLaunch.Models
{
    public class NotificationTypesResponse
    {
        [JsonProperty("d")]
        public NotificationTypesData Data { get; set; }

        public class NotificationTypesData
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
            public List<NotificationTypes> Data { get; set; }
        }
    }
}