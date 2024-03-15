using myTNB.AndroidApp.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.SSMRTerminate.Api
{
    public class GetRegisteredContactInfoRequest
    {
        [JsonProperty("contractAccount")]
        public string AccountNumber { get; set; }

        [JsonProperty("isOwnedAccount")]
        public string IsOwnedAccount { get; set; }

        [JsonProperty("ICNumber")]
        public string ICNumber { get; set; }

        [JsonProperty("usrInf")]
        public UserInterface usrInf { get; set; }
    }
}