using myTNB.AndroidApp.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.SSMRTerminate.Api
{
    public class GetSMRTerminationReasonsRequest
    {
        [JsonProperty("usrInf")]
        public UserInterface usrInf { get; set; }
    }
}