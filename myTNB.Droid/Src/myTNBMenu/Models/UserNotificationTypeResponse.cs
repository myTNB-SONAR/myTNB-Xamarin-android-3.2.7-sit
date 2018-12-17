using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Models
{
    public class UserNotificationTypeResponse
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

            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("data")]
            public List<UserNotificationType> Data { get; set; }


        }
    }
}