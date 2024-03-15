﻿using Newtonsoft.Json;
using Refit;

namespace myTNB.AndroidApp.Src.XDetailRegistrationForm.Models
{
    public class UserRegistrationResponse
    {
        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public UserRegistration userRegistration { get; set; }
    }
}