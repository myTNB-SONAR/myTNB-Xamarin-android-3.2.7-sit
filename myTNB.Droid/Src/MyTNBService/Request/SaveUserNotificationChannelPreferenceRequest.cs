using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class SaveUserNotificationChannelPreferenceRequest : BaseRequest
    {
        public string id, channelTypeId, isOpted;

        public SaveUserNotificationChannelPreferenceRequest(string id, string channelTypeId, string isOpted)
        {
            this.id = id;
            this.channelTypeId = channelTypeId;
            this.isOpted = isOpted;
        }
    }
}
