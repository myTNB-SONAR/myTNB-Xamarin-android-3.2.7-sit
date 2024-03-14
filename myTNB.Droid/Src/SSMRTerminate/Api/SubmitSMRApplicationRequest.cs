using myTNB.Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB.Android.Src.SSMRTerminate.Api
{
    public class SubmitSMRApplicationRequest
    {
        [JsonProperty("contractAccount")]
        public string AccountNumber { get; set; }

        [JsonProperty("oldPhone")]
        public string OldPhone { get; set; }

        [JsonProperty("newPhone")]
        public string NewPhone { get; set; }

        [JsonProperty("oldEmail")]
        public string OldEmail { get; set; }

        [JsonProperty("newEmail")]
        public string NewEmail { get; set; }

        [JsonProperty("SMRMode")]
        public string SMRMode { get; set; }

        [JsonProperty("reason")]
        public string TerminationReason { get; set; }

        [JsonProperty("usrInf")]
        public UserInterface usrInf { get; set; }
    }
}