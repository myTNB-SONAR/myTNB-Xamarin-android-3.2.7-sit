﻿using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.Rating.Request;
using System;
using System.Collections.Generic;
using static myTNB.AndroidApp.Src.Rating.Request.SubmitRateUsRequest;

namespace myTNB.AndroidApp.Src.Rating.MVP
{
    public class SubmitRatingContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {

            /// <summary>
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
            void EnableSubmitButton();

            /// <summary>
            /// Disable register button
            /// </summary>
            void DisableSubmitButton();


            ///<summary>
            ///Show thank you screen on successful submit rate us
            ///</summary>
            void ShowSumitRateUsSuccess();
        }

        public interface IUserActionsListener : IBasePresenter
        {

            /// <summary>
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
            void SubmitRateUs(SubmitRateUsRequest submitRateUsRequest);
        }

    }
}