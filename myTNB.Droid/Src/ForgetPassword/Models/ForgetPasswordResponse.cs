using Newtonsoft.Json;

namespace myTNB_Android.Src.ForgetPassword.Models
{
    public class ForgetPasswordResponse
    {
        [JsonProperty(PropertyName = "d")]
        public ForgetPassword response { get; set; }

    }
}