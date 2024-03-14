using Refit;

namespace myTNB.Android.Src.MultipleAccountPayment.Model
{
    public class MPSubmitPaymentRequest
    {
        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }

        [AliasAs("MERCHANT_TRANID")]
        public string MERCHANT_TRANID { get; set; }

        [AliasAs("accNum")]
        public string accNum { get; set; }

        [AliasAs("payAmount")]
        public string payAmount { get; set; }

        [AliasAs("custName")]
        public string custName { get; set; }

        [AliasAs("custEmail")]
        public string custEmail { get; set; }

        [AliasAs("custPhone")]
        public string custPhone { get; set; }

        [AliasAs("MPARAM1")]
        public string MPARAM1 { get; set; }

        [AliasAs("DESCRIPTION")]
        public string DESCRIPTION { get; set; }

        [AliasAs("cardNo")]
        public string cardNo { get; set; }

        [AliasAs("cardName")]
        public string cardName { get; set; }

        [AliasAs("expMonth")]
        public string expMonth { get; set; }

        [AliasAs("expYear")]
        public string expYear { get; set; }

        [AliasAs("cardCVV")]
        public string cvv { get; set; }

        //[AliasAs("Tokenize")]
        //public bool tokenize { get; set; }

        public MPSubmitPaymentRequest(string apiKeyID, string merchantId, string accNum, string payAm, string custName, string custEmail, string custPhone, string mParam1, string des, string cardNo, string cardName, string expMonth, string expYear, string cvv)
        {
            this.apiKeyID = apiKeyID;
            this.MERCHANT_TRANID = merchantId;
            this.accNum = accNum;
            this.payAmount = payAm;
            this.custName =
            this.custEmail = custEmail;
            this.custPhone = custPhone;
            this.MPARAM1 = mParam1;
            this.DESCRIPTION = des;
            this.cardNo = cardNo;
            this.cardName = cardName;
            this.expMonth = expMonth;
            this.expYear = expYear;
            this.cvv = cvv;
        }
    }
}
