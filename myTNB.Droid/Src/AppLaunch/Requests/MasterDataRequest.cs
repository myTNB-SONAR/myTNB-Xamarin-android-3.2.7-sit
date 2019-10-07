using myTNB_Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB_Android.Src.AppLaunch.Models
{
    public class MasterDataRequest
    {
        [JsonProperty("usrInf")]
        public UserInterface usrInf { get; set; }

        [JsonProperty("deviceInf")]
        public DeviceInterface deviceInf { get; set; }

    

        public  class DeviceInterface
        {

            [JsonProperty("DeviceId")]
            public string DeviceId { get; set; }

            [JsonProperty("AppVersion")]
            public string AppVersion { get; set; }

            [JsonProperty("OsType")]
            public string OsType { get; set; }

            [JsonProperty("OsVersion")]
            public string OsVersion { get; set; }

            [JsonProperty("DeviceDesc")]
            public string DeviceDesc { get; set; }

        }
    }
}