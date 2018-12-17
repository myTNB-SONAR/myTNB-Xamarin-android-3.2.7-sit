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
    public class REPaymentHistoryRequest
    {
        [JsonProperty("ApiKeyID")]
        [AliasAs("ApiKeyID")]
        public string ApiKeyID { get; set; }

        [JsonProperty("AccountNumber")]
        [AliasAs("AccountNumber")]
        public string AccountNum { get; set; }

        [JsonProperty("IsOwner")]
        [AliasAs("IsOwner")]
        public bool IsOwner { get; set; }

        [JsonProperty("Email")]
        [AliasAs("Email")]
        public string Email { get; set; }

        public REPaymentHistoryRequest()
        {
        }


    }
}