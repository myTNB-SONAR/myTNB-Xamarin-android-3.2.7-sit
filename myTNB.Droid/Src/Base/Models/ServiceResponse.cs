using Newtonsoft.Json;
using Refit;
using System;

namespace myTNB.AndroidApp.Src.Base.Models
{
    public class ServiceResponse
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

        public ServiceResponse(string __type, string status, bool isError, string message)
        {
            this.__type = __type;
            this.status = status;
            this.isError = isError;
            this.message = message;
        }
    }
}