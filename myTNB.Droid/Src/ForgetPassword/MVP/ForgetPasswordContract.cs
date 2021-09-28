using myTNB_Android.Src.Base.MVP;
using Refit;
using System;

namespace myTNB_Android.Src.ForgetPassword.MVP
{
    public class ForgetPasswordContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {

            /// <summary>
            /// Shows an empty email error
            /// Pre Validation
            /// </summary>
            void ShowEmptyEmailError();

            /// <summary>
            /// 
            /// </summary>
            void ShowEmptyCodeError();

            /// <summary>
            /// Shows an invalid email address
            /// Pre Validation
            /// </summary>
            void ShowInvalidEmailError();

            /// <summary>
            /// Shows a snackbar after success resend email verification
            /// </summary>
            void ShowEmailUpdateSuccess(string message, string email);

            /// <summary>
            /// Shows error message
            /// </summary>
            /// <param name="errorMessage"></param>
            void ShowError(string errorMessage);

            /// <summary>
            /// Shows success that the code will be sent to the email
            /// </summary>
            /// <param name="message"></param>
            void ShowSuccess(string message);


            void ShowCodeVerifiedSuccess();


            /// <summary>
            /// Show progress dialog in resetting password
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            /// Hide progress dialog after theres a response or catched an error (http error , cancelled)
            /// </summary>
            void HideProgressDialog();

            /// <summary>
            /// Show get code progress dialog
            /// </summary>
            void ShowGetCodeProgressDialog();

            /// <summary>
            /// Hide get code progress dialog
            /// </summary>
            void HideGetCodeProgressDialog();

            /// <summary>
            /// Clears all error messages in textinput layout
            /// </summary>
            void ClearErrorMessages();

            /// <summary>
            /// Clears all text in the edit text fields
            /// </summary>
            void ClearTextFields();

            /// <summary>
            /// Enable submit button after success or error from api access
            /// </summary>
            void EnableSubmitButton();

            /// <summary>
            /// Disable submit button to avoid multiple presses
            /// </summary>
            void DisableSubmitButton();

            /// <summary>
            /// NOT USED
            /// </summary>
            void StartProgress();

            /// <summary>
            /// NOT USED
            /// </summary>
            void EnableResendButton();

            /// <summary>
            /// NOT USED
            /// </summary>
            void DisableResendButton();

            /// <summary>
            /// Shows an cancelled exception with an option to retry
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
            /// Show empty error on pin 1
            /// </summary>
            void ShowEmptyErrorPin_1();

            /// <summary>
            /// Show empty error on pin 2
            /// </summary>
            void ShowEmptyErrorPin_2();

            /// <summary>
            /// Show empty error on pin 3
            /// </summary>
            void ShowEmptyErrorPin_3();

            /// <summary>
            /// Show empty error on pin 4
            /// </summary>
            void ShowEmptyErrorPin_4();

            void ShowEmptyErrorPin();

            void ShowEmailResendSuccess();

        }

        public interface IUserActionsListener : IBasePresenter
        {

            /// <summary>
            /// User actions to submit forget password email
            /// </summary>
            /// <param name="email">string value</param>
            /// <param name="username">string value</param>
            /// <param name="code">string value</param>
            void Submit(string apiKeyId, string email, string username, string code);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="email"></param>
            void GetCode(string apiKeyId, string email);

            void OnComplete();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="email"></param>
            void ResendEmailVerify(string apiKeyId, string email);
        }
    }
}