using Newtonsoft.Json;

namespace myTNB.Android.Src.ForgetPassword.Models
{
    public class ForgetPasswordResponse
    {
        [JsonProperty(PropertyName = "d")]
        public ForgetPassword response { get; set; }

    }
}