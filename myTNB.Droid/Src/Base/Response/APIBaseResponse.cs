using System;
using Newtonsoft.Json;

namespace myTNB.Android.Src.Base.Response
{
    public class APIBaseResponse
    {
        [JsonProperty(PropertyName = "__type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "ErrorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "ErrorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "DisplayMessage")]
        public string DisplayMessage { get; set; }

        [JsonProperty(PropertyName = "DisplayType")]
        public string DisplayType { get; set; }

        [JsonProperty(PropertyName = "DisplayTitle")]
        public string DisplayTitle { get; set; }
    }
}
