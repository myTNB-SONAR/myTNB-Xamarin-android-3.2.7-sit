using Newtonsoft.Json;

namespace myTNB.Android.Src.myTNBMenu.Models
{
    public class UserNotificationType
    {
        //  "Id": "",
        //"Title": "Billing & Payment",
        //"Code": "BP",
        //"PreferenceMode": "M",
        //"Type": "ANNOUNCEMENT",
        //"CreatedDate": "",
        //"MasterId": "1000004",
        //"IsOpted": "false"

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("PreferenceMode")]
        public string PreferenceMode { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("CreatedDate")]
        public string CreatedDate { get; set; }

        [JsonProperty("MasterId")]
        public string MasterId { get; set; }

        [JsonProperty("IsOpted")]
        public bool IsOpted { get; set; }

        [JsonProperty("ShowInPreference")]
        public bool ShowInPreference { get; set; }

        [JsonProperty("ShowInFilterList")]
        public bool ShowInFilterList { get; set; }


    }
}