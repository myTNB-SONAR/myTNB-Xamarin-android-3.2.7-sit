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
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Requests
{
    public class BillHistoryRequest : BaseRequest
    {
        public BillHistoryRequest(string apiKeyID) : base(apiKeyID)
        {
        }

        [JsonProperty("accNum")]
        [AliasAs("accNum")]
        public string AccountNum { get; set; }

        [JsonProperty("isOwner")]
        [AliasAs("isOwner")]
        public bool IsOwner { get; set; }

        [JsonProperty("email")]
        [AliasAs("email")]
        public string Email { get; set; }

    }
}