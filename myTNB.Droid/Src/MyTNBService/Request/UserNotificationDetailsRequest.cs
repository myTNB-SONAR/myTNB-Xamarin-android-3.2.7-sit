using System;
using myTNB_Android.Src.Base.Request;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class UserNotificationDetailsRequest : APIBaseRequest
    {
        public string NotificationType;
        public string NotificationId;
        public UserNotificationDetailsRequest(string notificationId, string notificationType)
        {
            NotificationId = notificationId;
            NotificationType = notificationType;
        }
    }
}
