using System;
using myTNB.AndroidApp.Src.Base.Response;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class UserNotificationDetailsResponse : BaseResponse<UserNotificationDetailsResponse.UserNotificationDetailsResponseData>
    {
        public UserNotificationDetailsResponseData GetData()
        {
            return Response.Data;
        }
        public class UserNotificationDetailsResponseData
        {
            [JsonProperty(PropertyName = "UserNotification")]
            public NotificationDetails.Models.NotificationDetails UserNotificationDetail { get; set; }
        }
    }
}
