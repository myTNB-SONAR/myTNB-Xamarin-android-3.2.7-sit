using System;
using myTNB.Android.Src.Base.Request;

namespace myTNB.Android.Src.MyTNBService.Request
{
    public class UserNotificationDetailsRequest : BaseRequest
    {
        public string NotificationType;
        public string NotificationId;
        public string PushMapId;
        public DeviceInfoRequest deviceInf;
        public UserNotificationDetailsRequest(string notificationId, string notificationType)
        {
            NotificationId = notificationId;
            NotificationType = notificationType;
            deviceInf = new DeviceInfoRequest();
        }
    }
}
