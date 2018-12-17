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
using Refit;
using Android.Accounts;
using System.Threading.Tasks;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.MultipleAccountPayment.Requests;

namespace myTNB_Android.Src.MultipleAccountPayment.Api
{
    public interface MPRequestPaymentApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/RequestMultiPayBill")] 
        Task<MPInitiatePaymentResponse> InitiatePayment([Body] MPInitiatePaymentRequestV3 requestPayment);
    }
}