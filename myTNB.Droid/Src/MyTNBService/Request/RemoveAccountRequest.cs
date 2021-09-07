using myTNB.Mobile;
using System;
using System.Collections.Generic;
using static myTNB.Mobile.FeatureInfoClass;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class RemoveAccountRequest : BaseRequest
    {
        public string accNum;
        public bool isTaggedSmartMeter;
        public DeviceInfoRequest deviceInf;
        public List<FeatureInfo> featureInfo;

        public RemoveAccountRequest(string accNum, bool tagSM)
        {
            deviceInf = new DeviceInfoRequest();
            this.accNum = accNum;
            this.isTaggedSmartMeter = tagSM;
            this.featureInfo = FeatureInfoManager.Instance.GetFeatureInfo();
        }
    }
}
