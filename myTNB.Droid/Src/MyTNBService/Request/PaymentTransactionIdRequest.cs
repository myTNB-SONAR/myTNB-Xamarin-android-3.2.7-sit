using System;
using System.Collections.Generic;
using myTNB;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.MyHome;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class PaymentTransactionIdRequest : BaseRequest
    {
        public DeviceInterface deviceInf { get; set; }
        public string customerName { get; set; }
        public string phoneNo { get; set; }
        public string platform { get; set; }
        public string registeredCardId { get; set; }
        public string paymentMode { get; set; }
        public string totalAmount { get; set; }
        public List<PaymentItem> paymentItems { get; set; }
        public string applicationType { get; set; } = string.Empty;
        public string applicationRefNo { get; set; } = string.Empty;
        public PaymentTransactionIdRequest(DeviceInterface deviceInf, string customerName, string phoneNo, string platform, string registeredCardId,
            string paymentMode, string totalAmount, List<PaymentItem> paymentItems, string applicationType, string applicationRefNo)
        {
            this.deviceInf = deviceInf;
            this.customerName = customerName;
            this.phoneNo = phoneNo;
            this.platform = platform;
            this.registeredCardId = registeredCardId;
            this.paymentMode = paymentMode;
            this.totalAmount = totalAmount.Replace(",", "");
            this.paymentItems = paymentItems;
            this.applicationType = MyHomeUtil.Instance.ApplicationType ?? null;
            this.applicationRefNo = MyHomeUtil.Instance.ReferenceNo ?? null;
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
            public bool myHomeEnabled { get; set; }
        }

        public class AccountPayment
        {
            public string PaymentType { get; set; }
            public string PaymentAmount { get; set; }
        }
    }
}
