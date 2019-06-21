using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB_Android.Src.AddAccount.Models
{
    public class AccountResponseV5
    {
        [JsonProperty(PropertyName = "d")]
        public AccountResponseD D { get; set; }

        public class AccountResponseD
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "data")]
            public List<Account> AccountListData { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "isError")]
            public bool IsError { get; set; }

            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }
        }

    }
}