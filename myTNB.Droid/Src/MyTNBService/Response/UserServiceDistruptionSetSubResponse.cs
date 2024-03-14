using System;
using myTNB.Android.Src.Base.Response;
using Newtonsoft.Json;

namespace myTNB.Android.Src.MyTNBService.Response
{
    public class UserServiceDistruptionSetSubResponse
    {
        [JsonProperty(PropertyName = "d")]
        public ResponseD Response { get; set; }

        public class ResponseD
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "IsSMRApplyDisabled")]
            public bool IsSMRApplyDisabled { get; set; }

            [JsonProperty(PropertyName = "IsEnergyTipsDisabled")]
            public bool IsEnergyTipsDisabled { get; set; }

            [JsonProperty(PropertyName = "IsOCRDown")]
            public bool IsOCRDown { get; set; }

            [JsonProperty(PropertyName = "IsSMRFeatureDisabled")]
            public bool IsSMRFeatureDisabled { get; set; }

            [JsonProperty(PropertyName = "IsRewardsDisabled")]
            public bool IsRewardsDisabled { get; set; }

            [JsonProperty(PropertyName = "IsPayEnabled")]
            public bool IsPayEnabled { get; set; }

            [JsonProperty(PropertyName = "IsLargeFontDisabled")]
            public bool IsLargeFontDisabled { get; set; }

            [JsonProperty(PropertyName = "IsApplicationSyncAPIEnable")]
            public bool IsApplicationSyncAPIEnable { get; set; }

            [JsonProperty(PropertyName = "ApplicationSyncAPIInterval")]
            public double ApplicationSyncAPIInterval { get; set; }

            [JsonProperty(PropertyName = "IsAppointmentDisabled")]
            public bool IsAppointmentDisabled { get; set; }

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

            [JsonProperty(PropertyName = "DisplayType")]
            public string DisplayType { get; set; }

            [JsonProperty(PropertyName = "DisplayTitle")]
            public string DisplayTitle { get; set; }

            [JsonProperty(PropertyName = "RefreshTitle")]
            public string RefreshTitle { get; set; }

            [JsonProperty(PropertyName = "RefreshMessage")]
            public string RefreshMessage { get; set; }

            [JsonProperty(PropertyName = "RefreshBtnText")]
            public string RefreshBtnText { get; set; }

            [JsonProperty(PropertyName = "ShowWLTYPage")]
            public bool ShowWLTYPage { get; set; }

        }
    }
}
