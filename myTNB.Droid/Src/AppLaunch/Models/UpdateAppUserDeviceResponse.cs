using System;
using Newtonsoft.Json;

namespace myTNB_Android.Src.AppLaunch.Models
{
    public class UpdateAppUserDeviceResponse
    {
        [JsonProperty("d")]
        public UpdateAppUserDeviceData Data { get; set; }

        public class UpdateAppUserDeviceData
        {
            [JsonProperty("__type")]
            public string Type { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("isError")]
            public bool IsError { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("errorCode")]
            public string ErrorCode { get; set; }

            [JsonProperty("errorMessage")]
            public string ErrorMessage { get; set; }

            [JsonProperty("displayMessage")]
            public string DisplayMessage { get; set; } = string.Empty;

            [JsonProperty("displayType")]
            public string DisplayType { get; set; }

            [JsonProperty("displayTitle")]
            public string DisplayTitle { get; set; }

            [JsonProperty("refreshTitle")]
            public string RefreshTitle { get; set; }

            [JsonProperty("refreshMessage")]
            public string RefreshMessage { get; set; }

            [JsonProperty("refreshBtnText")]
            public string RefreshBtnText { get; set; }

            [JsonProperty("isPayEnabled")]
            public bool IsPayEnabled { get; set; } = true;
        }
    }
}