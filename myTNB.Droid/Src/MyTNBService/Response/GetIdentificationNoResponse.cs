using System;
using Newtonsoft.Json;
using myTNB.Android.Src.Utils;
namespace myTNB.Android.Src.MyTNBService.Response
{
    public class GetIdentificationNoResponse 
    {
        [JsonProperty(PropertyName = "IdentificationNo")]
        public string IdentificationNo { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "isError")]
        public bool IsError { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

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

        [JsonProperty(PropertyName = "RefreshTitle")]
        public string RefreshTitle { get; set; }

        [JsonProperty(PropertyName = "RefreshMessage")]
        public string RefreshMessage { get; set; }

        [JsonProperty(PropertyName = "IsPayEnabled")]
        public bool IsPayEnabled { get; set; }


    }
}

