using Newtonsoft.Json;

namespace myTNB.Android.Src.UpdateMobileNo.Request
{
    public class SendUpdatePhoneTokenSMSRequest
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

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("clientType")]
        public string ClientType { get; set; }

        [JsonProperty("activeUserName")]
        public string ActiveUserName { get; set; }

        [JsonProperty("devicePlatform")]
        public string DevicePlatform { get; set; }

        [JsonProperty("deviceVersion")]
        public string DeviceVersion { get; set; }

        [JsonProperty("deviceCordova")]
        public string DeviceCordova { get; set; }

        [JsonProperty("username")]
        public string username { get; set; }

        [JsonProperty("userEmail")]
        public string userEmail { get; set; }

        [JsonProperty("sspUserId")]
        public string sspUserId { get; set; }

        [JsonProperty("mobileNo")]
        public string mobileNo { get; set; }

    }
}