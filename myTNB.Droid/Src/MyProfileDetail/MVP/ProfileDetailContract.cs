﻿using System;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.ManageCards.Models;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Models;
using Refit;

namespace myTNB.AndroidApp.Src.MyProfileDetail.MVP
{
    public class ProfileDetailContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Shows a snackbar after success resend email verification
            /// </summary>
            void ShowEmailUpdateSuccess(string message);

            /// <summary>
            /// Shows error message
            /// </summary>
            /// <param name="errorMessage"></param>
            void ShowError(string errorMessage);

            /// <summary>
            /// Shows a cancelled exception with an option to retry
            /// </summary>
            /// <param name="operationCanceledException">the returned exception</param>
            void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException);

            /// <summary>
            /// Shows an api exception with an option to retry
            /// </summary>
            /// <param name="apiException">the returned exception</param>
            void ShowRetryOptionsApiException(ApiException apiException);


            /// <summary>
            /// Shows an unknown exception with an option to retry
            /// </summary>
            /// <param name="exception">the returned exception</param>
            void ShowRetryOptionsUnknownException(Exception exception);

            /// <summary>
            /// Show get code progress dialog
            /// </summary>
            void ShowGetCodeProgressDialog();

            /// <summary>
            /// Hide get code progress dialog
            /// </summary>
            void HideGetCodeProgressDialog();

            /// <summary>
            /// Shows an cancelled exception with an option to retry
            /// </summary>
            /// <param name="operationCanceledException"></param>
            void ShowRetryOptionsCodeCancelledException(System.OperationCanceledException operationCanceledException);

            /// <summary>
            /// Shows an api exception with an option to retry
            /// </summary>
            /// <param name="apiException"></param>
            void ShowRetryOptionsCodeApiException(ApiException apiException);

            /// <summary>
            /// Shows an unknown exception with an option to retry
            /// </summary>
            /// <param name="exception"></param>
            void ShowRetryOptionsCodeUnknownException(Exception exception);

            /// <summary>
            /// Determines if Account Verifed banner is to be shown or not
            /// </summary>
            /// <param name="isVerified"></param>
            void ShowAccountVerified(bool isVerified);
            
            /// <summary>
            /// Reload the page
            /// </summary>
            void ReloadPage();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="email"></param>
            void ResendEmailVerify(string apiKeyId, string email);

            /// <summary>
            /// Triggers API Call to get Account Verified status
            /// </summary>
            void GetEKYCStatusOnCall();
            
            void GetID(); 
        }
    }
}
