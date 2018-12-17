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
using myTNB_Android.Src.MultipleAccountPayment.Model;

namespace myTNB_Android.Src.MultipleAccountPayment.Requests
{
    public class MPGetAccountDueAmountRequest : MPAccountDueRequest
    {

        public MPGetAccountDueAmountRequest(string apiKeyId, List<string> accounts) : base(apiKeyId, accounts)
        {
            base.apiKeyID = apiKeyID;
            base.accounts = accounts;
        }
    }
}