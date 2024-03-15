using myTNB.AndroidApp.Src.NotificationSettings.Models;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class SaveUserNotificationTypePreferenceResponse : BaseResponse<UserPreferenceData>
    {
        public UserPreferenceData GetData()
        {
            return Response.Data;
        }
    }
}
