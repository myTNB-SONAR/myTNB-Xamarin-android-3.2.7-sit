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
using myTNB_Android.Src.PaymentSuccessExperienceRating.Request;
using System.Threading.Tasks;
using myTNB_Android.Src.PaymentSuccessExperienceRating.Response;
using System.Threading;

namespace myTNB_Android.Src.PaymentSuccessExperienceRating.Api
{
    public interface SubmitExperienceRatingApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/SubmitExperienceRating")]
        Task<SubmitExperienceRatingResponse> SubmitRating([Body] SubmitExperienceRatingRequest request, CancellationToken cancellationToken);
    }
}