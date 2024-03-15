using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.myTNBMenu.Models
{
    public class UserNotificationChannelResponse
    {
        [JsonProperty("d")]
        public UserNotificationData Data { get; set; }

        public class UserNotificationData
        {
            //"__type": "mytnbapp.service.models.NotificationTypesStatus",
            //"status": "success",
            //"isError": "false",
            //"message": "Successful"


            [JsonProperty("__type")]
            public string Type { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("isError")]
            public bool IsError { get; set; }

            [JsonProperty("Message")]
            public string Message { get; set; }

            [JsonProperty("data")]
            public List<UserNotificationChannel> Data { get; set; }
        }
    }
}