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
using myTNB_Android.Src.Rating.Request;
using myTNB_Android.Src.Rating.Response;
using System.Threading.Tasks;
using System.Threading;

namespace myTNB_Android.Src.Rating.Api
{
    public interface GetRateUsQuestionsApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetRateUsQuestions")]
        Task<GetRateUsQuestionsResponse> GetQuestions([Body] GetRateUsQuestionsRequest requestPayment, CancellationToken cancellationToken);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/SubmitRateUs")]
        Task<SubmitRateUsResponse> SubmitRateUs([Body] SubmitRateUsRequest requestPayment, CancellationToken cancellationToken);
    }
}