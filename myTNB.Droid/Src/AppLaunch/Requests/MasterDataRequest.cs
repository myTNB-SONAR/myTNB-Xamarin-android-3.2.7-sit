using myTNB.Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB.Android.Src.AppLaunch.Models
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
            public int OsType { get; set; }

            [JsonProperty("OsVersion")]
            public string OsVersion { get; set; }

            [JsonProperty("DeviceDesc")]
            public string DeviceDesc { get; set; }

            [JsonProperty("VersionCode")]
            public string VersionCode { get; set; }

        }
    }
}