using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.RegistrationForm.Models
{
    public class UserCredentialsEntity
    {
        [JsonProperty("fullname")]
        public string Fullname { get; set; }

        [JsonProperty("icno")]
        public string ICNo { get; set; }

        [JsonProperty("mobile_no")]
        public string MobileNo { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("confirm_email")]
        public string ConfirmEmail { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("confirm_password")]
        public string ConfirmPassword { get; set; }

        [JsonProperty("idtype")]
        public string IdType { get; set; }
    }
}