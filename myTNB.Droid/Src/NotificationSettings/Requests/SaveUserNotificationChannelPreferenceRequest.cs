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
    public class SaveUserNotificationChannelPreferenceRequest
    {
        [JsonProperty("apiKeyID")]
        [AliasAs("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("id")]
        [AliasAs("id")]
        public string Id { get; set; }

        [JsonProperty("email")]
        [AliasAs("email")]
        public string Email { get; set; }

        [JsonProperty("channelTypeId")]
        [AliasAs("channelTypeId")]
        public string ChannelTypeId { get; set; }

        [JsonProperty("isOpted")]
        [AliasAs("isOpted")]
        public bool IsOpted { get; set; }
    }
}