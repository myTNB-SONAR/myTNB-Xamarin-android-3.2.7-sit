using myTNB.Android.Src.Base.Models;
using Refit;

namespace myTNB.Android.Src.ForgetPassword.Requests
{
    public class ForgetPasswordRequest : BaseRequest
    {

        [AliasAs("username")]
        public string username { get; set; }

        [AliasAs("userEmail")]
        public string userEmail { get; set; }

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

        public ForgetPasswordRequest(string apiKeyId, string username, string userEmail, string ipAddress, string clientType, string activeUserName, string devicePlatform, string deviceVersion, string deviceCordova) : base(apiKeyId)
        {
            this.apiKeyID = apiKeyID;
            this.username = username;
            this.userEmail = userEmail;
            this.ipAddress = ipAddress;
            this.clientType = clientType;
            this.activeUserName = activeUserName;
            this.devicePlatform = devicePlatform;
            this.deviceVersion = deviceVersion;
            this.deviceCordova = deviceCordova;
        }


    }
}