using myTNB_Android.Src.PaymentSuccessExperienceRating.Request;
using myTNB_Android.Src.PaymentSuccessExperienceRating.Response;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.PaymentSuccessExperienceRating.Api
{
    public interface SubmitExperienceRatingApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/SubmitExperienceRating")]
        Task<SubmitExperienceRatingResponse> SubmitRating([Body] SubmitExperienceRatingRequest request, CancellationToken cancellationToken);
    }
}