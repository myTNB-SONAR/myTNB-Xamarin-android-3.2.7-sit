using System.Collections.Generic;

namespace myTNB.Model.Usage
{
    public class AccountUsageResponseModel
    {
        public AccountUsageResponseDataModel d { set; get; }
    }

    public class AccountUsageResponseDataModel : BaseModelRefresh
    {
        public AccountUsageDataModel data { set; get; }
        public bool IsMonthlyTariffBlocksDisabled { get; set; }
        public bool IsMonthlyTariffBlocksUnavailable { get; set; }
    }

    public class AccountUsageDataModel
    {
        public ByMonthModel ByMonth { set; get; }
        public List<LegendItemModel> TariffBlocksLegend { set; get; }
    }
}
