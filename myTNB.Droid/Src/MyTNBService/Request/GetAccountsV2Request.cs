using myTNB.Mobile;
using System;
using System.Collections.Generic;
using static myTNB.Mobile.FeatureInfoClass;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class GetAcccountsV2Request : BaseRequest
    {
        public DeviceInfoRequest deviceInf;
        public List<FeatureInfo> featureInfo;
        public GetAcccountsV2Request()
        {
            deviceInf = new DeviceInfoRequest();
            this.featureInfo = FeatureInfoManager.Instance.GetFeatureInfo();
        }
    }
}
