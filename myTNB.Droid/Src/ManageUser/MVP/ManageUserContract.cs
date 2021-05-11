using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using Refit;
using System;

namespace myTNB_Android.Src.ManageUser.MVP
{
    public class ManageUserContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show remove progress dialog
            /// </summary>
            void ShowRemoveProgress();

            /// <summary>
            /// Show error message from api response
            /// </summary>
            /// <param name="error">string</param>
            void ShowErrorMessageResponse(string error);

            /// <summary>
            /// Hide remove progress dialog
            /// </summary>
            void HideRemoveProgress();

            /// <summary>
            /// Show initial nickname
            /// </summary>
            /// <param name="nickname">string</param>
            void ShowNickname(string nickname);

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

            void ShowSaveSuccess();
            
            void DisableSaveButton();

            void DisableResendButton();
            
            void PopulateDataCheckBox(UserManageAccessAccount updateacc);
            
            void ShowSuccessCancelInvite(string cancelInvite);

            void ShowInviteSuccess(string cancelInvite);
        }
        public interface IUserActionsListener : IBasePresenter
        {

            /// <summary>
            /// The returned result from another activity
            /// </summary>
            /// <param name="requestCode">integer</param>
            /// <param name="resultCode">enum</param>
            /// <param name="data">intent</param>
            void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data);

            void UpdateAccountAccessRight(string userAccountId, bool isHaveAccess, bool isApplyEBilling, string email);

            void CancelInvitedUser(string userId);

            void ResendInvitedUser(string email, string AccNum, bool isHaveAccess,bool isApplyEBilling);
        }
    }
}