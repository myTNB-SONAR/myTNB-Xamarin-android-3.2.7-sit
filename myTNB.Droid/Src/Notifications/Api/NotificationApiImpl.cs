using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Notifications.Models;
using myTNB_Android.Src.Notifications.MVP;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.Notifications.Api
{
    public class NotificationApiCall : NotificationContract.IApiNotification
    {
        INotificationApi notificationApi = null;
        HttpClient httpClient = null;
        public NotificationApiCall()
        {
#if DEBUG
            httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            notificationApi = RestService.For<INotificationApi>(httpClient);
#else
            notificationApi = RestService.For<INotificationApi>(Constants.SERVER_URL.END_POINT);
#endif
        }

        public Task<NotificationApiResponse> DeleteUserNotification(string deviceId, List<Models.UserNotificationData> userNotificationList)
        {
			List<NotificationData> updatedNotifications = new List<NotificationData>();
			foreach(UserNotificationData userNotificationData in userNotificationList)
			{
                updatedNotifications.Add(new NotificationData()
                {
                    NotificationId = userNotificationData.Id,
                    NotificationType = userNotificationData.NotificationType
                });
			}
            //var response = await notificationApi.DeleteUserNotification(new NotificationDeleteRequest
            //{
            //    ApiKeyID = Constants.APP_CONFIG.API_KEY_ID,
            //    Email = UserEntity.GetActive().Email,
            //    DeviceId = deviceId,
            //    SSPUserId = UserEntity.GetActive().UserID,
            //    UpdatedNotifications = updatedNotifications
            //}, new CancellationTokenSource());

            //return response;
            return notificationApi.DeleteUserNotification(new NotificationDeleteRequest
            {
                ApiKeyID = Constants.APP_CONFIG.API_KEY_ID,
                Email = UserEntity.GetActive().Email,
                DeviceId = deviceId,
                SSPUserId = UserEntity.GetActive().UserID,
                UpdatedNotifications = updatedNotifications
            }, new CancellationTokenSource());
        }

        public async Task<NotificationApiResponse> ReadUserNotification(string deviceId, List<Models.UserNotificationData> userNotificationList)
        {
            List<NotificationData> updatedNotifications = new List<NotificationData>();
            foreach (UserNotificationData userNotificationData in userNotificationList)
            {
                updatedNotifications.Add(new NotificationData()
                {
                    NotificationId = userNotificationData.Id,
                    NotificationType = userNotificationData.NotificationType
                });
            }
            return await notificationApi.ReadUserNotification(new NotificationReadRequest
            {
                ApiKeyID = Constants.APP_CONFIG.API_KEY_ID,
                Email = UserEntity.GetActive().Email,
                DeviceId = deviceId,
                SSPUserId = UserEntity.GetActive().UserID,
                UpdatedNotifications = updatedNotifications
            }, new CancellationTokenSource());
        }
    }
}
