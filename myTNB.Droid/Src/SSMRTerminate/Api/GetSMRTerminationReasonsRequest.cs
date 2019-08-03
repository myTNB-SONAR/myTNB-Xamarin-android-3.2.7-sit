using myTNB_Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB_Android.Src.SSMRTerminate.Api
{
    public class GetSMRTerminationReasonsRequest
    {
        [JsonProperty("usrInf")]
        public UserInterface usrInf { get; set; }
    }
}