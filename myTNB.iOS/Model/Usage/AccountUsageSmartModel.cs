using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Model.Usage
{
    public class AccountUsageSmartResponseModel
    {
        public AccountUsageSmartResponseDataModel d { set; get; }
    }

    public class AccountUsageSmartResponseDataModel : BaseModelRefresh
    {
        public AccountUsageSmartDataModel data { set; get; }
        public bool IsMonthlyTariffBlocksDisabled { get; set; }
        public bool IsMonthlyTariffBlocksUnavailable { get; set; }
        public bool IsMDMSCurrentlyUnavailable { get; set; }
        public bool IsUnplannedMDMSDown
        {
            get
            {
                return ErrorCode == StatusCodes.UnplannedMDMSDown;
            }
        }
        public bool IsPlannedMDMSDown
        {
            get
            {
                return ErrorCode == StatusCodes.PlannedMDMSDown;
            }
        }
        public bool IsMDMSDown
        {
            get
            {
                return IsUnplannedMDMSDown || IsPlannedMDMSDown;
            }
        }
        public bool IsDataEmpty
        {
            get
            {
                return ErrorCode == StatusCodes.EmptyData;
            }
        }
        public string CTA { set; get; }
    }

    public class AccountUsageSmartDataModel
    {
        public string IsCO2Disabled { set; get; }
        public OtherUsageMetricsModel OtherUsageMetrics { set; get; }
        public ByMonthModel ByMonth { set; get; }
        public List<ByDayModel> ByDay { set; get; }
        public List<LegendItemModel> TariffBlocksLegend { set; get; }
        public List<ToolTipItemModel> ToolTips;

        public string CurrentCycle { set; get; }
        public string StartDate { set; get; }
        public string MidDate { set; get; }
        public string EndDate { set; get; }
        public string DateRange { set; get; }
    }

    public class OtherUsageMetricsModel
    {
        public List<UsageCostItemModel> Usage { set; get; }
        public List<UsageCostItemModel> Cost { set; get; }
        public string CurrentCycleStartDate { set; get; }
    }

    public class UsageCostItemModel
    {
        public string Key { set; get; }
        public string SubTitle { set; get; }
        public string Title { set; get; }
        public string Value { set; get; }
        public string ValueIndicator { set; get; }
        public string ValueUnit { set; get; }
        [JsonIgnore]
        public UsageCostEnum UsageCostType
        {
            get
            {
                UsageCostEnum usageCostType = default(UsageCostEnum);

                if (!string.IsNullOrEmpty(Key))
                {
                    switch (Key)
                    {
                        case "CURRENTUSAGE":
                            usageCostType = UsageCostEnum.CURRENTUSAGE;
                            break;
                        case "AVERAGEUSAGE":
                            usageCostType = UsageCostEnum.AVERAGEUSAGE;
                            break;
                        case "CURRENTCOST":
                            usageCostType = UsageCostEnum.CURRENTCOST;
                            break;
                        case "PROJECTEDCOST":
                            usageCostType = UsageCostEnum.PROJECTEDCOST;
                            break;
                        case "ESTIMATEDREADING":
                            usageCostType = UsageCostEnum.ESTIMATEDREADING;
                            break;
                        default:
                            usageCostType = UsageCostEnum.None;
                            break;
                    }
                }
                return usageCostType;
            }
        }
        [JsonIgnore]
        public TrendEnum TrendType
        {
            get
            {
                TrendEnum trendType = default(TrendEnum);

                if (!string.IsNullOrEmpty(ValueIndicator))
                {
                    switch (ValueIndicator)
                    {
                        case "+":
                            trendType = TrendEnum.UP;
                            break;
                        case "-":
                            trendType = TrendEnum.DOWN;
                            break;
                        default:
                            trendType = TrendEnum.None;
                            break;
                    }
                }
                return trendType;
            }
        }
    }

    public class ToolTipItemModel
    {
        public string Type { set; get; }
        public string Title { set; get; }
        public List<string> Message { set; get; }
        public string SMLink { set; get; }
        public string SMBtnText { set; get; }
        [JsonIgnore]
        public UsageCostEnum UsageCostType
        {
            get
            {
                UsageCostEnum usageCostType = default(UsageCostEnum);

                if (!string.IsNullOrEmpty(Type))
                {
                    switch (Type)
                    {
                        case "CURRENTUSAGE":
                            usageCostType = UsageCostEnum.CURRENTUSAGE;
                            break;
                        case "AVERAGEUSAGE":
                            usageCostType = UsageCostEnum.AVERAGEUSAGE;
                            break;
                        case "CURRENTCOST":
                            usageCostType = UsageCostEnum.CURRENTCOST;
                            break;
                        case "PROJECTEDCOST":
                            usageCostType = UsageCostEnum.PROJECTEDCOST;
                            break;
                        case "ESTIMATEDREADING":
                            usageCostType = UsageCostEnum.ESTIMATEDREADING;
                            break;
                        default:
                            usageCostType = UsageCostEnum.None;
                            break;
                    }
                }
                return usageCostType;
            }
        }
    }

    public enum UsageCostEnum
    {
        None = 0,
        CURRENTUSAGE,
        AVERAGEUSAGE,
        CURRENTCOST,
        PROJECTEDCOST,
        ESTIMATEDREADING
    }

    public enum TrendEnum
    {
        None = 0,
        UP,
        DOWN
    }
}
