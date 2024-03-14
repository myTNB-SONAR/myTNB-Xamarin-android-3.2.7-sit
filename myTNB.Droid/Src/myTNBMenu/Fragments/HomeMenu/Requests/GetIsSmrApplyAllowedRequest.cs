using System.Collections.Generic;
using myTNB.Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.Requests
{
    public class GetIsSmrApplyAllowedRequest
    {
        [JsonProperty("usrInf")]
        public UserInterface usrInf { get; set; }

        [JsonProperty("contractAccounts")]
        public List<string> contractAccounts { get; set; }
    }
}