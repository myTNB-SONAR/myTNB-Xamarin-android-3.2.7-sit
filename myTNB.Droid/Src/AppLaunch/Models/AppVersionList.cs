using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.AppLaunch.Models
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