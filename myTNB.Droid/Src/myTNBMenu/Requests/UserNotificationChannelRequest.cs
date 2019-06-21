﻿using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Requests
{
    public class UserNotificationChannelRequest
    {

        [JsonProperty("apiKeyID")]
        [AliasAs("apiKeyID")]
        public string ApiKeyID { get; set; }

        [JsonProperty("email")]
        [AliasAs("email")]
        public string Email { get; set; }

    }
}