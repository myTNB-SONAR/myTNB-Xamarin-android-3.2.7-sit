﻿using Newtonsoft.Json;

namespace myTNB_Android.Src.AppLaunch.Requests
{
    public class WeblinkRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }
    }
}