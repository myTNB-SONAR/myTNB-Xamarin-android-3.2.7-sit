using System;
using System.Collections.Generic;
using Force.DeepCloner;
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

        public static void SetData(AccountUsageResponseModel response)
        {
            if (response != null && response.d != null
                && response.d.IsSuccess && response.d.data != null)
            {
                AccountUsageDataModel data = response.d.data.DeepClone();
                TariffLegendList = data.TariffBlocksLegend;
                ByMonthDateRange = data.ByMonth.Range;
                ByMonthUsage = data.ByMonth.Months;
            }
        }

        #region Usage Graph
        private static string _byMonthDateRange { set; get; } = string.Empty;
        private static List<MonthItemModel> _byMonthUsage = new List<MonthItemModel>();
        public static string ByMonthDateRange
        {
            private set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value)) { value = string.Empty; }
                _byMonthDateRange = value;
            }
            get { return _byMonthDateRange ?? string.Empty; }
        }

        public static List<MonthItemModel> ByMonthUsage
        {
            private set
            {
                if (value == null)
                {
                    value = new List<MonthItemModel>();
                }
                _byMonthUsage = value;
            }
            get { return _byMonthUsage != null ? _byMonthUsage : new List<MonthItemModel>(); }
        }
        #endregion
    }
}
