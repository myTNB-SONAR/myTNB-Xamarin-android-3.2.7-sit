using myTNB.Android.Src.Base.MVP;
using Refit;
using System;

namespace myTNB.Android.Src.LogoutRate.MVP
{
    public class LogoutRateContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Shows logout success screen
            /// </summary>
            void ShowLogoutSuccess();

            /// <summary>
            /// Shows an error message when logging out
            /// </summary>
            /// <param name="message">strnig</param>
            void ShowErrorMessage(string message);

            /// <summary>
            /// Shows progress dialog
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            /// Hide progress dialog
            /// </summary>
            void HideProgressDialog();

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
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// NOT USED
            /// </summary>
            /// <param name="deviceId"></param>
            void OnLogout(string deviceId);
        }
    }
}