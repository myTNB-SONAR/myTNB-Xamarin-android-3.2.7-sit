using myTNB.AndroidApp.Src.Base.MVP;
using Refit;
using System;

namespace myTNB.AndroidApp.Src.ResetPassword.MVP
{
    public class ResetPasswordContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show empty new password error
            /// </summary>
            void ShowEmptyNewPasswordError();

            /// <summary>
            /// Show password min of 6 characters error
            /// </summary>
            void ShowPasswordMinimumOf6CharactersError();

            /// <summary>
            /// Show empty confirm password error
            /// </summary>
            void ShowEmptyConfirmNewPasswordError();

            /// <summary>
            /// Show not equal password and confirm password error
            /// </summary>
            void ShowNotEqualConfirmNewPasswordToNewPasswordError();

            /// <summary>
            /// Show progress dialog
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            /// Hide progress dialog
            /// </summary>
            void HideProgressDialog();

            /// <summary>
            /// Show success dialog
            /// </summary>
            /// <param name="message">string</param>
            void ShowSuccessMessage(string message);

            /// <summary>
            /// Show error message from api response
            /// </summary>
            /// <param name="errorMessage">string</param>
            void ShowErrorMessage(string errorMessage);

            /// <summary>
            /// Show reset password success
            /// </summary>
            void ShowResetPasswordSuccess();

            /// <summary>
            /// Disable submit button
            /// </summary>
            void DisableSubmitButton();

            /// <summary>
            /// Enable submit button
            /// </summary>
            void EnableSubmitButton();

            /// <summary>
            /// Clear all fields
            /// </summary>
            void ClearTextFields();

            /// <summary>
            /// Clears all error messages
            /// </summary>
            void ClearErrorMessages();

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
            /// Sets the no. of notification badge
            /// </summary>
            /// <param name="count">integer representation of no of badges</param>
            void ShowNotificationCount(int count);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to submit reset password
            /// </summary>
            /// <param name="apiKeyId">string</param>
            /// <param name="newPassword">string</param>
            /// <param name="confirmNewPassword">string</param>
            /// <param name="oldPassword">string</param>
            /// <param name="username">string</param>
            /// <param name="deviceId">string</param>
            void Submit(string apiKeyId, string newPassword, string confirmNewPassword, string oldPassword, string username, string deviceId);

            bool CheckPasswordIsValid(string password);
        }
    }
}