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
using myTNB_Android.Src.AddAccount.Models;

namespace myTNB_Android.Src.AddAccount.Requests
{
    public class GetCustomerAccountsRequest : AccountsResquest
    {
        public GetCustomerAccountsRequest(string apiKeyID, string userID) : base(apiKeyID, userID)
        {
            base.apiKeyID = apiKeyID;
            base.userID = userID;
        }
    }
}