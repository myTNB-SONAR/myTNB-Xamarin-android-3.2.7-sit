using System;
using Newtonsoft.Json;

namespace myTNB.Android.Src.AppLaunch.Requests
{
    public class UpdateAppUserDeviceRequest
    {
        [JsonProperty("apiKeyID")]
        public string apiKeyID { get; set; }

        [JsonProperty("FCMToken")]
        public string FCMToken { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("AppVersion")]
        public string AppVersion { get; set; }

        [JsonProperty("OsType")]
        public string OsType { get; set; }

        [JsonProperty("OsVersion")]
        public string OsVersion { get; set; }

        [JsonProperty("DeviceIdOld")]
        public string DeviceIdOld { get; set; }

        [JsonProperty("DeviceIdNew")]
        public string DeviceIdNew { get; set; }

        public UpdateAppUserDeviceRequest()
        {

        }
    }
}
