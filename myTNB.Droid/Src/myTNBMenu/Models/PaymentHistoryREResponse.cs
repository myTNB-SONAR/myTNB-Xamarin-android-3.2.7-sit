﻿using System;
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
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Models
{
    public class PaymentHistoryREResponse
    {
        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public PaymentHistoryREData Data { get; set; }

        public class PaymentHistoryREData
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
            public List<PaymentHistoryRE> PaymentHistoryRE { get; set; }
        }
    }
}