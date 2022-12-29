using System;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models.DBR.AutoOptIn
{
    public class GetAutoOptInCaRequest
    {
        [JsonProperty("Cano")]
        public string Cano { set; get; }
        [JsonProperty("UserId")]
        public string UserId { set; get; }
    }
}
