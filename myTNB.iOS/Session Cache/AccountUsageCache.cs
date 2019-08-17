using System;
using System.Collections.Generic;
using myTNB.Model.Usage;

namespace myTNB
{
    public sealed class AccountUsageCache
    {
        private static readonly Lazy<AccountUsageCache> lazy = new Lazy<AccountUsageCache>(() => new AccountUsageCache());
        public static AccountUsageCache Instance { get { return lazy.Value; } }

        private static List<LegendItemModel> TariffLegendList = new List<LegendItemModel>();

        public static void AddTariffLegendList(AccountUsageResponseModel response)
        {
            if (TariffLegendList == null)
            {
                TariffLegendList = new List<LegendItemModel>();
            }
            if (response != null &&
                response.d != null &&
                response.d.data != null &&
                response.d.data.TariffBlocksLegend != null &&
                response.d.data.TariffBlocksLegend.Count > 0)
            {
                TariffLegendList = response.d.data.TariffBlocksLegend;
            }
        }

        public static void ClearTariffLegendList()
        {
            TariffLegendList.Clear();
        }

        public static List<LegendItemModel> GetTariffLegendList()
        {
            if (TariffLegendList != null)
            {
                return TariffLegendList;
            }
            return new List<LegendItemModel>();
        }
    }
}
