using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.MultipleAccountPayment.Model;

namespace myTNB_Android.Src.MultipleAccountPayment.Requests
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