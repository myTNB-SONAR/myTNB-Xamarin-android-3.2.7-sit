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
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.ResetPassword.Model
{
    public class ResetPassword
    {
        //[JsonProperty(PropertyName = "__type")]
        //[AliasAs("__type")]
        //public string __type { get; set; }

        [JsonProperty(PropertyName = "userId")]
        [AliasAs("userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "displayName")]
        [AliasAs("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "userName")]
        [AliasAs("userName")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "email")]
        [AliasAs("email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "dateCreated")]
        [AliasAs("dateCreated")]
        public string DateCreated { get; set; }

        [JsonProperty(PropertyName = "lastLoginDate")]
        [AliasAs("lastLoginDate")]
        public string LastLoginDate { get; set; }

        //[JsonProperty(PropertyName = "isError")]
        //[AliasAs("isError")]
        //public Boolean IsError { get; set; }

        //[JsonProperty(PropertyName = "message")]
        //[AliasAs("message")]
        //public string Message { get; set; }


    }
}