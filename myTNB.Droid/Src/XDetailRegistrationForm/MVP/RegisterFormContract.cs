using Android.Content.PM;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.XDetailRegistrationForm.Models;
using Refit;
using System;

namespace myTNB_Android.Src.XDetailRegistrationForm.MVP
{
    public class RegisterFormContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {

            void ShowIdentificationHint();
            void ShowInvalidIdentificationError();

            /// <summary>
            /// Show empty fullname error
            /// Pre Validation
            /// </summary>
            void ShowEmptyFullNameError();

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
            /// Show empty mobile no error
            /// Pre Validation
            /// </summary>
            void ShowEmptyMobileNoError();

            /// <summary>
            /// Show invalid mobile no error
            /// Pre Validation
            /// </summary>
            void ShowInvalidMobileNoError();

            /// <summary>
            /// Show min password error
            /// </summary>
            void ShowPasswordMinimumOf6CharactersError();

            /// <summary>
            /// Clear error password field
            /// </summary>
            void ClearICMinimumCharactersError();

            /// <summary>
            /// Shows an invalid message that the registration is unsuccessful
            /// </summary>
            /// <param name="errorMessage"></param>
            void ShowInvalidAcquiringTokenThruSMS(string errorMessage);


            /// <summary>
            /// Shows terms & conditions activity/dialog
            /// </summary>
            void ShowTermsAndConditions();

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
            void ShowRegisterValidation(UserCredentialsEntity entity);

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
            /// Action to show request SMS Permission
            /// </summary>
            void RequestSMSPermission();

            /// <summary>
            /// Shows SMS Permission
            /// </summary>
            void ShowSMSPermissionRationale();

            /// <summary>
            /// Checks for sms permission
            /// </summary>
            /// <returns>boolean representation of granted(true) or denied(false)</returns>
            bool IsGrantedSMSReceivePermission();

            /// <summary>
            /// Checks for showing sms rationale
            /// </summary>
            /// <returns>boolean representation of should show rationale or not</returns>
            bool ShouldShowSMSReceiveRationale();

            //void ShowFullNameError();

            void ShowFullICError();

            void ShowFullArmyIdError();

            void ShowFullPassportError();

            void ClearFullNameError();

            void ClearInvalidMobileError();

            void ClearICHint();

            void ShowProgressDialog();

            void HideProgressDialog();

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
            void OnAcquireToken(string fullname, string icno, string mobile_no, string email, string password, string idtype);

            void OnCheckID(string icno, string idtype);

            bool validateField(string fullname, string icno, string mobile_no, string idtype, bool checkbox);

            void CheckRequiredFields(string fullname, string icno, string mobile_no, string idtype, bool checkbox);

            /// <summary>
            /// User actions to navigate to terms & condition screen
            /// </summary>
            void NavigateToTermsAndConditions();

            /// <summary>
            /// User actions to go back to previous screen
            /// </summary>
            void GoBack();


            /// <summary>
            /// Action to request sms permission
            /// </summary>
            void OnRequestSMSPermission();

            /// <summary>
            /// The returned permission result
            /// </summary>
            /// <param name="requestCode">integer</param>
            /// <param name="permissions">string[] array</param>
            /// <param name="grantResults">Permission[] array</param>
            void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults);
        }
    }
}