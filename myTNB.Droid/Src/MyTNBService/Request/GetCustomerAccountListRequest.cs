using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class GetCustomerAccountListRequest : BaseRequest
    {
        public string LastSyncDateTime;
        public DeviceInfoRequest deviceInf;
        public List<Login.Models.FeatureInfo> featureInfo;

        public GetCustomerAccountListRequest()
        {
            LastSyncDateTime = "2021-08-01 23:55:55";
            deviceInf = new DeviceInfoRequest();
            featureInfo = new List<Login.Models.FeatureInfo>();
        }
    }
}
