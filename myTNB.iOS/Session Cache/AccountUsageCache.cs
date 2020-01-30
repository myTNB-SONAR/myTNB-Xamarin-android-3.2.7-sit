using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Force.DeepCloner;
using Foundation;
using myTNB.Model.Usage;
using Newtonsoft.Json;

namespace myTNB
{
    public sealed class AccountUsageCache
    {
        private static readonly Lazy<AccountUsageCache> lazy = new Lazy<AccountUsageCache>(() => new AccountUsageCache());
        public static AccountUsageCache Instance { get { return lazy.Value; } }

        private static List<LegendItemModel> TariffLegendList = new List<LegendItemModel>();
        public static AccountUsageResponseDataModel RefreshDataModel;

        public static bool IsSuccess { set; get; }
        public static bool IsDataEmpty { set; get; }
        public static bool IsPlannedDownTime { set; get; }
        public static bool IsUnplannedDownTime { set; get; }

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

        public static AccountUsageResponseDataModel GetRefreshDataModel()
        {
            if (RefreshDataModel != null)
            {
                return RefreshDataModel;
            }
            return new AccountUsageResponseDataModel();
        }

        public static void SetData(string accountNumber, AccountUsageResponseModel response)
        {
            IsSuccess = response?.d?.IsSuccess ?? false;
            IsDataEmpty = response?.d?.IsDataEmpty ?? false;
            IsPlannedDownTime = response?.d?.IsPlannedDownTime ?? false;
            IsUnplannedDownTime = response?.d?.IsUnplannedDownTime ?? false;
            if (response != null && response.d != null
                && response.d.IsSuccess && response.d.data != null)
            {
                AccountUsageDataModel data = response.d.data.DeepClone();
                RefreshDataModel = response.d.DeepClone();
                TariffLegendList = data.TariffBlocksLegend;
                ByMonthDateRange = data.ByMonth.Range;
                ByMonthUsage = data.ByMonth.Months;

                SaveToCache(accountNumber, RefreshDataModel);
            }
            else
            {
                RefreshDataModel = response?.d ?? new AccountUsageResponseDataModel();
            }
        }

        public static bool IsMonthlyTariffDisable
        {
            get
            {
                if (RefreshDataModel != null)
                {
                    return RefreshDataModel.IsMonthlyTariffBlocksDisabled;
                }
                return true;
            }
        }

        public static bool IsMonthlyTariffUnavailable
        {
            get
            {
                if (RefreshDataModel != null)
                {
                    return RefreshDataModel.IsMonthlyTariffBlocksUnavailable;
                }
                return true;
            }
        }

        public static string EmptyDataMessage
        {
            get
            {
                if (RefreshDataModel != null)
                {
                    return RefreshDataModel.DisplayTitle;
                }
                return string.Empty;
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

        #region User Defaults
        private static readonly string AccountDataPrefix = "AccountUsageDataCache_{0}";
        private static readonly string TimeStampPrefix = "AccountUsageTimeStampCache_{0}";
        private static readonly string DateFormat = "yyyy-MM-dd";
        #region Normal and RE
        public static void SaveToCache(string accountNumber, AccountUsageResponseDataModel data)
        {
            if (!accountNumber.IsValid()) { return; }
            try
            {
                string jsonData = JsonConvert.SerializeObject(data);
                if (!string.IsNullOrEmpty(accountNumber) && !string.IsNullOrWhiteSpace(accountNumber)
                    && !string.IsNullOrEmpty(jsonData) && !string.IsNullOrWhiteSpace(jsonData))
                {
                    string accKey = string.Format(AccountDataPrefix, accountNumber);
                    NSUserDefaults userDefaults = NSUserDefaults.StandardUserDefaults;
                    userDefaults.SetString(jsonData, accKey);
                    userDefaults.Synchronize();

                    string tsKey = string.Format(TimeStampPrefix, accountNumber);
                    string dateTime = DateTime.Now.ToString(DateFormat);
                    userDefaults.SetString(dateTime, tsKey);
                    userDefaults.Synchronize();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("SaveToCache Error: " + e.Message);
            }
        }

        public static AccountUsageResponseDataModel GetCachedData(string accountNumber)
        {
            if (!string.IsNullOrEmpty(accountNumber) && !string.IsNullOrWhiteSpace(accountNumber))
            {
                string key = string.Format(AccountDataPrefix, accountNumber);
                NSUserDefaults userDefaults = NSUserDefaults.StandardUserDefaults;
                string jsonData = userDefaults.StringForKey(key);
                if (!string.IsNullOrEmpty(jsonData) && !string.IsNullOrWhiteSpace(jsonData))
                {
                    AccountUsageResponseDataModel d = JsonConvert.DeserializeObject<AccountUsageResponseDataModel>(jsonData);
                    if (d != null && d.data != null)
                    {
                        RefreshDataModel = d;
                        TariffLegendList = d.data.TariffBlocksLegend;
                        ByMonthDateRange = d.data.ByMonth.Range;
                        ByMonthUsage = d.data.ByMonth.Months;
                        return d;
                    }
                }
            }
            return new AccountUsageResponseDataModel();
        }

        public static bool IsRefreshNeeded(string accountNumber)
        {
            if (!string.IsNullOrEmpty(accountNumber) && !string.IsNullOrWhiteSpace(accountNumber))
            {
                string key = string.Format(TimeStampPrefix, accountNumber);
                NSUserDefaults userDefaults = NSUserDefaults.StandardUserDefaults;
                string timeStamp = userDefaults.StringForKey(key);
                if (!string.IsNullOrEmpty(timeStamp) && !string.IsNullOrWhiteSpace(timeStamp))
                {
                    bool newDay = IsNewDay(timeStamp);
                    AccountUsageResponseDataModel cachedData = GetCachedData(accountNumber);
                    return newDay && cachedData.data != null && cachedData.data.ByMonth != null;
                }
            }

            return true;
        }

        private static bool IsNewDay(string date)
        {
            DateTime currentDate = DateTime.Now;
            DateTime cachedDate = DateTime.Parse(date);
            return cachedDate.Date.AddDays(1) == currentDate.Date;
        }
        #endregion

        public static void ClearCache()
        {
            string dataKey = string.Format(AccountDataPrefix, string.Empty);
            string tStampKey = string.Format(TimeStampPrefix, string.Empty);
            NSUserDefaults userDefaults = NSUserDefaults.StandardUserDefaults;
            NSObject[] userDefaultKeys = userDefaults.ToDictionary().Keys;
            for (int i = 0; i < userDefaultKeys.Length; i++)
            {
                string key = userDefaultKeys[i].ToString();
                if (key.Contains(dataKey) || key.Contains(tStampKey))
                {
                    userDefaults.RemoveObject(key);
                }
            }
            userDefaults.Synchronize();
        }
        #endregion

        public static bool AreAllTariffEmpty
        {
            get
            {
                if (ByMonthUsage != null && ByMonthUsage.Count > 0)
                {
                    for (int i = 0; i < ByMonthUsage.Count; i++)
                    {
                        MonthItemModel monthUsage = ByMonthUsage[i];
                        if (monthUsage != null && monthUsage.tariffBlocks != null && monthUsage.tariffBlocks.Count > 0)
                        {
                            double amountTotal = monthUsage.tariffBlocks.Sum(x => x.Amount);
                            double usageTotal = monthUsage.tariffBlocks.Sum(x => x.Usage);
                            if (amountTotal > 0 && usageTotal > 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
        }
    }
}