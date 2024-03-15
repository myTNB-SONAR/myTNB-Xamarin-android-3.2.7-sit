using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.ManageCards.Models
{
    public class RemoveRegisteredCardResponse
    {
        [JsonProperty(PropertyName = "d")]
        public RemoveRegisteredCardData Data { get; set; }

        public class RemoveRegisteredCardData
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