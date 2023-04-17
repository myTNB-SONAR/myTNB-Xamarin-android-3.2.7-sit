using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.NotificationDetails.Requests;
using myTNB_Android.Src.Notifications.Api;
using myTNB_Android.Src.Notifications.Models;
using Refit;
using static Android.Widget.CompoundButton;

namespace myTNB_Android.Src.Notifications.MVP
{
    public class NotificationContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show notification list
            /// </summary>
            /// <param name="userNotificationList">List<paramref name="UserNotificationData"/></param>
            void ShowNotificationsList(List<UserNotificationData> userNotificationList);

            /// <summary>
            /// Show notification filter
            /// </summary>
            void ShowNotificationFilter();

            /// <summary>
            /// Show notification filter name
            /// </summary>
            /// <param name="filterName">string</param>
            void ShowNotificationFilterName(string filterName);

            /// <summary>
            /// Show notification details
            /// </summary>
            /// <param name="details">NotificationDetails.Models.NotificationDetails</param>
            /// <param name="notificationData">UserNotificationData</param>
            /// <param name="position">integer</param>
            void ShowDetails(NotificationDetails.Models.NotificationDetails details, UserNotificationData notificationData, int position);

            //void ShowSelectedNotificationNewBillItem(NotificationDetails.Models.NotificationDetails details , UserNotificationData notificationData , int position);

            //void ShowSelectedNotificationDunningDisconnection(NotificationDetails.Models.NotificationDetails details, UserNotificationData notificationData, int position);

            /// <summary>
            /// Show and update read notification
            /// </summary>
            /// <param name="position">int</param>
            /// <param name="isRead">bool</param>
            void UpdateIsReadNotificationItem(int position, bool isRead);

            /// <summary>
            /// Show and delete notification
            /// </summary>
            /// <param name="position">int</param>
            /// <param name="isDelete">bool</param>
            void UpdateIsDeleteNotificationItem(int position, bool isDelete);

            /// <summary>
            /// Show notification removed
            /// </summary>
            void ShowNotificationRemoved();

            /// <summary>
            /// Show progress dialog
            /// </summary>
            void ShowProgress();

            /// <summary>
            /// Hide progress dialog
            /// </summary>
            void HideProgress();

            /// <summary>
            /// Clears adapter
            /// </summary>
            void ClearAdapter();

            void ShowView();

            void ShowRefreshView(bool isRefresh, string contentTxt, string btnTxt);


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
            /// Shows a failed error call with an option to cancel
            /// </summary>
            /// <param name="message">the returned exception</param>
            void ShowFailedErrorMessage(string errorMessage);

            /// <summary>
            /// Show query progress dialog
            /// </summary>
            void ShowQueryProgress();

            /// <summary>
            /// Hide query progress dialog
            /// </summary>
            void HideQueryProgress();

            /// <summary>
            /// The unique id of the device
            /// </summary>
            /// <returns>unique id alphanumeric strings</returns>
            string GetDeviceId();

            void DeleteNotificationByPosition(int notificationPos);

            void ReadNotificationByPosition(int notificationPos);

            void UpdatedSelectedNotifications();

            List<UserNotificationData> GetNotificationList();

            void UpdateSelectedNotification();

            void OnFailedNotificationAction();

			void ShowNotificationDetails(int notificationItemPosition);

            void UpdateDeleteNotifications();

            void UpdateReadNotifications();

            void ShowEditMode();

            void SetNotificationItemClickable(bool isClickable);

            void NavigateToDashboardWithIntent(Intent intent);
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

            /// <summary>
            /// Action to navigate to show notification filter
            /// </summary>
            void OnShowNotificationFilter();

            /// <summary>
            /// Action to query on load
            /// </summary>
            /// <param name="deviceId">string</param>
            void QueryOnLoad(string deviceId);

            //void QueryNotifications(string deviceId);

            /// <summary>
            /// Action to edit notification list
            /// </summary>
            void EditNotification();

            /// <summary>
            /// Delete all selected notifications.
            /// </summary>
            void DeleteAllSelectedNotifications();

            /// <summary>
            /// Read all selected notifications.
            /// </summary>
            void ReadAllSelectedNotifications();

            void ShowFilteredList();

            void OnShowNotificationDetails(UserNotificationData userNotification, int position);

            void InitialSetFilterName();
        }

        public interface IApiNotification
        {
            Task<NotificationApiResponse> DeleteUserNotification(string deviceId, List<UserNotificationData> userNotificationList);
            Task<NotificationApiResponse> ReadUserNotification(string deviceId, List<UserNotificationData> userNotificationList);
        }
    }
}
