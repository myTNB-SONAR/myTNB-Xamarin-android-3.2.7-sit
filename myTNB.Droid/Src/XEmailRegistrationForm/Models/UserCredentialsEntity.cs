using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.XEmailRegistrationForm.Models
{
    public class UserCredentialsEntity
    {
       

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }


    }
}