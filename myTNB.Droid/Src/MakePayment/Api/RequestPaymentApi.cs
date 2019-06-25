using myTNB_Android.Src.AddAccount.Requests;
using myTNB_Android.Src.MakePayment.Models;
using Refit;
using System.Threading.Tasks;

namespace myTNB_Android.Src.MakePayment.Api
{
    public interface RequestPaymentApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/RequestPayBill")]
        Task<InitiatePaymentResponse> InitiatePayment([Body] InitiatePaymentRequestV3 requestPayment);
    }
}