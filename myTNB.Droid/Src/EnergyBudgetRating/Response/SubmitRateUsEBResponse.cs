using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.EnergyBudgetRating.Response
{
    public class SubmitRateUsEBResponse
    {
        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public SubmitRateUsEBResult submitRateUsEBResult { get; set; }

        public class SubmitRateUsEBResult
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