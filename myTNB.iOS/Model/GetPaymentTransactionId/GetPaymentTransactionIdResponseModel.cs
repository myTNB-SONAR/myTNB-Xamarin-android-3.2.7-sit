using System.Collections.Generic;
using myTNB.Model;

namespace myTNB
{
    public class GetPaymentTransactionIdResponseModel
    {
        public GetPaymentTransactionIdModel d { set; get; } = new GetPaymentTransactionIdModel();
    }

    public class GetPaymentTransactionIdModel : BaseModelV2
    {
        public PaymentTransactionDataModel data { set; get; } = new PaymentTransactionDataModel();
    }

    public class PaymentTransactionDataModel
    {
        public string action { set; get; } = string.Empty;
        public string payMerchantID { set; get; } = string.Empty;
        public string payMerchant_transID { set; get; } = string.Empty;
        public string payCurrencyCode { set; get; } = string.Empty;
        public string payAmount { set; get; } = string.Empty;
        public string payCustName { set; get; } = string.Empty;
        public string payCustEmail { set; get; } = string.Empty;
        public string payCustPhoneNum { set; get; } = string.Empty;
        public string payProdDesc { set; get; } = string.Empty;
        public string payReturnUrl { set; get; } = string.Empty;
        public string paySign { set; get; } = string.Empty;
        public string payMParam { set; get; } = string.Empty;
        public string payMethod { set; get; } = string.Empty;
        public string platform { set; get; } = string.Empty;
        public string transactionType { set; get; } = string.Empty;
        public string tokenizedHashCodeCC { set; get; } = string.Empty;
        public List<PayAccountModel> payAccounts { set; get; } = new List<PayAccountModel>();
        public string Downtime { set; get; } = string.Empty;
        public string isError { set; get; } = string.Empty;
        public string message { set; get; } = string.Empty;
    }

    public class PayAccountModel
    {
        public string AccountOwnerName { set; get; } = string.Empty;
        public string AccountNo { set; get; } = string.Empty;
        public string Amount { set; get; } = string.Empty;
    }

    public class PayItemsModel
    {
        public string AccountOwnerName { set; get; } = string.Empty;
        public string AccountNo { set; get; } = string.Empty;
        public string AccountAmount { set; get; } = string.Empty;
    }

    public class PayMandatoryItems : PayItemsModel
    {
        public List<PaymentTypeModel> AccountPayments { set; get; } = new List<PaymentTypeModel>();
    }

    public class PaymentTypeModel
    {
        public string PaymentType { set; get; } = string.Empty;
        public string PaymentAmount { set; get; } = string.Empty;
    }
}