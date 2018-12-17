using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Models;
using Refit;
using myTNB_Android.Src.RegistrationForm.Models;

namespace myTNB_Android.Src.RegistrationForm.Requests
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