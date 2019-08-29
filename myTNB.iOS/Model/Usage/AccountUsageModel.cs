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
    }

    public class AccountUsageDataModel
    {
        public ByMonthModel ByMonth { set; get; }
        public List<LegendItemModel> TariffBlocksLegend { set; get; }
    }
}
