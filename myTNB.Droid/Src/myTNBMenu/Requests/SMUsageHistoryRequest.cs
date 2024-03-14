using myTNB.Android.Src.Base.Models;
using Newtonsoft.Json;
using Refit;

namespace myTNB.Android.Src.myTNBMenu.Requests
{
    public class SMUsageHistoryRequest
    {
        [JsonProperty("metercode")]
        [AliasAs("metercode")]
        public string MeterCode { get; set; }

        [JsonProperty("contractAccount")]
        public string AccountNumber { get; set; }

        [JsonProperty("isOwner")]
        public string isOwner { get; set; }

        [JsonProperty("usrInf")]
        public UserInterface userInterface { get; set; }
    }
}