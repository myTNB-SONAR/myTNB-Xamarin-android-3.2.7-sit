using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.LogoutRate.Models
{
    public class LogoutResponse
    {
        [JsonProperty("d")]
        public LogoutData Data { get; set; }

        public class LogoutData
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "isError")]
            public bool IsError { get; set; }

            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }
        }
    }
}