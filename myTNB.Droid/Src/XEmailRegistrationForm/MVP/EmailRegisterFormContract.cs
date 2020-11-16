using Android.Content.PM;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.XEmailRegistrationForm.Models;
using Refit;
using System;

namespace myTNB_Android.Src.XEmailRegistrationForm.MVP
{
    public class EmailRegisterFormContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
           
            /// 
            //void ShowRegister();
            void ShowRegister(UserCredentialsEntity entity);


            /// <summary>
            /// Show feedback
            /// </summary>
            /// 
            //void ShowEmailMessage();
            /// <summary>
            /// Show email message
            /// </summary>
            void ShowEmptyEmailError();

            /// <summary>
            /// Show invalid email error using regex
            /// Pre Validation
            /// </summary>
            void ShowInvalidEmailError();

            /// <summary>
            /// Show  hint email
            /// Pre Validation
            /// </summary>
            void ShowEmailHint();

            void ClearInvalidEmailHint();

            void ShowInvalidEmailPasswordError();

            void ShowInvalidAcquiringTokenThruSMS(string errorMessage);


            void ClearInvalidEmailError();

            /// <summary>
            /// Show empty confirm email error
            /// Pre Validation
            /// </summary>
            //void ShowEmptyConfirmEmailError();



            /// <summary>
            /// Show not equal to email confirm email error 
            /// Pre Validation
            /// </summary>
            //void ShowNotEqualConfirmEmailError();

            /// <summary>
            /// Clear error confirm email field
            /// </summary>
           // void ClearNotEqualConfirmEmailError();

            /// <summary>
            /// Show empty password error
            /// Pre Validation
            /// </summary>
            void ShowEmptyPasswordError();

            /// <summary>
            /// Show min password error
            /// </summary>
            void ShowPasswordMinimumOf6CharactersError();

            /// <summary>
            /// Clear error password field
            /// </summary>
            void ClearPasswordMinimumOf6CharactersError();

            /// <summary>
            /// Show empty confirm password error
            /// Pre Validation
            /// </summary>
            //void ShowEmptyConfirmPasswordError();

            /// <summary>
            /// Show not equal to password confirm password error
            /// Pre Validation
            /// </summary>
            //void ShowNotEqualConfirmPasswordError();

            /// <summary>
            /// Clear error password error
            /// </summary>
            //void ClearNotEqualConfirmPasswordError();


            /// <summary>
            /// Shows previous screen
            /// </summary>
            void ShowBackScreen();


            /// <summary>
            /// Clears all fields
            /// </summary>
            void ClearFields();

            /// <summary>
            /// Clears all error messages
            /// </summary>
            void ClearAllErrorFields();


            /// <summary>
            /// Show registration progress dialog
            /// </summary>
            void ShowRegistrationProgressDialog();

            /// <summary>
            /// Hide registration progress dialog
            /// </summary>
            void HideRegistrationProgressDialog();

            /// <summary>
            /// Shows the next screen register validation
            /// </summary> 
            //void ShowRegisterValidation(UserCredentialsEntity entity);

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
            /// Enable register button
            /// </summary>
            void EnableRegisterButton();

            /// <summary>
            /// Disable register button
            /// </summary>
            void DisableRegisterButton();

            /// <summary>
            /// Checks for showing sms rationale
            /// </summary>
            /// <returns>boolean representation of should show rationale or not</returns>
           // bool ShouldShowSMSReceiveRationale();

           // void ShowFullNameError();


           // void ClearFullNameError();


            void ShowProgressDialog();

            void HideProgressDialog();

        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// User action to register
            /// </summary>

            /// <param name="email">string email</param>
            /// <param name="password">string password</param>
            /// 

            //void NavigateToRegister();

            /// <summary>
            /// Action to navigate to find us
            /// </summary>
            void OnAcquireToken(string email, string password);

            void CheckRequiredFields(string email, string password);

            /// <summary>
            /// User actions to go back to previous screen
            /// </summary>
            void GoBack();
        }
    }
}