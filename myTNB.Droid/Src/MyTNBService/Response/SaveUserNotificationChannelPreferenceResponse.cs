using System;
using myTNB.AndroidApp.Src.NotificationSettings.Models;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class SaveUserNotificationChannelPreferenceResponse : BaseResponse<UserPreferenceData>
    {
        public UserPreferenceData GetData()
        {
            return Response.Data;
        }
    }
}
