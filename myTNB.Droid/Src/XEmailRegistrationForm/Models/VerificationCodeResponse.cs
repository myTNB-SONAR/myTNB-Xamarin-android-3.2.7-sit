using Newtonsoft.Json;
using Refit;

namespace myTNB.AndroidApp.Src.XEmailRegistrationForm.Activity.Models
{
    public class VerificationCodeResponse
    {
        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public VerificationCode verificationCode { get; set; }
    }
}