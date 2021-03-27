using Android.Content.PM;
using myTNB_Android.Src.Base.MVP;
using Refit;
using System;

namespace myTNB_Android.Src.RegisterValidation.MVP
{
    public class RegistrationValidationContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Shows invalid pin
            /// </summary>
            void ShowInvalidPin();

            /// <summary>
            /// Shows a message that an An SMS containing the activation pin has been sent to your number.
            /// </summary>
            void ShowSMSPinInfo();

            /// <summary>
            /// Shows a view that a pin has been resent
            /// </summary>
            void ShowResendPin();

            /// <summary>
            /// 
            /// </summary>
            void StartProgress();

            /// <summary>
            ///  Shows the error returned from the API
            ///  Post-Validation
            /// </summary>
            void ShowError(string errorMessage);

            /// <summary>
            /// Returns unique device id
            /// </summary>
            /// <returns>string</returns>
            string GetDeviceId();

            /// <summary>
            /// Returns bool representation of counting down
            /// </summary>
            /// <returns>bool</returns>
            bool IsStillCountingDown();

            /// <summary>
            /// Clear all errors
            /// </summary>
            void ClearErrors();

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

            /// <summary>
            /// Enable resend button
            /// </summary>
            void EnableResendButton();

            /// <summary>
            /// Disable resend button
            /// </summary>
            void DisableResendButton();

            /// <summary>
            /// Show account list
            /// </summary>
            void ShowAccountListActivity();

            void ShowEmailRegisterPopUp();

            /// <summary>
            /// Show snackbar error from api response
            /// </summary>
            /// <param name="resourceStringId">integer</param>
            void ShowSnackbarError(int resourceStringId);

            /// <summary>
            /// Show request sms permission
            /// </summary>
            void RequestSMSPermission();

            /// <summary>
            /// Show sms permission rationale
            /// </summary>
            void ShowSMSPermissionRationale();

            /// <summary>
            /// Returns granted sms permission
            /// </summary>
            /// <returns>bool</returns>
            bool IsGrantedSMSReceivePermission();

            /// <summary>
            /// Returns if valid to show sms permission
            /// </summary>
            /// <returns>bool</returns>
            bool ShouldShowSMSReceiveRationale();

            /// <summary>
            /// Show registration progress dialog
            /// </summary>
            void ShowRegistrationProgress();

            /// <summary>
            /// Hide registration progress dialog
            /// </summary>
            void HideRegistrationProgress();


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

            void ShowNotificationCount(int count);

            void ShowEmptyErrorPin();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// User actions that resends request for SMS pin
            /// <example>
            /// <see cref="OnRegister(string, string, string, string, string)" /> method
            /// </example>
            /// </summary>
            ///<see cref="OnRegister(string, string, string, string, string)"/>
            void ResendAsync();

            /// <summary>
            /// Action to register 
            /// </summary>
            /// <param name="num1">string</param>
            /// <param name="num2">string</param>
            /// <param name="num3">string</param>
            /// <param name="num4">string</param>
            /// <param name="deviceId">string</param>
            void OnRegister(string num1, string num2, string num3, string num4, string deviceId);

            /// <summary>
            /// Action to complete
            /// </summary>
            void OnComplete();

            /// <summary>
            /// Action to navigate to account list
            /// </summary>
            void OnNavigateToAccountListActivity();

            /// <summary>
            /// Action for request permission result
            /// </summary>
            /// <param name="requestCode"></param>
            /// <param name="permissions"></param>
            /// <param name="grantResults"></param>
            void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults);

        }
    }
}