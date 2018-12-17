using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.MakePayment.Models;
using Refit;

namespace myTNB_Android.Src.MakePayment.Models
{
    public class InitiatePaymentResponse
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
            public InitiatePaymentResult initiatePaymentResult { get; set; }
        }

    }
}