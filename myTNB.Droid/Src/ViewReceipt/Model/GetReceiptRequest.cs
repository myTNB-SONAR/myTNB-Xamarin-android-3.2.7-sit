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
using Refit;

namespace myTNB_Android.Src.ViewReceipt.Model
{
    public class GetReceiptRequest
    {
        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }

        [AliasAs("merchant_transId")]
        public string merchant_transId { get; set; }

        public GetReceiptRequest(string apiKeyID, string merchant_transId)
        {
            this.apiKeyID = apiKeyID;
            this.merchant_transId = merchant_transId;
        }
    }
}