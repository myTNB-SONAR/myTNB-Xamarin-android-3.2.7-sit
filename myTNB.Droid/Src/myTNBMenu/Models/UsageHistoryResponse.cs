using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Models
{
    public class UsageHistoryResponse
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

            [JsonProperty(PropertyName = "RefreshTitle")]
            [AliasAs("RefreshTitle")]
            public string RefreshTitle { get; set; }

            [JsonProperty(PropertyName = "RefreshMessage")]
            [AliasAs("RefreshMessage")]
            public string RefreshMessage { get; set; }

            [JsonProperty(PropertyName = "RefreshBtnText")]
            [AliasAs("RefreshBtnText")]
            public string RefreshBtnText { get; set; }

            [JsonProperty(PropertyName = "isError")]
            [AliasAs("isError")]
            public bool IsError { get; set; }

            [JsonProperty(PropertyName = "message")]
            [AliasAs("message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "data")]
            [AliasAs("data")]
            public UsageHistoryData UsageHistoryData { get; set; }
        }

    }
}