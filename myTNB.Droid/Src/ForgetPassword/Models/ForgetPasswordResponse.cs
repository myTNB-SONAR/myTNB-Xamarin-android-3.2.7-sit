using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.ForgetPassword.Models
{
    public class ForgetPasswordResponse
    {
        [JsonProperty(PropertyName = "d")]
        public ForgetPassword response { get; set; }

    }
}