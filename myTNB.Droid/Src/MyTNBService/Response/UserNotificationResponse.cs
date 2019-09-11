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
    }
}
