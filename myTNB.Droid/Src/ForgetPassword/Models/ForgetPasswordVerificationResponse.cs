﻿using System;
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

namespace myTNB_Android.Src.ForgetPassword.Models
{
    public class ForgetPasswordVerificationResponse
    {
        [JsonProperty(PropertyName = "d")]
        public ForgetPassword response { get; set; }

    }
}