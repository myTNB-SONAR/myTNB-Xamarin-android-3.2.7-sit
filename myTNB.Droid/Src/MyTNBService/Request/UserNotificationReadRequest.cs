using System;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.Base.Request;
using myTNB.AndroidApp.Src.Notifications.Models;

namespace myTNB.AndroidApp.Src.MyTNBService.Request
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
