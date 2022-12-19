using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.AppLaunch.PostServices
{
    public class PostServicesRequest
    {
        [JsonProperty("usrInf")]
        public UserInfo UserInfo { set; get; }

        [JsonProperty("deviceInf")]
        public DeviceInfo DeviceInfo { set; get; }
    }

    public class UserInfo
    {
        [JsonProperty("eid")]
        public string Username { set; get; } = string.Empty;
        [JsonProperty("sspuid")]
        public string SSPUID { set; get; } = string.Empty;
        [JsonProperty("did")]
        public string DeviceID { set; get; } = string.Empty;
        [JsonProperty("ft")]
        public string FCMToken { set; get; } = string.Empty;
        [JsonProperty("lang")]
        public string Language { set; get; } = string.Empty;
        public string sec_auth_k1 { set; get; } = string.Empty;
        public string sec_auth_k2 { set; get; } = string.Empty;
        public string ses_param1 { set; get; } = string.Empty;
        public string ses_param2 { set; get; } = string.Empty;
    }

    public class DeviceInfo
    {
        public string DeviceId { set; get; } = string.Empty;
        public string AppVersion { set; get; } = string.Empty;
        public string OsType { set; get; } = string.Empty;
        public string OsVersion { set; get; } = string.Empty;
        public string DeviceDesc { set; get; } = string.Empty;
    }
}
