using myTNB.Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB.Android.Src.AppLaunch.Models
{
    public class MasterDataResponse : BaseResponse<MasterData>
    {
        [JsonProperty("d")]
        public MasterDataObj Data { get; set; }

        public class MasterDataObj
        {
            [JsonProperty("__type")]
            public string Type { get; set; }

            [JsonProperty("ErrorCode")]
            public string ErrorCode { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("data")]
            public MasterData MasterData { get; set; }

            [JsonProperty("IsSMRApplyDisabled")]
            public bool IsSMRApplyDisabled { get; set; }

            [JsonProperty("IsEnergyTipsDisabled")]
            public bool IsEnergyTipsDisabled { get; set; }

            [JsonProperty("IsOCRDown")]
            public bool IsOCRDown { get; set; }

            [JsonProperty("IsSMRFeatureDisabled")]
            public bool IsSMRFeatureDisabled { get; set; }

            [JsonProperty("IsRewardsDisabled")]
            public bool IsRewardsDisabled { get; set; }

            [JsonProperty(PropertyName = "DisplayTitle")]
            public string DisplayTitle { get; set; }

            [JsonProperty(PropertyName = "DisplayMessage")]
            public string DisplayMessage { get; set; }
        }
    }
}