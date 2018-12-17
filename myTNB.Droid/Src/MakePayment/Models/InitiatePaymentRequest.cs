using System;
using Refit;

namespace myTNB_Android.Src.MakePayment.Models
{
    public class InitiatePaymentRequest
    {
        [AliasAs("apiKeyID")]         public string apiKeyID { get; set; }          [AliasAs("customerName")]         public string customerName { get; set; }          [AliasAs("accNum")]         public string accNum { get; set; }          [AliasAs("amount")]         public string amount { get; set; }          [AliasAs("email")]         public string email { get; set; }          [AliasAs("phoneNo")]         public string phoneNo { get; set; }          [AliasAs("sspUserId")]         public string sspUserId { get; set; }          [AliasAs("platform")]         public string platform { get; set; }          [AliasAs("paymentMode")]         public string paymentMode { get; set; }          [AliasAs("registeredCardId")]         public string registeredCardId { get; set; }          public InitiatePaymentRequest(string apiKeyID, string customerName, string accNum, string amount, string email, string phoneNo, string sspUserID, string platform, string paymentMode, string registeredCardId)         {             this.apiKeyID = apiKeyID;             this.customerName = customerName;             this.accNum = accNum;             this.amount = amount;             this.email = email;             this.phoneNo = phoneNo;             this.sspUserId = sspUserID;             this.platform = platform;             this.paymentMode = paymentMode;             this.registeredCardId = registeredCardId;         } 
    }
}
