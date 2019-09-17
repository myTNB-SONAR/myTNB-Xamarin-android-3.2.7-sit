using Refit;

namespace myTNB_Android.Src.ViewReceipt.Model
{
    public class GetReceiptRequest
    {
        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }

        [AliasAs("merchant_transId")]
        public string merchant_transId { get; set; }

        [AliasAs("contractAccount")]
        public string contractAccount { get; set; }

        [AliasAs("email")]
        public string email { get; set; }

        public GetReceiptRequest(string apiKeyID, string merchant_transId, string contractAccount, string email)
        {
            this.apiKeyID = apiKeyID;
            this.merchant_transId = merchant_transId;
            this.contractAccount = contractAccount;
            this.email = email;
        }
    }
}
