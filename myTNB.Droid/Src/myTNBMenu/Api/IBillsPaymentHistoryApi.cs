using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.Api
{
    public interface IBillsPaymentHistoryApi
    {

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetPaymentHistory")]
        Task<PaymentHistoryResponseV5> GetPaymentHistoryV5([Body] PaymentHistoryRequest paymentHistoryRequest, CancellationToken cancellationToken);

        //For RE account payment history
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetREPaymentHistory")]
        Task<PaymentHistoryREResponse> GetREPaymentHistory([Body] REPaymentHistoryRequest paymentHistoryRequest, CancellationToken cancellationToken);

    }
}