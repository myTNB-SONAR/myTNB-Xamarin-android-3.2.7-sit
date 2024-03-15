using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.Base.Models
{
    public class SubmittedFeedbackDetailsResponse
    {
        [JsonProperty("d")]
        public SubmittedFeedbackDetailsData Data { get; set; }

        public class SubmittedFeedbackDetailsData
        {
            [JsonProperty("__type")]
            public string Type { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("isError")]
            public bool IsError { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("data")]
            public SubmittedFeedbackDetails Data { get; set; }
        }
    }
}