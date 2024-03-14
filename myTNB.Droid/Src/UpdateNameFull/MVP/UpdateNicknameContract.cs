using myTNB.Android.Src.Base.MVP;
using Refit;
using System;

namespace myTNB.Android.Src.UpdateNameFull.MVP
{
    public class UpdateNicknameContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show empty nickname error
            /// </summary>
            void ShowEmptyNickNameError();

            /// <summary>
            /// Show error message ffrom api response
            /// </summary>
            /// <param name="error">string</param>
            void ShowResponseError(string error);

            /// <summary>
            /// Show progress dialog
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            /// Hide progress dialog
            /// </summary>
            void HideProgressDialog();

            /// <summary>
            /// Show update nickname success
            /// </summary>
            /// <param name="newNickName">string</param>
            void ShowSuccessUpdateName();

            /// <summary>
            /// Enable save button
            /// </summary>
            void EnableSaveButton();

            /// <summary>
            /// Disable save button
            /// </summary>
            void DisableSaveButton();

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

            void ClearError();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to update nickname
            /// </summary>
            /// <param name="newName">string</param>
            void OnUpdateName(string newName);

            /// <summary>
            /// Action to verify nickname
            /// </summary>
            /// <param name="accountNo">string</param>
            /// <param name="newAccountNickname">string</param>
            void OnVerifyName(string newName);
        }
    }
}