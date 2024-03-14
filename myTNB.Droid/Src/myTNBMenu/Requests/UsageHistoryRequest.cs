using myTNB.Android.Src.Base.Models;
using Newtonsoft.Json;
using Refit;

namespace myTNB.Android.Src.myTNBMenu.Requests
{
    public class UsageHistoryRequest
    {
        [JsonProperty("contractAccount")]
        public string AccountNumber { get; set; }

        [JsonProperty("isOwner")]
        public string isOwner { get; set; }

        [JsonProperty("accountType")]
        public string accountType { get; set; }

        [JsonProperty("usrInf")]
        public UserInterface userInterface { get; set; }
    }
}