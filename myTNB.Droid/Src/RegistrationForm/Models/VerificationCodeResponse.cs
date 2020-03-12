﻿using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.RegistrationForm.Activity.Models
{
    public class VerificationCodeResponse
    {
        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public VerificationCode verificationCode { get; set; }
    }
}