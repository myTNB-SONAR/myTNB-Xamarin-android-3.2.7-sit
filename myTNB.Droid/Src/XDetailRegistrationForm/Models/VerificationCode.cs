using Newtonsoft.Json;
using Refit;
using System;

namespace myTNB_Android.Src.XDetailRegistrationForm.Activity.Models
{
    public class VerificationCode
    {
        [JsonProperty(PropertyName = "__type")]
        [AliasAs("__type")]
        public string __type { get; set; }

        [JsonProperty(PropertyName = "status")]
        [AliasAs("status")]
        public string status { get; set; }

        [JsonProperty(PropertyName = "isError")]
        [AliasAs("isError")]
        public Boolean isError { get; set; }

        [JsonProperty(PropertyName = "message")]
        [AliasAs("message")]
        public string message { get; set; }

        public VerificationCode(string __type, string status, bool isError, string message)
        {
            this.__type = __type;
            this.status = status;
            this.isError = isError;
            this.message = message;
        }
    }
}