﻿using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.Request;
using Refit;
using System.Threading;
using System.Threading.Tasks;


namespace myTNB_Android.Src.Base.Api
{
    public interface IFeedbackApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetFeedbackCategory")]
        Task<FeedbackCategoryResponse> GetFeedbackCategory([Body] FeedbackCategoryRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetStatesForFeedback")]
        Task<FeedbackStateResponse> GetStatesForFeedback([Body] FeedbackStateRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetOtherFeedbackType")]
        Task<FeedbackTypeResponse> GetOtherFeedbackType([Body] FeedbackTypeRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetSubmittedFeedbackDetails")]
        Task<SubmittedFeedbackDetailsResponse> GetSubmittedFeedbackDetails([Body] SubmittedFeedbackDetailsRequest request, CancellationToken token);
    }
}