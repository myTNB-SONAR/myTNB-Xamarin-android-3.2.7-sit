using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using Refit;
using System;

namespace myTNB.AndroidApp.Src.AddNewUser.MVP
{
    public class AddNewUserContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show remove progress dialog
            /// </summary>
            void ShowProgress();

            /// <summary>
            /// Show error message from api response
            /// </summary>
            /// <param name="error">string</param>
            void ShowErrorMessageResponse(string errorTitle, string errorDetail);

            /// <summary>
            /// Hide remove progress dialog
            /// </summary>
            void HideProgress();

            /// <summary>
            /// Enable add user button
            /// </summary>
            void EnableAddUserButton();

            /// <summary>
            /// Disable add user button
            /// </summary>
            void DisableAddUserButton();

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
            
            void ShowSuccessAddNewUser(string email);
            void ShowSuccessAddNewUserPreRegister(string email);
        }
        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action on update nickname
            /// </summary>
            void OnUpdateNickname();

            /// <summary>
            /// Action to remove account
            /// </summary>
            /// <param name="accountData">AccountData</param>
            void OnAddAccount(string userEmail, string accNo, bool isHaveAccess, bool isHaveEBilling);

            void CheckRequiredFields(string email);

            /// <summary>
            /// The returned result from another activity
            /// </summary>
            /// <param name="requestCode">integer</param>
            /// <param name="resultCode">enum</param>
            /// <param name="data">intent</param>
            void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data);
        }
    }
}