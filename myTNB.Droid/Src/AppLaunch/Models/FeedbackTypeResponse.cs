using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.Android.Src.AppLaunch.Models
{
    public class FeedbackTypeResponse
    {

        [JsonProperty("d")]
        public FeedbackTypeData Data { get; set; }

        public class FeedbackTypeData
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
            public List<FeedbackType> Data { get; set; }
        }
    }
}