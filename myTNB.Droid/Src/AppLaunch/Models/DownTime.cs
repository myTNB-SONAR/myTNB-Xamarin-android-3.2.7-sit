using Newtonsoft.Json;
using Refit;

namespace myTNB.AndroidApp.Src.AppLaunch.Models
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