using System;
using myTNB.Android.Src.Base.Request;
using myTNB.Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB.Android.Src.MyTNBService.Request
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


