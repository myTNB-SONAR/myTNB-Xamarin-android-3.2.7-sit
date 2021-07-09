using System;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class UserNotificationDetailsRequestNew : BaseRequest
    {
       
        public string NotificationTypeID;
        public string BCRMNotificationTypeID;
        public string NotificationRequestId;

        public UserNotificationDetailsRequestNew(string notificationTypeID, string BCRMTypeID, string notificationRequestId)
        {
            NotificationTypeID = notificationTypeID;
            BCRMNotificationTypeID = BCRMTypeID;
            NotificationRequestId = notificationRequestId;
        }
    }
}


