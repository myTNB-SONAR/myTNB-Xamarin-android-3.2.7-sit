using myTNB.Android.Src.Database.Model;
using Newtonsoft.Json;

namespace myTNB.Android.Src.Base.Models
{
    public class UserInterface
    {
        [JsonProperty("eid")]
        public string eid { get; set; }  //Email ID

        [JsonProperty("sspuid")]
        public string sspuid { get; set; } // SSP User ID

        [JsonProperty("did")]
        public string did { get; set; } // Device ID

        [JsonProperty("ft")]
        public string ft { get; set; } //FCM Token

        [JsonProperty("lang")]
        public string lang { get; set; } //User Preferred Language, Default will be English

        [JsonProperty("sec_auth_k1")]
        public string sec_auth_k1 { get; set; } //Our favorite APIKey goes in here

        [JsonProperty("sec_auth_k2")]
        public string sec_auth_k2 { get; set; } //Leave empty for now

        [JsonProperty("ses_param1")]
        public string ses_param1 { get; set; } //Leave empty for now

        [JsonProperty("ses_param2")]
        public string ses_param2 { get; set; } //Leave empty for now
    }
}