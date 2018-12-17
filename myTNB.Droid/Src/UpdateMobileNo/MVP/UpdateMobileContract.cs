﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.MVP;
using Refit;
using myTNB_Android.Src.Login.Requests;

namespace myTNB_Android.Src.UpdateMobileNo.MVP
{
    public class UpdateMobileContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show invalid mobile error
            /// </summary>
            void ShowInvalidMobileNoError();

            /// <summary>
            /// Clear all errors
            /// </summary>
            void ClearErrors();

            /// <summary>
            /// Show success
            /// </summary>
            void ShowSuccess(string newPhoneNo);

            /// <summary>
            /// Show error message from api response
            /// </summary>
            /// <param name="message">string</param>
            void ShowErrorMessage(string message);

            /// <summary>
            /// Show progress
            /// </summary>
            void ShowProgress();

            /// <summary>
            /// Hide progress
            /// </summary>
            void HideProgress();

            /// <summary>
            /// Enable save button
            /// </summary>
            void EnableSaveButton();

            /// <summary>
            /// Disable save button
            /// </summary>
            void DisableSaveButton();

            /// <summary>
            /// Show mobile no
            /// </summary>
            /// <param name="mobileNo">string</param>
            void ShowMobile(string mobileNo);

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

        }
        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to save new phone number
            /// </summary>
            /// <param name="newPhoneNumber">string</param>
            void OnSave(string newPhoneNumber, UserAuthenticationRequest request);

            /// <summary>
            /// Action to verify mobile no
            /// </summary>
            /// <param name="mobileNo">string</param>
            void OnVerifyMobile(string mobileNo, bool isForceUpdate);

            /// <summary>
            /// V2 service to generate sms token
            /// to verfiy mobile number
            /// </summary>
            void OnUpdatePhoneNo(string newPhoneNumber, UserAuthenticationRequest request);
        }
    }
}