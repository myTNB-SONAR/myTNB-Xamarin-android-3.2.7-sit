using myTNB.Android.Src.Base.Models;
using Refit;

namespace myTNB.Android.Src.ForgetPassword.Requests
{
    public class ForgetPasswordVerificationCodeRequest : BaseRequest
    {


        [AliasAs("token")]
        public string token { get; set; }

        [AliasAs("username")]
        public string username { get; set; }

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

        public ForgetPasswordVerificationCodeRequest(string apiKeyId, string token, string username, string ipAddress, string clientType, string activeUserName, string devicePlatform, string deviceVersion, string deviceCordova) : base(apiKeyId)
        {
            this.token = token;
            this.username = username;
            this.ipAddress = ipAddress;
            this.clientType = clientType;
            this.activeUserName = activeUserName;
            this.devicePlatform = devicePlatform;
            this.deviceVersion = deviceVersion;
            this.deviceCordova = deviceCordova;
        }
    }
}