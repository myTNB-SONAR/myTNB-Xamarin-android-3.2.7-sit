using System;
using myTNB.Android.Src.NotificationSettings.Models;

namespace myTNB.Android.Src.MyTNBService.Response
{
    public class SaveUserNotificationChannelPreferenceResponse : BaseResponse<UserPreferenceData>
    {
        public UserPreferenceData GetData()
        {
            return Response.Data;
        }
    }
}
