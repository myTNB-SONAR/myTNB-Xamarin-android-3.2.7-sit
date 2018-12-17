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

namespace myTNB_Android.Src.NotificationDetails.Requests
{
    public class NotificationDetailsRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyID { get; set; }

        [JsonProperty("notificationId")]
        public string NotificationId { get; set; }
    }
}