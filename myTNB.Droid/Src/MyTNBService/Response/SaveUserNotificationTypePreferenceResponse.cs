using myTNB.Android.Src.NotificationSettings.Models;

namespace myTNB.Android.Src.MyTNBService.Response
{
    public class SaveUserNotificationTypePreferenceResponse : BaseResponse<UserPreferenceData>
    {
        public UserPreferenceData GetData()
        {
            return Response.Data;
        }
    }
}
