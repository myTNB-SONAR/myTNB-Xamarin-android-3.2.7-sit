using Refit;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.MultipleAccountPayment.Model
{
    public class MPAccountDueRequest
    {
        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }

        [AliasAs("accounts")]
        public List<string> accounts { get; set; }


        public MPAccountDueRequest(string apiKeyID, List<string> accounts)
        {
            this.apiKeyID = apiKeyID;
            this.accounts = accounts;
        }

    }
}
