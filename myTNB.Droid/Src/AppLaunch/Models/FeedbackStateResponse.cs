using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.Android.Src.AppLaunch.Models
{
    public class FeedbackStateResponse
    {
        [JsonProperty("d")]
        public FeedbackStateData Data { get; set; }

        public class FeedbackStateData
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
            public List<FeedbackState> Data { get; set; }
        }
    }
}