using System;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class AppLaunchMasterDataBaseResponse<T> : BaseResponse<T>
    {
        public class AppLaunchMasterResponseD : ResponseD
        {
            [JsonProperty(PropertyName = "IsSMRApplyDisabled")]
            public bool IsSMRApplyDisabled { get; set; }

            [JsonProperty(PropertyName = "IsEnergyTipsDisabled")]
            public bool IsEnergyTipsDisabled { get; set; }

            [JsonProperty(PropertyName = "IsOCRDown")]
            public bool IsOCRDown { get; set; }

            [JsonProperty(PropertyName = "IsSMRFeatureDisabled")]
            public bool IsSMRFeatureDisabled { get; set; }

            [JsonProperty(PropertyName = "IsFeedbackUpdateDetailDisabled")]
            public bool IsFeedbackUpdateDetailDisabled { get; set; }

            [JsonProperty(PropertyName = "IsRewardsDisabled")]
            public bool IsRewardsDisabled { get; set; }
        }
    }
}
