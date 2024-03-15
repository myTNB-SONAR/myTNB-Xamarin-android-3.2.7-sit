using Refit;

namespace myTNB.AndroidApp.Src.ViewReceipt.Model
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