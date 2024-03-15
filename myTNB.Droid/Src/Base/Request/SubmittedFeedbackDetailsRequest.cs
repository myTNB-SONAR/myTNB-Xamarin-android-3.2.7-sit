﻿using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.Base.Request
{
    public class SubmittedFeedbackDetailsRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("serviceReqNo")]
        public string ServiceReqNo { get; set; }
    }
}