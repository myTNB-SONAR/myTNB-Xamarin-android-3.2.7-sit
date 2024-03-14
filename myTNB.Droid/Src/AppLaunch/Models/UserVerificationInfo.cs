using Newtonsoft.Json;

namespace myTNB.Android.Src.AppLaunch.Models
{
    public class UserVerificationInfo
    {
        [JsonProperty("Email")]
        public bool Email { get; set; }
    }
}