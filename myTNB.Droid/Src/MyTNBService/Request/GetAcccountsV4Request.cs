using myTNB.Mobile;
using System;
using System.Collections.Generic;
using static myTNB.Mobile.FeatureInfoClass;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class GetAcccountsV4Request : BaseRequestV4
    {
        public DeviceInfoRequest deviceInf;
        public List<FeatureInfo> featureInfo;

        public GetAcccountsV4Request()
        {
            deviceInf = new DeviceInfoRequest();
            this.featureInfo = FeatureInfoManager.Instance.GetFeatureInfo();
        }
    }
}
