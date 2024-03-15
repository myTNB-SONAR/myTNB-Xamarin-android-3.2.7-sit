using myTNB.Mobile;
using System;
using System.Collections.Generic;
using static myTNB.Mobile.FeatureInfoClass;

namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class SaveUserNotificationTypePreferenceRequest : BaseRequest
    {
        public string id, notificationTypeId, isOpted;
        public DeviceInfoRequest deviceInf;
        public List<FeatureInfo> featureInfo;

        public SaveUserNotificationTypePreferenceRequest(string id, string notificationTypeId, string isOpted)
        {
            this.id = id;
            this.notificationTypeId = notificationTypeId;
            this.isOpted = isOpted;
            deviceInf = new DeviceInfoRequest();
            this.featureInfo = FeatureInfoManager.Instance.GetFeatureInfo();
        }
    }
}
