using myTNB_Android.Src.NotificationSettings.Models;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class SaveUserNotificationTypePreferenceResponse : BaseResponse<UserPreferenceData>
    {
        public UserPreferenceData GetData()
        {
            return Response.Data;
        }
    }
}
