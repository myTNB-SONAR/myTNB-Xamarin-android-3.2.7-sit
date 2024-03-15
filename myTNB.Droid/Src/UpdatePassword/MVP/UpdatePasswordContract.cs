using myTNB.AndroidApp.Src.Base.MVP;
using Refit;
using System;

namespace myTNB.AndroidApp.Src.UpdatePassword.MVP
{
    public class UpdatePasswordContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show empty current password error
            /// </summary>
            void ShowEmptyCurrentPassword();

            /// <summary>
            /// Show empty new password error
            /// </summary>
            void ShowEmptyNewPassword();

            /// <summary>
            /// Show empty confirm password error
            /// </summary>
            void ShowEmptyConfirmPassword();

            /// <summary>
            /// Show invalid current password error
            /// </summary>
            void ShowInvalidCurrentPassword();

            /// <summary>
            /// Show invalid new password error
            /// </summary>
            void ShowInvalidNewPassword();

            /// <summary>
            /// Show new password not eq to confirm password error
            /// </summary>
            void ShowNewPasswordNotEqualToConfirmPassword();

            /// <summary>
            /// Clear all errors
            /// </summary>
            void ClearErrors();

            /// <summary>
            /// Show progress dialog
            /// </summary>
            void ShowProgress();

            /// <summary>
            /// Hide progress dialog
            /// </summary>
            void HideProgress();

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
            /// Show error message from api response
            /// </summary>
            /// <param name="message">string</param>
            void ShowErrorMessage(string message);

            /// <summary>
            /// Show success
            /// </summary>
            void ShowSuccess();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to save update password
            /// </summary>
            /// <param name="currentPassword">string</param>
            /// <param name="newPassword">string</param>
            /// <param name="confirmNewPassword">string</param>
            void OnSave(string currentPassword, string newPassword, string confirmNewPassword);

            /// <summary>
            /// Checks if password is valid
            /// </summary>
            bool CheckPasswordIsValid(string password);
        }
    }
}