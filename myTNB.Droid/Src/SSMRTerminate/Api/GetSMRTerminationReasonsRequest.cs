using myTNB.Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB.Android.Src.SSMRTerminate.Api
{
    public class GetSMRTerminationReasonsRequest
    {
        [JsonProperty("usrInf")]
        public UserInterface usrInf { get; set; }
    }
}