using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public class BaseRequest
    {
        [JsonProperty("usrInf")]
        public object UserInfo { get; set; }
        [JsonProperty("deviceInf")]
        public object DeviceInfo { get; set; }
    }
}