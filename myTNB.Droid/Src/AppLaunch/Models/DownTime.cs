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

namespace myTNB_Android.Src.AppLaunch.Models
{
    public class DownTime
    {
        [JsonProperty(PropertyName = "System")]
        [AliasAs("System")]
        public string System { get; set; }

        [JsonProperty(PropertyName = "IsDown")]
        [AliasAs("IsDown")]
        public bool IsDown { get; set; }

        [JsonProperty(PropertyName = "DowntimeMessage")]
        [AliasAs("DowntimeMessage")]
        public string DowntimeMessage { get; set; }

        [JsonProperty(PropertyName = "DowntimeTextMessage")]
        [AliasAs("DowntimeTextMessage")]
        public string DowntimeTextMessage { get; set; }

        [JsonProperty(PropertyName = "DowntimeStart")]
        [AliasAs("DowntimeStart")]
        public string DowntimeStart { get; set; }

        [JsonProperty(PropertyName = "DowntimeEnd")]
        [AliasAs("DowntimeEnd")]
        public string DowntimeEnd { get; set; }
    }
}