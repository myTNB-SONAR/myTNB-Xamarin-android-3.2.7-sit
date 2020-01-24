using System.Collections.Generic;
using myTNB.Model;

namespace myTNB
{
    public class PendingPaymentResponseModel
    {
        public PaymentModelResponseDataModel d { set; get; }
    }

    public class PaymentModelResponseDataModel : BaseModelV2
    {
        public List<PendingPaymentDataModel> data { set; get; }
    }

    public class PendingPaymentDataModel
    {
        public string contractAccountNumber { set; get; }
        public bool HasPendingPayment { set; get; }
        public string PaymentAmount { set; get; }
        public string MerchantTransactionId { set; get; }
        public string CreatedDate { set; get; }
    }
}
