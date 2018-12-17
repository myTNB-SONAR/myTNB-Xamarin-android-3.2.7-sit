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
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Requests
{
    public class UserNotificationTypeRequest
    {

        [JsonProperty("apiKeyID")]
        [AliasAs("apiKeyID")]
        public string ApiKeyID { get; set; }

        [JsonProperty("email")]
        [AliasAs("email")]
        public string Email { get; set; }

        [JsonProperty("deviceId")]
        [AliasAs("deviceId")]
        public string DeviceId { get; set; }
    }
}