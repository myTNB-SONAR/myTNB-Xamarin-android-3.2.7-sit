using Newtonsoft.Json;
using Refit;

namespace myTNB.AndroidApp.Src.Login.Models
{
    public class UserResponse
    {
        [JsonProperty("d")]
        [AliasAs("d")]
        public UserData Data { get; set; }

        public class UserData
        {
            [JsonProperty("data")]
            [AliasAs("data")]
            public User User { get; set; }

            [JsonProperty(PropertyName = "__type")]
            [AliasAs("__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "isError")]
            [AliasAs("isError")]
            public bool IsError { get; set; }

            [JsonProperty(PropertyName = "message")]
            [AliasAs("message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "status")]
            [AliasAs("status")]
            public string Status { get; set; }
        }


    }
}