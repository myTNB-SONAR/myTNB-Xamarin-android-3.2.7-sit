﻿using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.RegistrationForm.Models
{
    public class UserRegistrationResponse
    {
        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public UserRegistration userRegistration { get; set; }
    }
}