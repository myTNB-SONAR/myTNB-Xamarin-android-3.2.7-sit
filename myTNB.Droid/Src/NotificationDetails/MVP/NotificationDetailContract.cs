using myTNB.Android.Src.Base.MVP;
using Refit;
using System;

namespace myTNB.Android.Src.NotificationDetails.MVP
{
    public class NotificationDetailContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Set customized toolbar title text
            /// </summary>
            /// <param name="resourceString">integer</param>
            void ShowToolbarTitle(int resourceString);

            /// <summary>
            /// Show removing progress dialog
            /// </summary>
            void ShowRemovingProgress();

            /// <summary>
            /// Hide removing progress dialog
            /// </summary>
            void HideRemovingProgress();

            /// <summary>
            /// Show notification list as deleted
            /// </summary>
            void ShowNotificationListAsDeleted();

            /// <summary>
            /// Show account number
            /// </summary>
            void ShowAccountNumber();

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
            /// The unique id of the device
            /// </summary>
            /// <returns>unique id alphanumeric strings</returns>
            string GetDeviceId();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to navigate to remove notification
            /// </summary>
            /// <param name="notificationDetails">NotificationDetails.Models.NotificationDetails</param>
            void OnRemoveNotification(NotificationDetails.Models.NotificationDetails notificationDetails);

            /// <summary>
            /// Action to navigate to view details
            /// </summary>
            void OnViewDetails();

            /// <summary>
            /// Action to navigate to pay
            /// </summary>
            void OnPay();
        }
    }
}