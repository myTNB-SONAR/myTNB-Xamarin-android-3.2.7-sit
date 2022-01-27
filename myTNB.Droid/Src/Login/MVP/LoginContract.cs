using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Login.Requests;
using Refit;
using System;

namespace myTNB_Android.Src.Login.MVP
{
    public class LoginContract
    {

        public interface IView : IBaseView<IUserActionsListener>
        {

            /// <summary>
            /// Shows progress dialog invokes when user clicked logged in
            ///  Pre-Validation
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            ///  Hides progress dialog when logging in is finish
            ///  Pre-Validation
            /// </summary>
            void HideProgressDialog();

            /// <summary>
            ///  Shows empty email error
            ///  Pre-Validation
            /// </summary>
            void ShowEmptyEmailError();

            /// <summary>
            ///  Shows empty password error
            ///  Pre-Validation
            /// </summary>
            void ShowEmptyPasswordError();

            /// <summary>
            ///  Shows the invalid email error using regular expression
            ///  Pre-Validation
            /// </summary>
            void ShowInvalidEmailError();

            /// <summary>
            ///  Shows the invalid email/password error returned from the API
            ///  Post-Validation
            /// </summary>
            void ShowInvalidEmailPasswordError(string errorMessageTitle, string errorMessageDetails);

            /// <summary>
            /// Clears email & password errors
            /// </summary>
            void ClearErrors();

            /// <summary>
            /// Shows Dashboard Activity
            /// </summary>
            void ShowDashboard();

            /// <summary>
            /// Shows Add Account Activity
            /// </summary>
            void ShowAddAccount();
            

            /// <summary>
            /// Shows Reset Password ACtivity provided there is a reset flag
            /// </summary>
            void ShowResetPassword(string email, string enteredPassword);

            /// <summary>
            /// Shows ForgetPassword Activity
            /// </summary>
            void ShowForgetPassword();

            /// <summary>
            /// Shows Register Form
            /// </summary>
            void ShowRegisterForm();

            /// <summary>
            /// Disables Login Button
            /// </summary>
            void DisableLoginButton();

            /// <summary>
            /// Enable Login Button
            /// </summary>
            void EnableLoginButton();

            /// <summary>
            /// The unique id of the device
            /// </summary>
            /// <returns>unique id alphanumeric strings</returns>
            string GetDeviceId();


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


            /// <summary>
            /// Force user to update phone number
            /// </summary>
            void ShowUpdatePhoneNumber(UserAuthenticationRequest request, string phoneNumber);

            void ShowEmailVerifyDialog();

            // /// <summary>
            // /// Shows a snackbar after success resend email verification
            // /// </summary>
            void ShowEmailUpdateSuccess(string email);

            // /// <summary>
            // /// Shows error message
            // /// </summary>
            // /// <param name="errorMessage"></param>
            void ShowError(string errorMessage);


            // /// <summary>
            // /// Shows a snackbar after success email verification through dynamic link
            // /// </summary>
            void ShowUpdateUserStatusActivate(string message);


            // /// <summary>
            // /// Shows a snackbar after success removed account through dynamic link
            // /// </summary>
            void ShowUpdateUserStatusDeactivate(string message);


#if STUB
            string GetCustomerAccountsStub();

            string GetCustomerAccountsStubV5();

            string GetLoginResponseStubV4();

            string GetLoginResponseStubV5();
#endif

        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Performs all prevalidation and does login asynchronously
            /// If data returns an error then it shows the post validation error
            /// </summary>
            /// <param name="username"></param>
            /// <param name="password"></param>
            void LoginAsync(string username, string password, string deviceId, bool rememberMe);

            /// <summary>
            /// Navigates to Forget Password Screen
            /// </summary>
            void NavigateToForgetPassword();

            /// <summary>
            /// Navigates to Register form
            /// </summary>
            void NavigateToRegistrationForm();

            /// <summary>
            /// Cancels in-progress login
            /// </summary>
            void CancelLogin();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="email"></param>
            void ResendEmailVerify(string apiKeyId, string email);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="userId"></param>
            void UpdateUserStatusActivate(string userId);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="userId"></param>
            void UpdateUserStatusDeactivate(string userId);

        }
    }
}