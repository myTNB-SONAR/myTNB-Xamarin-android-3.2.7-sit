using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class SaveUserNotificationTypePreferenceRequest : BaseRequest
    {
        public string id, notificationTypeId, isOpted;
        public DeviceInfoRequest deviceInf;

        public SaveUserNotificationTypePreferenceRequest(string id, string notificationTypeId, string isOpted)
        {
            this.id = id;
            this.notificationTypeId = notificationTypeId;
            this.isOpted = isOpted;
            deviceInf = new DeviceInfoRequest();
        }
    }
}
