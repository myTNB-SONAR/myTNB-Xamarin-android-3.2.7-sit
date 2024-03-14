using Newtonsoft.Json;

namespace myTNB.Android.Src.UpdateNickname.Models
{
    public class UpdateLinkedAccountNickNameResponse
    {

        [JsonProperty("d")]
        public UpdateLinkedAccountNickNameData Data { get; set; }

        public class UpdateLinkedAccountNickNameData
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