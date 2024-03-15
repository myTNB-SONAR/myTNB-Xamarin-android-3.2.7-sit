using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.AppLaunch.Models
{
    public class AccountSMRStatusResponse
    {

        [JsonProperty("d")]
        public AccountsSMRStatusResponse Response { get; set; }

        public class AccountsSMRStatusResponse
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "ErrorCode")]
            public string ErrorCode { get; set; }

            [JsonProperty(PropertyName = "ErrorMessage")]
            public string ErrorMessage { get; set; }

            [JsonProperty(PropertyName = "DisplayMessage")]
            public string DisplayMessage { get; set; }

            [JsonProperty(PropertyName = "DisplayType")]
            public string DisplayType { get; set; }

            [JsonProperty(PropertyName = "DisplayTitle")]
            public string DisplayTitle { get; set; }

            [JsonProperty(PropertyName = "data")]
            public List<AccountSMRStatus> Data { get; set; }
        }
    }
}