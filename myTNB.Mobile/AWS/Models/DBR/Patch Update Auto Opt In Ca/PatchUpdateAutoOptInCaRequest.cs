using System;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models.DBR
{
    public class PatchUpdateAutoOptInCaRequest
    {
        [JsonProperty("CaNo")]
        public string CaNo { set; get; }
        [JsonProperty("UserId")]
        public string UserId { set; get; }
    }
}
