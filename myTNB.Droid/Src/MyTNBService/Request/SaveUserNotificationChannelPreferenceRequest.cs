using myTNB.Mobile;
using System;
using System.Collections.Generic;
using static myTNB.Mobile.FeatureInfoClass;

namespace myTNB.Android.Src.MyTNBService.Request
{
    public class SaveUserNotificationChannelPreferenceRequest : BaseRequest
    {
        public string id, channelTypeId, isOpted;
        public List<FeatureInfo> featureInfo;

        public SaveUserNotificationChannelPreferenceRequest(string id, string channelTypeId, string isOpted)
        {
            this.id = id;
            this.channelTypeId = channelTypeId;
            this.isOpted = isOpted;
            this.featureInfo = FeatureInfoManager.Instance.GetFeatureInfo();
        }
    }
}
