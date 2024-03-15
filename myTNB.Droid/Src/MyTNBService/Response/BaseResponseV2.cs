using System;
using Newtonsoft.Json;
using myTNB.AndroidApp.Src.Utils;
using myTNB.AndroidApp.Src.MyTNBService.InterfaceAPI;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class BaseResponseV2<T>
    {
        [JsonProperty(PropertyName = "d")]
        public ResponseD Response { get; set; }

        public bool IsSuccessResponse()
        {
            bool IsSuccess = false;
            if (Response != null && Response.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
            {
                IsSuccess = true;
            }
            return IsSuccess;
        }
        public class ResponseD
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "data")]
            public T Data { get; set; }

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

            [JsonProperty(PropertyName = "IsFeedbackUpdateDetailDisabled")]
            public bool IsFeedbackUpdateDetailDisabled { get; set; }

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

        }
    }
}
