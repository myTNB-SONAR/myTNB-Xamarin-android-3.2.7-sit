using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.MultipleAccountPayment.Requests;
using Refit;
using System.Threading.Tasks;

namespace myTNB_Android.Src.MultipleAccountPayment.Api
{
    public interface MPRequestPaymentApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/RequestMultiPayBill")]
        Task<MPInitiatePaymentResponse> InitiatePayment([Body] MPInitiatePaymentRequestV3 requestPayment);
    }
}