using System;
using System.Collections.Generic;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.ServiceDistruptionRating.Model;
using Newtonsoft.Json;
using static myTNB.Mobile.FeatureInfoClass;

namespace myTNB_Android.Src.ServiceDistruptionRating.Request
{
    public class UserServiceDistruptionSetSubRequest : BaseRequest
    {
        public string QueueTopic;
        public ServiceDisruptionInfo servicedisruptionInfo;
        public List<FeatureInfo> featureInfo;
        public DeviceInfoRequest deviceInf;

        public UserServiceDistruptionSetSubRequest(string QTopic, ServiceDisruptionInfo sdInfo)
        {
            QueueTopic = QTopic;
            servicedisruptionInfo = sdInfo;
            deviceInf = new DeviceInfoRequest();
            this.featureInfo = myTNB.Mobile.FeatureInfoManager.Instance.GetFeatureInfo();
        }
    }
}


