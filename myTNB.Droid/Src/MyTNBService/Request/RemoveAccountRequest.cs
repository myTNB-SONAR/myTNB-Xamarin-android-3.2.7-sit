using System;
using System.Collections.Generic;
using myTNB.Mobile;
using static myTNB.Mobile.FeatureInfoClass;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class RemoveAccountRequest : BaseRequestV4
    {
        public string accNum;
        public DeviceInfoRequest deviceInf;
        public List<FeatureInfo> featureInfo;
        public bool isTaggedSmartMeter;
        public bool IsInManageAccessList;
        public bool isOwner;

        public RemoveAccountRequest(string accNum, bool isTaggedSmartMeter, bool isOwner, bool isInManageAccessList)
        {
            deviceInf = new DeviceInfoRequest();
            this.accNum = accNum;
            this.featureInfo = FeatureInfoManager.Instance.GetFeatureInfo();
            this.isTaggedSmartMeter = isTaggedSmartMeter;
            this.IsInManageAccessList = isInManageAccessList;
            this.isOwner = isOwner;
        }
    }
}
