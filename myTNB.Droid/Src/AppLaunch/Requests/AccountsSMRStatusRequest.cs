using System.Collections.Generic;
using myTNB.Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB.Android.Src.AppLaunch.Requests
{
    public class AccountsSMRStatusRequest
    {
        [JsonProperty("contractAccounts")]
        public List<string> ContractAccounts { get; set; }

        [JsonProperty("usrInf")]
        public UserInterface UserInterface { get; set; }
    }
}