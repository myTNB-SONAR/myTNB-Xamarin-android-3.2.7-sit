using Newtonsoft.Json;
using Refit;

namespace myTNB.Android.Src.Rating.Response
{
    public class SubmitRateUsResponse
    {
        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public SubmitRateUsResult submitRateUsResult { get; set; }

        public class SubmitRateUsResult
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

        }
    }
}