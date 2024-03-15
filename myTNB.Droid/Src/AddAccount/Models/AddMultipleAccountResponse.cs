using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.AddAccount.Models
{
    public class AddMultipleAccountResponse
    {
        [JsonProperty(PropertyName = "d")]
        public Response response { get; set; }

        public class Response
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "data")]
            public List<AddAccount> Data { get; set; }


            [JsonProperty(PropertyName = "ErrorCode")]
            public string ErrorCode { get; set; }

            [JsonProperty(PropertyName = "DisplayMessage")]
            public string DisplayMessage { get; set; }
        }
    }
}