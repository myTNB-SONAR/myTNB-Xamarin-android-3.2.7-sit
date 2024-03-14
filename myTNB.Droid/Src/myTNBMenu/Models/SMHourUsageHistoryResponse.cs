using Newtonsoft.Json;
using Refit;

namespace myTNB.Android.Src.myTNBMenu.Models
{
    public class SMHourUsageHistoryResponse
    {
        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public UsageHistoryD Data { get; set; }

        public class UsageHistoryD
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
            public SMHourUsageHistoryData SMHourUsageHistoryData { get; set; }
        }

    }
}