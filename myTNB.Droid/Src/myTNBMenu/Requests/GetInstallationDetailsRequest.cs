using System;
using myTNB.AndroidApp.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.myTNBMenu.Requests
{
    public class GetInstallationDetailsRequest
    {
        [JsonProperty("contractAccount")]
        public string AccountNumber { get; set; }

        [JsonProperty("isOwner")]
        public string IsOwner { get; set; }

        [JsonProperty("usrInf")]
        public UserInterface usrInf { get; set; }
    }
}
