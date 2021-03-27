using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.XEmailRegistrationForm.Models
{
    public class UserRegistrationResponse
    {
        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public UserRegistration userRegistration { get; set; }
    }
}