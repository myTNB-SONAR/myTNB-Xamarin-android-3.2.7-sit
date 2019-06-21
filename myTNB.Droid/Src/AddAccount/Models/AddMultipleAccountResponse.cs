using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB_Android.Src.AddAccount.Models
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

            [JsonProperty(PropertyName = "isError")]
            public bool IsError { get; set; }

            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "data")]
            public List<AddAccount> Data { get; set; }
        }
    }
}