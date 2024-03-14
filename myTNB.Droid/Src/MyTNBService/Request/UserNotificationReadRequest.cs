using System;
using System.Collections.Generic;
using myTNB.Android.Src.Base.Request;
using myTNB.Android.Src.Notifications.Models;

namespace myTNB.Android.Src.MyTNBService.Request
{
    public class UserNotificationReadRequest : BaseRequest
    {
        public List<NotificationData> updatedNotifications;
        public NotificationData notificationData;
        public UserNotificationReadRequest(List<UserNotificationData> selectedNotificationList)
        {
            updatedNotifications = new List<NotificationData>();
            selectedNotificationList.ForEach(data =>
            {
                notificationData = new NotificationData();
                notificationData.NotificationId = data.Id;
                notificationData.NotificationType = data.NotificationType;
                updatedNotifications.Add(notificationData);
            });
        }

        public class NotificationData
        {
            public string NotificationType { get; set; }
            public string NotificationId { get; set; }
        }
    }
}
