using System;
using System.Collections.Generic;
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

        public RemoveAccountRequest(string accNum, bool tagSM, bool manageAccessList, bool isOwn)
        {
            deviceInf = new DeviceInfoRequest();
            this.accNum = accNum;
            featureInfo = new List<FeatureInfo>();
            isTaggedSmartMeter = tagSM;
            IsInManageAccessList = manageAccessList;
            isOwner = isOwn;
        }
    }
}
