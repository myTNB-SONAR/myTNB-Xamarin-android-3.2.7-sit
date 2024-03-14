using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.Android.Src.AddAccount.Models
{
    public class BCRMAccountResponse
    {
        [JsonProperty(PropertyName = "d")]
        public BCRMAccountData Data { get; set; }

        public class BCRMAccountData
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "data")]
            public List<BCRMAccount> BCRMAccountList { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "isError")]
            public bool IsError { get; set; }

            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }
        }
    }
}