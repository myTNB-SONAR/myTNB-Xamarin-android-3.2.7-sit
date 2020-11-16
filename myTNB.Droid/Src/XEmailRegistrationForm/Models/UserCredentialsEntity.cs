using Newtonsoft.Json;

namespace myTNB_Android.Src.XEmailRegistrationForm.Models
{
    public class UserCredentialsEntity
    {
       

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }


    }
}