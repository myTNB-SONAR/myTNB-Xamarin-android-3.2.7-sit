using myTNB_Android.Src.Rating.Request;
using myTNB_Android.Src.Rating.Response;
using Refit;
using System.Threading;
using System.Threading.Tasks;

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