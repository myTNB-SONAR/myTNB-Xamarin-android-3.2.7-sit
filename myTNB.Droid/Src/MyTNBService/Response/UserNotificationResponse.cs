using System;
using System.Collections.Generic;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.Response;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class UserNotificationResponse
    {
        [JsonProperty(PropertyName = "d")]
        public APIResponse Data { get; set; }

        public class APIResponse : APIBaseResponse
        {
            [JsonProperty(PropertyName = "data")]
            public UserNotificationResponseData ResponseData { get; set; }

            [JsonProperty(PropertyName = "RefreshTitle")]
            public string RefreshTitle { get; set; }

            [JsonProperty(PropertyName = "RefreshMessage")]
            public string RefreshMessage { get; set; }

            [JsonProperty(PropertyName = "RefreshBtnText")]
            public string RefreshBtnText { get; set; }
        }

        public class UserNotificationResponseData
        {
            [JsonProperty(PropertyName = "userNotifications")]
            public List<UserNotification> UserNotificationList { get; set; }
        }

        //public class UserNotification
        //{
        //    [JsonProperty(PropertyName = "NotificationType")]
        //    public string NotificationType { get; set; }

        //    [JsonProperty(PropertyName = "Id")]
        //    public string Id { get; set; }

        //    [JsonProperty(PropertyName = "Email")]
        //    public string Email { get; set; }

        //    [JsonProperty(PropertyName = "DeviceId")]
        //    public string DeviceId { get; set; }

        //    [JsonProperty(PropertyName = "AccountNum")]
        //    public string AccountNum { get; set; }

        //    [JsonProperty(PropertyName = "Title")]
        //    public string Title { get; set; }

        //    [JsonProperty(PropertyName = "Message")]
        //    public string Message { get; set; }

        //    [JsonProperty(PropertyName = "IsRead")]
        //    public string IsRead { get; set; }

        //    [JsonProperty(PropertyName = "IsDeleted")]
        //    public string IsDeleted { get; set; }

        //    [JsonProperty(PropertyName = "NotificationTypeId")]
        //    public string NotificationTypeId { get; set; }

        //    [JsonProperty(PropertyName = "BCRMNotificationTypeId")]
        //    public string BCRMNotificationTypeId { get; set; }

        //    [JsonProperty(PropertyName = "Target")]
        //    public string Target { get; set; }

        //    [JsonProperty(PropertyName = "CreatedDate")]
        //    public string CreatedDate { get; set; }

        //    [JsonProperty(PropertyName = "AccountDetails")]
        //    public string AccountDetails { get; set; }
        //}
    }
}
