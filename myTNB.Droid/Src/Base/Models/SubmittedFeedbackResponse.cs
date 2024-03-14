using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.Android.Src.Base.Models
{
    public class SubmittedFeedbackResponse
    {
        [JsonProperty("d")]
        public SubmittedFeedbackData Data { get; set; }

        public class SubmittedFeedbackData
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
            public List<SubmittedFeedback> Data { get; set; }
        }
    }
}