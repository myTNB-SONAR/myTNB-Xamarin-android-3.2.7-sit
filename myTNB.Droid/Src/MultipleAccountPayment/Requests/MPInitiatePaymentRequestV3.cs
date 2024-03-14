using myTNB.Android.Src.MultipleAccountPayment.Model;
using System.Collections.Generic;

namespace myTNB.Android.Src.MultipleAccountPayment.Requests
{
    public class MPInitiatePaymentRequestV3 : MPInitiatePaymentRequest
    {
        public MPInitiatePaymentRequestV3(string apiKeyId, string custName, string custEmail, string custPhone, string sspUserID, string platform, string registeredCardId, string paymentMode, string totalAmount, List<PaymentItems> paymentItems) : base(apiKeyId, custName, custEmail, custPhone, sspUserID, platform, registeredCardId, paymentMode, totalAmount, paymentItems)
        {
            base.apiKeyID = apiKeyID;
            base.customerName = custName;
            base.email = custEmail;
            base.phoneNo = custPhone;
            base.sspUserId = sspUserID;
            base.platform = platform;
            base.registeredCardId = registeredCardId;
            base.paymentMode = paymentMode;
            base.totalAmount = totalAmount;
            base.paymentItems = paymentItems;
        }
    }
}