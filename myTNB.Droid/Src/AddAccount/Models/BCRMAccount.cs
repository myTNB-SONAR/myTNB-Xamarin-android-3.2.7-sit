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
using Refit;

namespace myTNB_Android.Src.AddAccount.Models
{
    public class BCRMAccount
    {
        [JsonProperty(PropertyName = "accNum")]
        [AliasAs("accNum")]
        public string accNum { get; set; }

        [JsonProperty(PropertyName = "accountTypeId")]
        [AliasAs("accountTypeId")]
        public string accountTypeId { get; set; }

        [JsonProperty(PropertyName = "accountStAddress")]
        [AliasAs("accountStAddress")]
        public string accountStAddress { get; set; }

        [JsonProperty(PropertyName = "icNum")]
        [AliasAs("icNum")]
        public string icNum { get; set; }

        [JsonProperty(PropertyName = "isOwned")]
        [AliasAs("isOwned")]
        public bool isOwned { get; set; }

        [JsonProperty(PropertyName = "accountCategoryId")]
        [AliasAs("accountCategoryId")]
        public string accountCategoryId { get; set; }
    }
}