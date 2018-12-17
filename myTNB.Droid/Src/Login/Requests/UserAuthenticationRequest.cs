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
using Refit;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Login.Requests
{
    public class UserAuthenticationRequest : BaseRequest
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("clientType")]
        public string ClientType { get; set; }

        [JsonProperty("activeUserName")]
        public string ActiveUserName { get; set; }

        [JsonProperty("devicePlatform")]
        public string DevicePlatform { get; set; }

        [JsonProperty("deviceVersion")]
        public string DeviceVersion { get; set; }

        [JsonProperty("deviceCordova")]
        public string DeviceCordova { get; set; }

        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("fcmToken")]
        public string FcmToken { get; set; }


        public UserAuthenticationRequest(string apiKeyId) : base(apiKeyId)
        {

        }
    }
}