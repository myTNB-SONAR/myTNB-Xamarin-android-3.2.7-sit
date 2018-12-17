using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB_Android.Src.SummaryDashBoard.Models
{
    public class SummaryDashBordRequest
    {
        public SummaryDashBordRequest()
        {



        }


        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("SSPUserId")]
        public string SspUserId { get; set; }

        [JsonProperty("accounts")]
        public List<string> AccNum { get; set; }
    }
}
