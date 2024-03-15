using System;
using System.Collections.Generic;
using myTNB.Mobile;
using static myTNB.Mobile.FeatureInfoClass;

namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class GetCustomerAccountListRequest : BaseRequestV4
    {
        public string LastSyncDateTime;
        public DeviceInfoRequest deviceInf;
        public List<FeatureInfo> featureInfo;

        public GetCustomerAccountListRequest()
        {
            LastSyncDateTime = "2021-08-01 23:55:55";
            deviceInf = new DeviceInfoRequest();
            this.featureInfo = FeatureInfoManager.Instance.GetFeatureInfo();
        }
    }
}
