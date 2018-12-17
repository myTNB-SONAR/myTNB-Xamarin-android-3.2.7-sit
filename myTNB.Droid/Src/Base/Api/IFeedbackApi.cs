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
using System.Threading.Tasks;
using System.Threading;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;


namespace myTNB_Android.Src.Base.Api
{
    public interface IFeedbackApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/SubmitFeedback")]
        Task<PreLoginFeedbackResponse> SubmitFeedback([Body] FeedbackRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetFeedbackCategory")]
        Task<FeedbackCategoryResponse> GetFeedbackCategory([Body] FeedbackCategoryRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetStatesForFeedback")]
        Task<FeedbackStateResponse> GetStatesForFeedback([Body] FeedbackStateRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetSubmittedFeedbackList")]
        Task<SubmittedFeedbackResponse> GetSubmittedFeedbackList([Body] SubmittedFeedbackRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetOtherFeedbackType")]
        Task<FeedbackTypeResponse> GetOtherFeedbackType([Body] FeedbackTypeRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetSubmittedFeedbackDetails")]
        Task<SubmittedFeedbackDetailsResponse> GetSubmittedFeedbackDetails([Body] SubmittedFeedbackDetailsRequest request, CancellationToken token);
    }
}