using myTNB.AndroidApp.Src.Base.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.SummaryDashBoard.Models
{
    public class SummaryDashBordRequest
    {
        [JsonProperty("accounts")]
        public List<string> AccNum { get; set; }

        [JsonProperty("usrInf")]
        public UserInterface usrInf { get; set; }
    }
}
