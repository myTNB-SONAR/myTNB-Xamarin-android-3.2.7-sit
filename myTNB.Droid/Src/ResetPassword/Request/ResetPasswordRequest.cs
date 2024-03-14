using myTNB.Android.Src.Base.Models;
using Refit;

namespace myTNB.Android.Src.ResetPassword.Request
{
    public class ResetPasswordRequest : BaseRequest
    {

        [AliasAs("username")]
        public string username { get; set; }

        //[AliasAs("email")]
        //public string email { get; set; }

        [AliasAs("currentPassword")]
        public string currentPassword { get; set; }

        [AliasAs("newPassword")]
        public string newPassword { get; set; }

        [AliasAs("confirmNewPassword")]
        public string confirmNewPassword { get; set; }

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

        public ResetPasswordRequest(string apiKeyId, string username, string currentPassword, string newPassword, string confirmNewPassword, string ipAddress, string clientType, string activeUserName, string devicePlatform, string deviceVersion, string deviceCordova) : base(apiKeyId)
        {
            this.username = username;
            //this.email = email;
            this.currentPassword = currentPassword;
            this.newPassword = newPassword;
            this.confirmNewPassword = confirmNewPassword;
            this.ipAddress = ipAddress;
            this.clientType = clientType;
            this.activeUserName = activeUserName;
            this.devicePlatform = devicePlatform;
            this.deviceVersion = deviceVersion;
            this.deviceCordova = deviceCordova;
        }
    }
}