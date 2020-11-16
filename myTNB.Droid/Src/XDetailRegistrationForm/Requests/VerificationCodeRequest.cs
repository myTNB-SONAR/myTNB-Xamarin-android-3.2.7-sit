using myTNB_Android.Src.Base.Models;
using Refit;

namespace myTNB_Android.Src.XDetailRegistrationForm.Requests
{
    public class VerificationCodeRequest : BaseRequest
    {

        [AliasAs("userEmail")]
        public string userEmail { get; set; }

        [AliasAs("username")]
        public string username { get; set; }

        [AliasAs("mobileNo")]
        public string mobileNo { get; set; }

        [AliasAs("ipAddress")]
        public string ipAddress { get; set; }

        [AliasAs("clientType")]
        public string clientType { get; set; }

        [AliasAs("activeUserName")]
        public string activeUserName { get; set; }

        [AliasAs("devicePlatform")]
        public string devicePlatform { get; set; }

        [AliasAs("deviceVersion")]
        public string deviceVersion { get; set; }

        [AliasAs("deviceCordova")]
        public string deviceCordova { get; set; }

        public VerificationCodeRequest(string apiKeyId) : base(apiKeyId)
        {

        }


    }
}