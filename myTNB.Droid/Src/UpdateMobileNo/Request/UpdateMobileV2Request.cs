using Newtonsoft.Json;

namespace myTNB.Android.Src.UpdateMobileNo.Request
{
    public class UpdateMobileV2Request
    {
        //{
        //	"apiKeyID"       : "9515F2FA-C267-42C9-8087-FABA77CB84DF"
        //	"sspUserId"      : "",
        //	"email"	         : "montecillodavid.acn@gmail.com",
        //	"oldPhoneNumber" : "",
        //	"newPhoneNumber" : ""
        //}
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("sspUserId")]
        public string UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("oldPhoneNumber")]
        public string OldPhoneNumber { get; set; }

        [JsonProperty("newPhoneNumber")]
        public string NewPhoneNumber { get; set; }

        [JsonProperty("token")]
        public string token { get; set; }
    }
}