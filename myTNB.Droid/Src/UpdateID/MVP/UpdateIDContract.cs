using Android.Content.PM;
using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.XDetailRegistrationForm.Models;
using Refit;
using System;

namespace myTNB.AndroidApp.Src.UpdateID.MVP
{
    public class UpdateIDContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            void ShowSuccessUpdateID();

            void ShowIdentificationHint();

            void ShowInvalidIdentificationError();

            /// <summary>
            /// Show empty IC no error
            /// Pre Validation
            /// </summary>
            void ShowEmptyICNoError();

            /// <summary>
            /// Show invalid IC no error
            /// Post Validation
            /// </summary>
            void ShowInvalidICNoError();

            /// <summary>
            /// Show progress
            /// </summary>
            void ShowProgress();


            /// <summary>
            /// Shows previous screen
            /// </summary>
            void ShowBackScreen();

            /// <summary>
            /// Hide progress
            /// </summary>
            void HideProgress();

            /// <summary>
            /// Clear all errors
            /// </summary>
            void ClearErrors();

            /// <summary>
            /// Clears all fields
            /// </summary>
            void ClearFields();

            /// <summary>
            /// Clears all error messages
            /// </summary>
            void ClearAllErrorFields();

            /// <summary>
            /// Enable register button
            /// </summary>
            void EnableRegisterButton();

            /// <summary>
            /// Disable register button
            /// </summary>
            void DisableRegisterButton();

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
            void ShowErrorMessage(string displayMessage);

            void ShowFullICError();

            void ShowFullArmyIdError();

            void ShowFullPassportError();

            void ShowProgressDialog();

            void HideProgressDialog();
            
            void callConfirm(string icno, string idtype);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// User action to register
            /// </summary>
            /// <param name="fullname">string fullname</param>
            /// <param name="icno">string icno</param>
            /// <param name="mobile_no">string mobile_no</param>
            /// <param name="email">string email</param>
            /// <param name="confirm_email">string confirm email</param>
            /// <param name="password">string password</param>
            /// <param name="confirm_password">string confirm password</param>
           // void OnAcquireToken(string fullname, string icno, string mobile_no, string email, string password);

            void CheckRequiredFields(string icno, string idtype);

            bool validateField(string icno, string idtype);

            /// <summary>
            /// The returned permission result
            /// </summary>
            /// <param name="requestCode">integer</param>
            /// <param name="permissions">string[] array</param>
            /// <param name="grantResults">Permission[] array</param>
            void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults);

            void OnUpdateIC(string no_ic, string idtype);

            void OnCheckID(string icno, string idtype);

            /// <summary>
            /// User actions to go back to previous screen
            /// </summary>
            void GoBack();
        }
    }
}