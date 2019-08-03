using myTNB_Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB_Android.Src.SSMRTerminate.Api
{
    public class GetRegisteredContactInfoRequest
    {
        [JsonProperty("contractAccount")]
        public string AccountNumber { get; set; }

        [JsonProperty("isOwnedAccount")]
        public string IsOwnedAccount { get; set; }

        [JsonProperty("usrInf")]
        public UserInterface usrInf { get; set; }
    }
}