using System.Collections.Generic;
using myTNB.Mobile;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Utils;
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
                    if (entity.BCRMNotificationTypeId != Constants.BCRM_NOTIFICATION_MYHOME_NC_ADDRESS_SEARCH_COMPLETED &&
                        entity.BCRMNotificationTypeId != Constants.BCRM_NOTIFICATION_MYHOME_NC_RESUME_APPLICATION &&
                        entity.BCRMNotificationTypeId != Constants.BCRM_NOTIFICATION_MYHOME_NC_APPLICATION_COMPLETED &&
                        entity.BCRMNotificationTypeId != Constants.BCRM_NOTIFICATION_MYHOME_NC_APPLICATION_CONTRACTOR_COMPLETED &&
                        entity.BCRMNotificationTypeId != Constants.BCRM_NOTIFICATION_MYHOME_NC_OTP_VERIFY &&
                        entity.BCRMNotificationTypeId != Constants.BCRM_NOTIFICATION_MYHOME_NC_CONTRACTOR_ACCEPTED &&
                        entity.BCRMNotificationTypeId != Constants.BCRM_NOTIFICATION_MYHOME_NC_CONTRACTOR_REJECTED &&
                        entity.BCRMNotificationTypeId != Constants.BCRM_NOTIFICATION_MYHOME_NC_CONTRACTOR_NO_RESPONSE &&
                        entity.BCRMNotificationTypeId != Constants.BCRM_NOTIFICATION_MYHOME_NC_APPLICATION_REQUIRES_UPDATE)
                    {
                        filteredList.Add(entity);
                    }
                }
            }

            return filteredList;
        }
    }
}