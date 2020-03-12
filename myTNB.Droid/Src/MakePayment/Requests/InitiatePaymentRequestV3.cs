using myTNB_Android.Src.MakePayment.Models;

namespace myTNB_Android.Src.AddAccount.Requests
{
    public class InitiatePaymentRequestV3 : InitiatePaymentRequest
    {
        public InitiatePaymentRequestV3(string apiKeyId, string custName, string accNum, string payAm, string custEmail, string custPhone, string sspUserID, string platform, string paymentMode, string registeredCardId) : base(apiKeyId, custName, accNum, payAm, custEmail, custPhone, sspUserID, platform, paymentMode, registeredCardId)
        {
            base.apiKeyID = apiKeyID;
            base.customerName = custName;
            base.accNum = accNum;
            base.amount = payAm;
            base.email = custEmail;
            base.phoneNo = custPhone;
            base.sspUserId = sspUserID;
            base.platform = platform;
            base.paymentMode = paymentMode;
            base.registeredCardId = registeredCardId;
        }
    }
}