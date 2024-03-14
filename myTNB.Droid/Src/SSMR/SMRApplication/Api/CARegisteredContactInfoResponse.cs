using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Android.Src.SSMR.SMRApplication.Api
{
    public class CARegisteredContactInfoResponse
    {
        [JsonProperty(PropertyName = "d")]
        public AccountResponseData Data { get; set; }

        public class AccountResponseData
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "data")]
            public AccountDetails AccountDetailsData { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "ErrorCode")]
            public string ErrorCode { get; set; }

            [JsonProperty(PropertyName = "ErrorMessage")]
            public string ErrorMessage { get; set; }

            [JsonProperty(PropertyName = "DisplayMessage")]
            public string DisplayMessage { get; set; }

            [JsonProperty(PropertyName = "DisplayTitle")]
            public string DisplayTitle { get; set; }

            [JsonProperty(PropertyName = "DisplayType")]
            public string DisplayType { get; set; }

            [JsonProperty(PropertyName = "RefreshBtnText")]
            public string RefreshBtnText { get; set; }
        }

        public class AccountDetails
        {
            [JsonProperty(PropertyName = "Email")]
            public string Email { get; set; }

            [JsonProperty(PropertyName = "Mobile")]
            public string Mobile { get; set; }

            [JsonProperty(PropertyName = "isAllowEdit")]
            public bool isAllowEdit { get; set; }
        }
    }
}
