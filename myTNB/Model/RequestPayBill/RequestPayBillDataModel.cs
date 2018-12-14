using System.Collections.Generic;

namespace myTNB.Model
{
    public class RequestPayBillDataModel
    {
        public string action { set; get; }
        public string payMerchantID { set; get; }
        public string payMerchant_transID { set; get; }
        public string payCurrencyCode { set; get; }
        public string payAmount { set; get; }
        public string payCustName { set; get; }
        public string payCustEmail { set; get; }
        public string payCustPhoneNum { set; get; }
        public string payProdDesc { set; get; }
        public string payReturnUrl { set; get; }
        public string paySign { set; get; }
        public string payMParam { set; get; }
        public string payMethod { set; get; }
        public string platform { set; get; }
        public string transactionType { set; get; }
        public string tokenizedHashCodeCC { set; get; }
        public string isError { set; get; }
        public string message { set; get; }
        public List<PayAccountDataModel> payAccounts { set; get; }
    }
}