using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.Rating.Request;
using myTNB_Android.Src.Rating.Response;
using Refit;
using System;
using System.Collections.Generic;
using static myTNB_Android.Src.Rating.Request.SubmitRateUsRequest;

namespace myTNB_Android.Src.ServiceDistruptionRating.MVP
{
    public class ServiceDistruptionRatingContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {

            /// Show registration progress dialog
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            /// Hide registration progress dialog
            /// </summary>
            void HideProgressDialog();

            /// <summary>
            /// Shows an unknown exception with an option to retry
            /// </summary>
            /// <param name="exception">the returned exception</param>
            void ShowRetryOptionsUnknownException(Exception exception);

            /// <summary>
            /// Shows an unknown exception with an option to retry
            /// </summary>
            /// <param name="exception">the returned exception</param>
            void ShowError(string exception);

            ///<summary>
            /// Show questions success
            ///</summary>
            void ShowGetQuestionSuccess(GetRateUsQuestionResponse response);

            /// <summary>
            /// Enable register button
            /// </summary>
            void EnableShareButton();

            /// <summary>
            /// Disable register button
            /// </summary>
            void DisableShareButton();

            void ShowRetryOptionsApiException(ApiException apiException);

            void OnCallService();

            void OnBackPressed();
        }

        public interface IUserActionsListener : IBasePresenter
        {

            /*/// <summary>
            /// User actions to go back to previous screen
            /// </summary>
            void GetQuestions(string questtionCatId);

            /// <summary>
            /// Get rate us questions from api
            /// </summary>
            void GetRateUsQuestions(string questionCategoryID);


            ///<summary>
            ///Prepare submit rate us data
            ///</summary>
            void PrepareSubmitRateUsRequest(string referenceId, string deviceID, List<InputAnswerDetails> inputAnswerDetails);

            ///<summary>
            ///Submit rate us rating from the user input 
            /// </summary>
            void SubmitRateUs(SubmitRateUsRequest submitRateUsRequest);*/
        }

    }
}