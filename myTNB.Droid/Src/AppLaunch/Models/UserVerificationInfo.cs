using Newtonsoft.Json;

namespace myTNB_Android.Src.AppLaunch.Models
{
    public class UserVerificationInfo
    {
        [JsonProperty("Email")]
        public bool Email { get; set; }
    }
}