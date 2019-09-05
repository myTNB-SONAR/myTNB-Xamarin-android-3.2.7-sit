using System.Collections.Generic;
using myTNB.Model;

namespace myTNB
{
    public class GetPaymentReceiptResponseModel
    {
        public GetPaymentReceiptDataModel d { set; get; } = new GetPaymentReceiptDataModel();
    }
    public class GetPaymentReceiptDataModel : BaseModelV2
    {
        public ReceiptDataModel data { set; get; } = new ReceiptDataModel();
    }

    public class ReceiptDataModel
    {
        public string referenceNum { set; get; }
        public List<MultiPayDataModel> accMultiPay { set; get; }
        public string customerName { set; get; }
        public string customerEmail { set; get; }
        public string customerPhone { set; get; }
        public string payMethod { set; get; }
        public string payTransID { set; get; }
        public string paySellerNum { set; get; }
        public string payTransStatus { set; get; }
        public string payTransDate { set; get; }
        public string merchantID { set; get; }
        public string payAmt { set; get; }
    }

    public class MultiPayDataModel
    {
        public string AccountOwnerName { set; get; }
        public string accountNum { set; get; }
        public string itmAmt { set; get; }
    }
}