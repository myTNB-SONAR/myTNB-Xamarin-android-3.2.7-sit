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
using Refit;
using Newtonsoft.Json;

namespace myTNB_Android.Src.NotificationSettings.Requests
{
    public class SaveUserNotificationTypePreferenceRequest
    {
        [JsonProperty("apiKeyID")]
        [AliasAs("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("id")]
        [AliasAs("id")]
        public string Id { get; set; }

        [JsonProperty("deviceId")]
        [AliasAs("deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("email")]
        [AliasAs("email")]
        public string Email { get; set; }

        [JsonProperty("notificationTypeId")]
        [AliasAs("notificationTypeId")]
        public string NotificationTypeId { get; set; }

        [JsonProperty("isOpted")]
        [AliasAs("isOpted")]
        public bool IsOpted { get; set; }
    }
}