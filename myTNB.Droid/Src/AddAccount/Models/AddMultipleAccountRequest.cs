using Refit;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.AddAccount.Models
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