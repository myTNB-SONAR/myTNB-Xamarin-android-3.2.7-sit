using myTNB_Android.Src.MakePayment.Models;

namespace myTNB_Android.Src.MakePayment.Requests
{
    public class SubmitPaymentRequestPG : SubmitPaymentRequest
    {
        public SubmitPaymentRequestPG(string apiKeyID, string merchantId, string accNum, string payAm, string custName, string custEmail, string custPhone, string mParam1, string des, string cardNo, string cardName, string expM, string expY, string cvv) : base(apiKeyID, merchantId, accNum, payAm, custName, custEmail, custPhone, mParam1, des, cardNo, cardName, expM, expY, cvv)
        {
            base.apiKeyID = apiKeyID;
            base.MERCHANT_TRANID = merchantId;
            base.accNum = accNum;
            base.payAmount = payAm;
            base.custName = custName;
            base.custEmail = custEmail;
            base.custPhone = custPhone;
            base.MPARAM1 = mParam1;
            base.DESCRIPTION = des;
        }
    }
}
