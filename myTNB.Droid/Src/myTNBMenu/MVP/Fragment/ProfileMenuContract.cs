using System;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Database.Model;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class ProfileMenuContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show notification
            /// </summary>
            void ShowNotifications();

            /// <summary>
            /// Show notification progress dialog
            /// </summary>
            void ShowNotificationsProgressDialog();

            /// <summary>
            /// HIde notification progress dialog
            /// </summary>
            void HideNotificationsProgressDialog();

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
            /// Shows user data
            /// </summary>
            /// <param name="user">UserEntity</param>
            /// <param name="numOfCards">integer</param>
            void ShowUserData(UserEntity user, int numOfCards);

        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to navigate to notification
            /// </summary>
            /// <param name="deviceId">string</param>
            void OnNotification(string deviceId);

        }
    }
}
