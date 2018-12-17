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
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.RegistrationForm.Models
{
    public class UserRegistrationResponse 
    {
        [JsonProperty(PropertyName ="d")]
        [AliasAs("d")]
        public UserRegistration userRegistration { get; set; }
    }
}