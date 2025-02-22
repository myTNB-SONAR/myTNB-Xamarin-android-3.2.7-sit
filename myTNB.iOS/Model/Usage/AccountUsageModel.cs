﻿using System.Collections.Generic;

namespace myTNB.Model.Usage
{
    public class AccountUsageResponseModel
    {
        public AccountUsageResponseDataModel d { set; get; }
    }

    public class AccountUsageResponseDataModel : BaseModelV2
    {
        public AccountUsageDataModel data { set; get; }
        public bool IsMonthlyTariffBlocksDisabled { get; set; }
        public bool IsMonthlyTariffBlocksUnavailable { get; set; }
        public bool IsDataEmpty
        {
            get
            {
                return ErrorCode == "7201";
            }
        }
    }

    public class AccountUsageDataModel
    {
        public ByMonthModel ByMonth { set; get; }
        public List<LegendItemModel> TariffBlocksLegend { set; get; }
    }
}
