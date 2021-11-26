using System.Collections.Generic;
using myTNB.Mobile.AWS;
using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.FeatureInfo
{
    public class PostSaveFeatureInfoRequest
    {
        [JsonProperty("accounts")]
        public List<ContractAccountModel> Accounts { set; get; }

        [JsonProperty("QueueTopic")]
        public string QueueTopic { set; get; }

        [JsonProperty("featureInfo")]
        public object FeatureInfo { set; get; }

        [JsonProperty("deviceInf")]
        public object DeviceInf { set; get; }

        [JsonProperty("usrInf")]
        public object UsrInf { set; get; }
    }
}