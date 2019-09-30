using myTNB_Android.Src.Base.Models;
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Requests
{
    public class UsageHistoryRequest
    {
        [JsonProperty("contractAccount")]
        public string AccountNumber { get; set; }

        [JsonProperty("isOwner")]
        public string isOwner { get; set; }

        [JsonProperty("usrInf")]
        public UserInterface userInterface { get; set; }
    }
}