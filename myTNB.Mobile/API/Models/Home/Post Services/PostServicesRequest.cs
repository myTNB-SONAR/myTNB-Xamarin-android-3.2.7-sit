using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.Home.PostServices
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
        public string UserName { set; get; }
        [JsonProperty("sspuid")]
        public string UserID { set; get; }
        [JsonProperty("did")]
        public string DeviceID { set; get; }
        [JsonProperty("ft")]
        public string FCMToken { set; get; }
        [JsonProperty("lang")]
        public string Language { set; get; }
        public string sec_auth_k1 { set; get; }
        public string sec_auth_k2 { set; get; }
        public string ses_param1 { set; get; }
        public string ses_param2 { set; get; }
    }

    public class DeviceInfo
    {
        public string DeviceId { set; get; }
        public string AppVersion { set; get; }
        public string OsType { set; get; }
        public string OsVersion { set; get; }
        public string DeviceDesc { set; get; }
    }

    public class DeviceInfoExtra
    {
        public string DeviceId { set; get; }
        public string AppVersion { set; get; }
        public string OsType { set; get; }
        public string OsVersion { set; get; }
        public string DeviceDesc { set; get; }
        public string VersionCode { set; get; }
    }

    public class UserInfoExtra
    {
        [JsonProperty("eid")]
        public string UserName { set; get; }
        [JsonProperty("sspuid")]
        public string UserID { set; get; }
        [JsonProperty("did")]
        public string DeviceID { set; get; }
        [JsonProperty("ft")]
        public string FCMToken { set; get; }
        [JsonProperty("lang")]
        public string Language { set; get; }
        public string sec_auth_k1 { set; get; }
        public string sec_auth_k2 { set; get; }
        public string ses_param1 { set; get; }
        public string ses_param2 { set; get; }
        public bool IsWhiteList { set; get; }
    }
}