using System;
using System.Collections.Generic;
using myTNB.Mobile;
using static myTNB.Mobile.FeatureInfoClass;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class GetUserNotificationTypePreferencesRequest : BaseRequest
    {
        public List<FeatureInfo> featureInfo;

        public GetUserNotificationTypePreferencesRequest()
        {
            this.featureInfo = FeatureInfoManager.Instance.GetFeatureInfo();
        }
    }
}

