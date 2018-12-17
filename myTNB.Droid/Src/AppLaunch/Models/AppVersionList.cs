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

namespace myTNB_Android.Src.AppLaunch.Models
{
    public class AppVersionList
    {
        [JsonProperty(PropertyName = "Platform")]
        [AliasAs("Platform")]
        public string Platform { get; set; }

        [JsonProperty(PropertyName = "Version")]
        [AliasAs("Version")]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "Features")]
        [AliasAs("Features")]
        public List<FeatureObj> Features { get; set; }

        public class FeatureObj
        {
            [JsonProperty(PropertyName = "Feature")]
            [AliasAs("Feature")]
            public string Feature { get; set; }
        }
    }
}