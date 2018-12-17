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

namespace myTNB_Android.Src.AddAccount.Models
{
    public class AccountsResquest
    {
        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }

        [AliasAs("userID")]
        public string userID { get; set; }

        public AccountsResquest(string apiKeyID, string userID)
        {
            this.apiKeyID = apiKeyID;
            this.userID = userID;
        }
    }
}