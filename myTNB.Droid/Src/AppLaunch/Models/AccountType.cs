using Newtonsoft.Json;
using Refit;
using System;

namespace myTNB.AndroidApp.Src.AppLaunch.Models
{
    public class AccountType
    {
        [JsonProperty(PropertyName = "__type")]
        [AliasAs("__type")]
        public string __type { get; set; }

        [JsonProperty(PropertyName = "AccountType")]
        [AliasAs("AccountType")]
        public int accountType { get; set; }

        [JsonProperty(PropertyName = "AccountTypeName")]
        [AliasAs("AccountTypeName")]
        public string accountTypeName { get; set; }

        [JsonProperty(PropertyName = "isError")]
        [AliasAs("isError")]
        public Boolean isErrror { get; set; }

        [JsonProperty(PropertyName = "message")]
        [AliasAs("message")]
        public string message { get; set; }

        public AccountType(string type, int accountType, string accountTypeName, bool isErrror, string message)
        {
            __type = type;
            this.accountType = accountType;
            this.accountTypeName = accountTypeName;
            this.isErrror = isErrror;
            this.message = message;
        }

        public override string ToString()
        {
            return __type + " " + accountType + " " + accountTypeName + " " + isErrror + " " + message;
        }
    }
}