using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace myTNB.Android.Src.ViewBill.Model
{
    public class GetBillMaskingResponse
    {
        [JsonProperty("d")]
        public BillMaskingStatusResponse Response { get; set; }

        public class BillMaskingStatusResponse
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "ErrorCode")]
            public string ErrorCode { get; set; }

            [JsonProperty(PropertyName = "ErrorMessage")]
            public string ErrorMessage { get; set; }

            [JsonProperty(PropertyName = "DisplayMessage")]
            public string DisplayMessage { get; set; }

            [JsonProperty(PropertyName = "DisplayType")]
            public string DisplayType { get; set; }

            [JsonProperty(PropertyName = "DisplayTitle")]
            public string DisplayTitle { get; set; }

            [JsonProperty(PropertyName = "binaryBill")]
            public byte[] binaryBill { get; set; }
        }
    }
}

