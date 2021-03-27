using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.XDetailRegistrationForm.Activity.Models
{
    public class VerificationCodeResponse
    {
        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public VerificationCode verificationCode { get; set; }
    }
}