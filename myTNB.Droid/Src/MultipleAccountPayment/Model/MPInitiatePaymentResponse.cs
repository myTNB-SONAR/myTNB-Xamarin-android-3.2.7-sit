﻿using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.MultipleAccountPayment.Model
{
    public class MPInitiatePaymentResponse
    {

        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public RequestPayBill requestPayBill { get; set; }

        public class RequestPayBill
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
            public MPInitiatePaymentResult initiatePaymentResult { get; set; }
        }

    }
}