﻿using System;
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

namespace myTNB_Android.Src.AppLaunch.Models
{
    public class UserNotification
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("DeviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("AccountNum")]
        public string AccountNum { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("IsRead")]
        public bool IsRead { get; set; }

        [JsonProperty("IsDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("NotificationTypeId")]
        public string NotificationTypeId { get; set; }

        [JsonProperty("BCRMNotificationTypeId")]
        public string BCRMNotificationTypeId { get; set; }

        [JsonProperty("CreatedDate")]
        public string CreatedDate { get; set; }

        [JsonProperty("NotificationType")]
        public string NotificationType { get; set; }

        [JsonProperty("Target")]
        public string Target { get; set; }
    }
}
