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
    public class AddMultipleAccountRequest
    {
        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }

        [AliasAs("sspUserId")]
        public string sspUserId { get; set; }

        [AliasAs("email")]
        public string email { get; set; }

        [AliasAs("billAccounts")]
        public List<AddAccount> billAccounts { get; set; }

        public AddMultipleAccountRequest(string apiKeyID, string sspUserId, string email, List<AddAccount> accounts)
        {
            this.apiKeyID = apiKeyID;
            this.sspUserId = sspUserId;
            this.billAccounts = accounts;
            this.email = email;
        }
    }
}