using System;
using myTNB.AndroidApp.Src.Base.Request;
using myTNB.AndroidApp.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.MyTNBService.Request
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


