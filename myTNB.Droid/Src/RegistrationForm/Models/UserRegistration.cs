using Newtonsoft.Json;
using Refit;
using System;

namespace myTNB.AndroidApp.Src.RegistrationForm.Models
{
    public class UserRegistration
    {
        [JsonProperty(PropertyName = "__type")]
        [AliasAs("__type")]
        public string __type { get; set; }

        [JsonProperty(PropertyName = "isError")]
        [AliasAs("isError")]
        public Boolean IsError { get; set; }

        [JsonProperty(PropertyName = "message")]
        [AliasAs("message")]
        public string Message { get; set; }

        public UserRegistration(string __type, bool isError, string message)
        {
            this.__type = __type ?? "";
            this.IsError = isError;
            this.Message = message ?? "";
        }
    }
}