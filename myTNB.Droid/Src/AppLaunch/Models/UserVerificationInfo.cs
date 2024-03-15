using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.AppLaunch.Models
{
    public class UserVerificationInfo
    {
        [JsonProperty("Email")]
        public bool Email { get; set; }
    }
}