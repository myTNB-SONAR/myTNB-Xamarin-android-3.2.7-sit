using System.Collections.Generic;
using myTNB.Mobile;
using myTNB.AndroidApp.Src.AppLaunch.Models;
using myTNB.AndroidApp.Src.MyHome;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
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

            [JsonIgnore]
            public List<UserNotification> FilteredUserNotificationList {
                get
                {
                    if (!MyHomeUtility.Instance.IsAccountEligible)
                    {
                        return FilterOutMyHomeNotifs(UserNotificationList);
                    }

                    return UserNotificationList;
                }
            }
        }

        public static List<UserNotification> FilterOutMyHomeNotifs(List<UserNotification> list)
        {
            List<UserNotification> filteredList = new List<UserNotification>();
            foreach (UserNotification entity in list)
            {
                if (entity.BCRMNotificationTypeId.IsValid())
                {
                    if (!MyHomeConstants.MYHOME_NOTIFS.Contains(entity.BCRMNotificationTypeId))
                    {
                        filteredList.Add(entity);
                    }
                }
            }

            return filteredList;
        }
    }
}