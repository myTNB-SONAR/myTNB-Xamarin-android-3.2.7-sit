using Newtonsoft.Json;

namespace myTNB.Android.Src.Base.Models
{
    public class PreLoginFeedbackResponse
    {
        [JsonProperty("d")]
        public PreLoginFeedbackData Data { get; set; }

        public class PreLoginFeedbackData
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
            public PreLoginFeedback Data { get; set; }
        }
    }
}