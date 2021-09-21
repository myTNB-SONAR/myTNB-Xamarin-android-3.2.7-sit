using System;
using System.Collections.Generic;
using static myTNB.Mobile.FeatureInfoClass;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class RemoveAccountRequestNew : BaseRequestV2
    {
        public string accNum;
        public DeviceInfoRequest deviceInf;
        public List<FeatureInfo> featureInfo;
        public bool isTaggedSmartMeter;

        public RemoveAccountRequestNew(string accNum)
        {
            deviceInf = new DeviceInfoRequest();
            this.accNum = accNum;
            featureInfo = new List<FeatureInfo>();
            isTaggedSmartMeter = false;
        }
    }
}
