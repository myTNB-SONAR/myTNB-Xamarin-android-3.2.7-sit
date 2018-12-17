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
    public class SMUsageHistoryRequest : BaseRequest
    {
        [JsonProperty("accNum")]
        [AliasAs("accNum")]
        public string AccountNum { get; set; }

        [JsonProperty("userEmail")]
        [AliasAs("userEmail")]
        public string userEmail { get; set; }

        [JsonProperty("sspUserId")]
        [AliasAs("sspUserId")]
        public string sspUserId { get; set; }

        [JsonProperty("isOwner")]
        [AliasAs("isOwner")]
        public bool isOwner { get; set; }


        [JsonProperty("metercode")]
        [AliasAs("metercode")]
        public string MeterCode { get; set; }

        public SMUsageHistoryRequest(string apiKeyID) : base(apiKeyID)
        {

        }
    }
}