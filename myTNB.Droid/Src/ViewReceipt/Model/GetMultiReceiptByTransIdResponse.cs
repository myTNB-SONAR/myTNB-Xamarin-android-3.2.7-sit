using Newtonsoft.Json;
using Refit;

namespace myTNB.AndroidApp.Src.ViewReceipt.Model
{
    public class GetMultiReceiptByTransIdResponse
    {
        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public Receipt receipt { get; set; }

        public class Receipt
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
            public MultiReceiptDetails receiptDetails { get; set; }
        }

    }
}