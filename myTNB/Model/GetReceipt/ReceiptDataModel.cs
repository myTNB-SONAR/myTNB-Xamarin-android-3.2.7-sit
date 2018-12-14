using System.Collections.Generic;
using myTNB.Model.GetReceipt;

namespace myTNB.Model
{
    public class ReceiptDataModel
    {
        public string referenceNum { set; get; }
        public string accountNum { set; get; }
        public List<MultiPayDataModel> accMultiPay { set; get; }
        public string customerName { set; get; }
        public string customerEmail { set; get; }
        public string customerPhone { set; get; }
        public string payAmt { set; get; }
        public string payMethod { set; get; }
        public string payTransID { set; get; }
        public string paySellerNum { set; get; }
        public string payTransStatus { set; get; }
        public string payTransDate { set; get; }
        public string merchantID { set; get; }
        public string isError { set; get; }
        public string message { set; get; }
    }
}