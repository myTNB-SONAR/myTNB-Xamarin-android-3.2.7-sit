using Newtonsoft.Json;
using Refit;

namespace myTNB.Android.Src.ForgetPassword.Models
{
    public class ForgetPassword
    {
        [JsonProperty(PropertyName = "__type")]
        [AliasAs("__type")]
        public string __type { get; set; }

        [JsonProperty(PropertyName = "status")]
        [AliasAs("status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "isError")]
        [AliasAs("isError")]
        public bool IsError { get; set; }

        [JsonProperty(PropertyName = "message")]
        [AliasAs("message")]
        public string Message { get; set; }

        public ForgetPassword(string __type, string status, bool isError, string message)
        {
            this.__type = __type ?? "";
            Status = status ?? "";
            IsError = isError;
            Message = message ?? "";
        }
    }
}