using Newtonsoft.Json;

namespace myTNB.Android.Src.UpdatePassword.Models
{
    public class UpdatePasswordResponse
    {
        [JsonProperty("d")]
        public UpdatePasswordData Data { get; set; }

        public class UpdatePasswordData
        {
            [JsonProperty("__type")]
            public string Type { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("isError")]
            public bool IsError { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }
        }
    }
}