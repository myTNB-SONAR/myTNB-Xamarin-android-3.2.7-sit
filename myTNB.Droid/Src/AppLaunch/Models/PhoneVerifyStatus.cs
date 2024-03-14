using Newtonsoft.Json;
using Refit;
using System;

namespace myTNB.Android.Src.AppLaunch.Models
{
    public class PhoneVerifyStatus
    {

        [JsonProperty(PropertyName = "IsPhoneVerified")]
        [AliasAs("IsPhoneVerified")]
        public Boolean IsPhoneVerified { get; set; }

        [JsonProperty(PropertyName = "PhoneNumber")]
        [AliasAs("PhoneNumber")]
        public string PhoneNumber { get; set; }

    }
}