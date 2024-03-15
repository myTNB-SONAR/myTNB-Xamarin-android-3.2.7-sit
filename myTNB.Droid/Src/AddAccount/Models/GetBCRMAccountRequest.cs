using Refit;

namespace myTNB.AndroidApp.Src.AddAccount.Models
{
    public class GetBCRMAccountRequest
    {
        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }

        [AliasAs("currentLinkedAccounts")]
        public string currentLinkedAccounts { get; set; }

        [AliasAs("email")]
        public string email { get; set; }

        [AliasAs("identificationNo")]
        public string identificationNo { get; set; }


        public GetBCRMAccountRequest(string apiKeyID, string currentLinkedAccounts, string email, string identificationNo)
        {
            this.apiKeyID = apiKeyID;
            this.currentLinkedAccounts = currentLinkedAccounts;
            this.email = email;
            this.identificationNo = identificationNo;
        }
    }
}