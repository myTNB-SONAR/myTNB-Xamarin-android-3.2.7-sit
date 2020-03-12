﻿using System;
using myTNB_Android.Src.Base.Response;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class LanguagePreferenceResponse
    {
        [JsonProperty(PropertyName = "d")]
        public APIResponse Data { get; set; }

        public class APIResponse : APIBaseResponse
        {
            [JsonProperty(PropertyName = "data")]
            public LanguagePreferenceResponseData ResponseData { get; set; }
        }

        public class LanguagePreferenceResponseData
        {
            [JsonProperty(PropertyName = "lang")]
            public string lang { get; set; }
        }
    }
}
