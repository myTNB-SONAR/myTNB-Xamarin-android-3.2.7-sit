using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;

namespace myTNB.Android.Src.myTNBMenu.Models
{
    public class BillHistoryResponseV5
    {

        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public BillHistoryData Data { get; set; }

        public class BillHistoryData
        {
            [JsonProperty(PropertyName = "__type")]
            [AliasAs("__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "status")]
            [AliasAs("status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "")]
            [AliasAs("isError")]
            public bool IsError { get; set; }

            [JsonProperty(PropertyName = "message")]
            [AliasAs("message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "data")]
            [AliasAs("data")]
            public List<BillHistoryV5> BillHistory { get; set; }
        }
    }
}