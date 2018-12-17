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

namespace myTNB_Android.Src.NotificationSettings.Models
{
    public class UserPreferenceData
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Email")]
        public string Email { get; set; }
        [JsonProperty("DeviceId")]
        public string DeviceId { get; set; }
        [JsonProperty("NotificationTypeId")]
        public string NotificationTypeId { get; set; }
        [JsonProperty("IsOpted")]
        public bool IsOpted { get; set; }
    }
}