using Newtonsoft.Json;
using Refit;

namespace myTNB.AndroidApp.Src.myTNBMenu.Models
{
    public class SMUsageHistoryResponse
    {
        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public UsageHistoryD Data { get; set; }

        public class UsageHistoryD
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "ErrorCode")]
            [AliasAs("ErrorCode")]
            public string ErrorCode { get; set; }

            [JsonProperty(PropertyName = "RefreshTitle")]
            [AliasAs("RefreshTitle")]
            public string RefreshTitle { get; set; }

            [JsonProperty(PropertyName = "RefreshMessage")]
            [AliasAs("RefreshMessage")]
            public string RefreshMessage { get; set; }

            [JsonProperty(PropertyName = "RefreshBtnText")]
            [AliasAs("RefreshBtnText")]
            public string RefreshBtnText { get; set; }

            [JsonProperty(PropertyName = "IsMonthlyTariffBlocksDisabled")]
            [AliasAs("IsMonthlyTariffBlocksDisabled")]
            public bool IsMonthlyTariffBlocksDisabled { get; set; }

            [JsonProperty(PropertyName = "IsMonthlyTariffBlocksUnavailable")]
            [AliasAs("IsMonthlyTariffBlocksUnavailable")]
            public bool IsMonthlyTariffBlocksUnavailable { get; set; }

            [JsonProperty(PropertyName = "IsMDMSCurrentlyUnavailable")]
            [AliasAs("IsMDMSCurrentlyUnavailable")]
            public bool IsMDMSCurrentlyUnavailable { get; set; }

            [JsonProperty(PropertyName = "ErrorMessage")]
            [AliasAs("ErrorMessage")]
            public string ErrorMessage { get; set; }

            [JsonProperty(PropertyName = "DisplayMessage")]
            [AliasAs("DisplayMessage")]
            public string DisplayMessage { get; set; }

            [JsonProperty(PropertyName = "DisplayType")]
            [AliasAs("DisplayType")]
            public string DisplayType { get; set; }

            [JsonProperty(PropertyName = "DisplayTitle")]
            [AliasAs("DisplayTitle")]
            public string DisplayTitle { get; set; }

            [JsonProperty(PropertyName = "CTA")]
            [AliasAs("CTA")]
            public string CTA { get; set; }

            [JsonProperty(PropertyName = "data")]
            [AliasAs("data")]
            public SMUsageHistoryData SMUsageHistoryData { get; set; }
        }

    }
}