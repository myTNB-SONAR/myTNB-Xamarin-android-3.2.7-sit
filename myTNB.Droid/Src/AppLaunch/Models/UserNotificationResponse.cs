﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB_Android.Src.AppLaunch.Models
{
    public class UserNotificationResponse
    {
        [JsonProperty("d")]
        public UserNotificationData Data { get; set; }

        public class UserNotificationData
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
            public List<UserNotification> Data { get; set; }
        }
    }
}