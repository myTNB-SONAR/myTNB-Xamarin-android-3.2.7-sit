﻿using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.AppLaunch.Requests
{
    public class WeblinkRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }
    }
}