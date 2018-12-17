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

namespace myTNB_Android.Src.MultipleAccountPayment.Model
{
    public class MPAccountDueResponse
    {

        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public AccountDueAmountResponse accountDueAmountResponse { get; set; }

        public class AccountDueAmountResponse
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
            public List<Account> accounts { get; set; }
        }

        public class Account
        {           
            [JsonProperty(PropertyName = "amountDue")]
            [AliasAs("amountDue")]
            public double amountDue { get; set; }

            [JsonProperty(PropertyName = "billDueDate")]
            [AliasAs("billDueDate")]
            public string billDueDate { get; set; }

            [JsonProperty(PropertyName = "accNum")]
            [AliasAs("accNum")]
            public string accNum { get; set; }
        }
    }
}