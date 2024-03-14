using myTNB.Android.Src.Base.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.Android.Src.SummaryDashBoard.Models
{
    public class SummaryDashBordRequest
    {
        [JsonProperty("accounts")]
        public List<string> AccNum { get; set; }

        [JsonProperty("usrInf")]
        public UserInterface usrInf { get; set; }
    }
}
