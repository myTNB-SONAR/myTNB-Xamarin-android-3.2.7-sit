using myTNB_Android.Src.Base.Models;
using Refit;

namespace myTNB_Android.Src.XEmailRegistrationForm.Requests
{
    public class UserRegistrationRequest : BaseRequest
    {
        public UserRegistrationRequest(string apiKeyID) : base(apiKeyID)
        {

        }

        //"displayName": "tester7",
        // "username": "tester7",
        // "email": "tester7.tnb@gmail.com",
        // "token": "12010",
        // "password": "Abc123",
        // "confirmPassword": "Abc123"
        [AliasAs("displayName")]
        public string displayName { get; set; }

        [AliasAs("username")]
        public string username { get; set; }

        [AliasAs("email")]
        public string email { get; set; }

        [AliasAs("token")]
        public string token { get; set; }

        [AliasAs("password")]
        public string password { get; set; }

        [AliasAs("confirmPassword")]
        public string confirmPassword { get; set; }

        [AliasAs("icNo")]
        public string icNo { get; set; }

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


    }
}