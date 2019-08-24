using System.Collections.Generic;

namespace myTNB.Model.Usage
{
    public class AccountUsageSmartResponseModel
    {
        public AccountUsageSmartResponseDataModel d { set; get; }
    }

    public class AccountUsageSmartResponseDataModel : BaseModelRefresh
    {
        public AccountUsageSmartDataModel data { set; get; }
    }

    public class AccountUsageSmartDataModel
    {
        public string IsCO2Disabled { set; get; }
        //OtherUsageMetrics
        public List<ByMonthModel> ByMonth { set; get; }
        public List<ByDayModel> ByDay { set; get; }
        public List<LegendItemModel> TariffBlocksLegend { set; get; }
        public List<ToolTipItemModel> ToolTips;
    }

    public class ToolTipItemModel
    {
        public string Type { set; get; }
        public string Title { set; get; }
        public string Message { set; get; }
        public string SMLink { set; get; }
        public string SMBtnText { set; get; }
    }
}
