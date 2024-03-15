using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.AppLaunch.Models
{
    public class FeedbackCategoryResponse
    {

        [JsonProperty("d")]
        public FeedbackCategoryData Data { get; set; }

        public class FeedbackCategoryData
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
            public List<FeedbackCategory> Data { get; set; }
        }
    }
}