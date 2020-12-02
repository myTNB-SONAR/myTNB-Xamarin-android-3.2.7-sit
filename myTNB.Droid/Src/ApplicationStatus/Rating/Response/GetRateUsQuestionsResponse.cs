using myTNB_Android.Src.Rating.Model;
using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;

namespace myTNB_Android.Src.ApplicationStatusRating.Response
{
    public class GetRateUsQuestionsResponse
    {
        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public FeedbackQuestionStatus feedbackQuestionStatus { get; set; }

        public class FeedbackQuestionStatus
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "status")]
            [AliasAs("status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "isError")]
            [AliasAs("isError")]
            public bool IsError { get; set; }

            [JsonProperty(PropertyName = "message")]
            [AliasAs("message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "data")]
            [AliasAs("data")]
            public List<RateUsQuestion> rateUsQuestionList { get; set; }
        }
    }
}