using System.Collections.Generic;
using myTNB.AndroidApp.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.AppLaunch.Requests
{
    public class AccountsSMRStatusRequest
    {
        [JsonProperty("contractAccounts")]
        public List<string> ContractAccounts { get; set; }

        [JsonProperty("usrInf")]
        public UserInterface UserInterface { get; set; }
    }
}