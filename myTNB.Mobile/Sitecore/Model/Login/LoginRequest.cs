using Newtonsoft.Json;

namespace myTNB.Mobile.Sitecore
{
    public class LoginRequest: BaseRequest
    {
        [JsonProperty("domain")]
        internal string Domain { set; get; }
        [JsonProperty("username")]
        internal string Username { set; get; }
        [JsonProperty("password")]
        internal string Password { set; get; }
    }
}