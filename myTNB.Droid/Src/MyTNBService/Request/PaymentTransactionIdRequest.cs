using System;
using System.Collections.Generic;
using myTNB_Android.Src.Base.Request;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class PaymentTransactionIdRequest : BaseRequest
    {
        public string customerName { get; set; }
        public string phoneNo { get; set; }
        public string platform { get; set; }
        public string registeredCardId { get; set; }
        public string paymentMode { get; set; }
        public string totalAmount { get; set; }
        public List<PaymentItem> paymentItems { get; set; }
        public PaymentTransactionIdRequest(string customerName, string phoneNo, string platform, string registeredCardId,
            string paymentMode, string totalAmount, List<PaymentItem> paymentItems)
        {
            this.customerName = customerName;
            this.phoneNo = phoneNo;
            this.platform = platform;
            this.registeredCardId = registeredCardId;
            this.paymentMode = paymentMode;
            this.totalAmount = totalAmount.Replace(",", "");
            this.paymentItems = paymentItems;
        }

        public class PaymentItemAccountPayment : PaymentItem
        {
            public List<AccountPayment> AccountPayments { get; set; }
        }

        public class PaymentItem
        {
            public string AccountOwnerName { get; set; }
            public string AccountNo { get; set; }
            public string AccountAmount { get; set; }
            public bool dbrEnabled { get; set; }
        }

        public class AccountPayment
        {
            public string PaymentType { get; set; }
            public string PaymentAmount { get; set; }
        }
    }
}
