using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.XEmailRegistrationForm.Activity.Models
{
    public class VerificationCodeResponse
    {
        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public VerificationCode verificationCode { get; set; }
    }
}