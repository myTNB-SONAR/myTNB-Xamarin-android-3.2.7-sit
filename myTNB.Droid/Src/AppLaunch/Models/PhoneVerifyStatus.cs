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
using Refit;
using Newtonsoft.Json;

namespace myTNB_Android.Src.AppLaunch.Models
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