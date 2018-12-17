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
using myTNB_Android.Src.Base.Models;
using Refit;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Requests
{
    public class UsageHistoryRequest : BaseRequest
    {
        [JsonProperty("accNum")]
        [AliasAs("accNum")]
        public string AccountNum { get; set; }

        public UsageHistoryRequest(string apiKeyID) : base(apiKeyID)
        {

        }
    }
}