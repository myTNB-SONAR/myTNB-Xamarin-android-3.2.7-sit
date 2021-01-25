using System.Collections.Generic;
using myTNB_Android.Src.AppLaunch.Models;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class UserNotificationResponse : BaseResponse<UserNotificationResponse.UserNotificationResponseData>
    {
        public UserNotificationResponseData GetData()
        {
            return Response.Data;
        }

        public class UserNotificationResponseData
        {
            [JsonProperty(PropertyName = "UserNotificationList")]
            public List<UserNotification> UserNotificationList { get; set; }
        }
    }
}