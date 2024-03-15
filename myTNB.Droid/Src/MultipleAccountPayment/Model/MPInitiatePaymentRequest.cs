using Refit;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.MultipleAccountPayment.Model
{
    public class MPInitiatePaymentRequest
    {
        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }

        [AliasAs("customerName")]
        public string customerName { get; set; }

        [AliasAs("email")]
        public string email { get; set; }

        [AliasAs("phoneNo")]
        public string phoneNo { get; set; }

        [AliasAs("sspUserId")]
        public string sspUserId { get; set; }

        [AliasAs("platform")]
        public string platform { get; set; }

        [AliasAs("paymentMode")]
        public string paymentMode { get; set; }

        [AliasAs("totalAmount")]
        public string totalAmount { get; set; }

        [AliasAs("paymentItems")]
        public List<PaymentItems> paymentItems { get; set; }

        [AliasAs("registeredCardId")]
        public string registeredCardId { get; set; }

        public MPInitiatePaymentRequest(string apiKeyID, string customerName, string email, string phoneNo, string sspUserID, string platform, string registeredCardId, string paymentMode, string totalAmount, List<PaymentItems> paymentItems)
        {
            this.apiKeyID = apiKeyID;
            this.customerName = customerName;
            this.email = email;
            this.phoneNo = phoneNo;
            this.sspUserId = sspUserID;
            this.platform = platform;
            this.registeredCardId = registeredCardId;
            this.paymentMode = paymentMode;
            this.totalAmount = totalAmount;
            this.paymentItems = paymentItems;
        }
    }

    public class PaymentItems
    {
        [AliasAs("AccountNo")]
        public string AccountNo { get; set; }

        [AliasAs("Amount")]
        public string Amount { get; set; }

        [AliasAs("AccountOwnerName")]
        public string AccountOwnerName { get; set; }
    }
}
