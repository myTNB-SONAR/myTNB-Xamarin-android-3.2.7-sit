using System;
using myTNB_Android.Src.NotificationSettings.Models;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class SaveUserNotificationChannelPreferenceResponse : BaseResponse<UserPreferenceData>
    {
        public UserPreferenceData GetData()
        {
            return Response.Data;
        }
    }
}
