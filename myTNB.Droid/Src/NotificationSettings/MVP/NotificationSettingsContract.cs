using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.SelectNotification.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace myTNB_Android.Src.NotificationSettings.MVP
{
    public class NotificationSettingsContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            void HideShowProgressDialog();

            void ShowProgressDialog();

            /// <summary>
            /// Show notification types list
            /// </summary>
            /// <param name="typePreferenceList">List<paramref name="NotificationTypeUserPreference"/></param>
            void ShowNotificationTypesList(List<NotificationTypeUserPreference> typePreferenceList);

            /// <summary>
            /// Show notification channel list
            /// </summary>
            /// <param name="channelPreferenceList">List<paramref name="NotificationChannelUserPreference"/></param>
            void ShowNotificationChannelList(List<NotificationChannelUserPreference> channelPreferenceList);

            /// <summary>
            /// Show update success notification type
            /// </summary>
            /// <param name="typePreference">NotificationTypeUserPreference</param>
            /// <param name="position">integer</param>
            void ShowSuccessUpdatedNotificationType(NotificationTypeUserPreference typePreference, int position);

            /// <summary>
            /// Show udate success notification channel
            /// </summary>
            /// <param name="channelPreference">NotificationChannelUserPreference</param>
            /// <param name="position">integer</param>
            void ShowSuccessUpdatedNotificationChannel(NotificationChannelUserPreference channelPreference, int position);

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
            /// Shows a notification type cancelled exception with an option to retry
            /// </summary>
            /// <param name="operationCanceledException">returned OperationCanceledException</param>
            /// <param name="typePreference">NotificationTypeUserPreference</param>
            /// <param name="position">integer</param>
            void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException, NotificationTypeUserPreference typePreference, int position);

            /// <summary>
            /// Shows a notification type api exception with an option to retry
            /// </summary>
            /// <param name="apiException">returned ApiException</param>
            /// <param name="typePreference">NotificationTypeUserPreference</param>
            /// <param name="position">integer</param>
            void ShowRetryOptionsApiException(ApiException apiException, NotificationTypeUserPreference typePreference, int position);

            /// <summary>
            /// Shows a notification type unknown exception with an option to retry
            /// </summary>
            /// <param name="exception">returned Exception</param>
            /// <param name="typePreference"></param>
            /// <param name="position"></param>
            void ShowRetryOptionsUnknownException(Exception exception, NotificationTypeUserPreference typePreference, int position);



            /// <summary>
            /// Shows a notification channel cancelled exception with an option to retry
            /// </summary>
            /// <param name="operationCanceledException">returned OperationCanceledException</param>
            /// <param name="channelPreference">NotificationChannelUserPreference</param>
            /// <param name="position">integer</param>
            void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException, NotificationChannelUserPreference channelPreference, int position);

            /// <summary>
            /// Shows a notification channel api exception with an option to retry
            /// </summary>
            /// <param name="operationCanceledException">returned ApiException</param>
            /// <param name="channelPreference">NotificationChannelUserPreference</param>
            /// <param name="position">integer</param>
            void ShowRetryOptionsApiException(ApiException apiException, NotificationChannelUserPreference channelPreference, int position);

            /// <summary>
            /// Shows a notification channel unknown exception with an option to retry
            /// </summary>
            /// <param name="operationCanceledException">returned Exception</param>
            /// <param name="channelPreference">NotificationChannelUserPreference</param>
            /// <param name="position">integer</param>
            void ShowRetryOptionsUnknownException(Exception exception, NotificationChannelUserPreference channelPreference, int position);

            void OnSavedFloatingButtonTimeStampRecieved(string timestamp);

            void OnFloatingButtonTimeStampRecieved(string timestamp);

            void OnSavedFBContentTimeStampRecieved(string timestamp);

            void OnFBContentTimeStampRecieved(string timestamp);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to navigate to notification
            /// </summary>
            /// <param name="deviceId">string</param>
            void OnNotification(string deviceId);

            /// <summary>
            /// Action to select to type item
            /// </summary>
            /// <param name="item">NotificationTypeUserPreference</param>
            /// <param name="position">integer</param>
            /// <param name="deviceId">string</param>
            void OnTypeItemClick(NotificationTypeUserPreference item, int position, string deviceId);

            /// <summary>
            /// Action to select to channel item
            /// </summary>
            /// <param name="item">NotificationChannelUserPreference</param>
            /// <param name="position">integer</param>
            void OnChannelItemClick(NotificationChannelUserPreference item, int position);

            void OnGetFloatingButtonTimeStamp();

            void OnGetFloatingButtonItem();

            Task OnGetFloatingButtonCache();

            void OnGetFBContentTimeStamp();

            void OnGetFBContentItem();

            Task OnGetFBContentCache();
        }
    }
}