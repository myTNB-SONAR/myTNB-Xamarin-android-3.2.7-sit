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
using System.Threading;
using myTNB_Android.Src.myTNBMenu.Requests;
using myTNB_Android.Src.myTNBMenu.Models;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.Api
{
    public interface IBillsPaymentHistoryApi
    {
        
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetPaymentHistory")]
        Task<PaymentHistoryResponseV5> GetPaymentHistoryV5([Body] PaymentHistoryRequest paymentHistoryRequest, CancellationToken cancellationToken);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetBillHistory")]
        Task<BillHistoryResponseV5> GetBillHistoryV5([Body] BillHistoryRequest billHistoryRequest, CancellationToken cancellationToken);

        //For RE account payment history
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetREPaymentHistory")]
        Task<PaymentHistoryREResponse> GetREPaymentHistory([Body] REPaymentHistoryRequest paymentHistoryRequest, CancellationToken cancellationToken);

    }
}