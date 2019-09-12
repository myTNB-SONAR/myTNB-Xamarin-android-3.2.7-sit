﻿using System;
using System.Collections.Generic;
using Force.DeepCloner;
using Foundation;
using myTNB.Model.Usage;
using Newtonsoft.Json;

namespace myTNB
{
    public sealed class AccountUsageSmartCache
    {
        private static readonly Lazy<AccountUsageSmartCache> lazy = new Lazy<AccountUsageSmartCache>(() => new AccountUsageSmartCache());
        public static AccountUsageSmartCache Instance { get { return lazy.Value; } }

        private static List<LegendItemModel> TariffLegendList = new List<LegendItemModel>();
        private static OtherUsageMetricsModel UsageMetrics = new OtherUsageMetricsModel();
        private static List<ToolTipItemModel> Tooltips = new List<ToolTipItemModel>();
        public static AccountUsageSmartResponseDataModel RefreshDataModel;

        public static bool IsSuccess;

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

        public static OtherUsageMetricsModel GetUsageMetrics()
        {
            if (UsageMetrics != null)
            {
                return UsageMetrics;
            }
            return new OtherUsageMetricsModel();
        }

        public static List<ToolTipItemModel> GetTooltips()
        {
            if (Tooltips != null)
            {
                return Tooltips;
            }
            return new List<ToolTipItemModel>();
        }

        public static AccountUsageSmartResponseDataModel GetRefreshDataModel()
        {
            if (RefreshDataModel != null)
            {
                return RefreshDataModel;
            }
            return new AccountUsageSmartResponseDataModel();
        }

        public static void SetData(string accountNumber, AccountUsageSmartResponseModel response)
        {
            IsSuccess = response?.d?.IsSuccess ?? false;
            if (response != null && response.d != null
                && response.d.IsSuccess && response.d.data != null)
            {
                AccountUsageSmartDataModel data = response.d.data.DeepClone();
                RefreshDataModel = response.d.DeepClone();
                TariffLegendList = data.TariffBlocksLegend;
                ByMonthDateRange = data.ByMonth.Range;
                ByMonthUsage = data.ByMonth.Months;
                ByDayUsage = data.ByDay;
                UsageMetrics = data.OtherUsageMetrics;
                Tooltips = data.ToolTips;

                SaveToCache(accountNumber, RefreshDataModel);
            }
            else
            {
                RefreshDataModel = response?.d ?? new AccountUsageSmartResponseDataModel();
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
        private static List<ByDayModel> _byDayUsage = new List<ByDayModel>();
        public static List<ByDayModel> ByDayUsage
        {
            private set
            {
                if (value == null)
                {
                    value = new List<ByDayModel>();
                }
                _byDayUsage = value;
            }
            get { return _byDayUsage != null ? _byDayUsage : new List<ByDayModel>(); }
        }

        public static List<DayItemModel> FlatDays
        {
            get
            {
                if (ByDayUsage != null && ByDayUsage.Count > 0)
                {
                    List<DayItemModel> dayList = new List<DayItemModel>();
                    foreach (ByDayModel item in ByDayUsage)
                    {
                        if (item != null && item.Days != null)
                        {
                            dayList.AddRange(item.Days.DeepClone());
                        }
                    }
                    return dayList;
                }
                return new List<DayItemModel>();
            }
        }

        #endregion

        #region User Defaults
        private static readonly string AccountDataPrefix = "AccountUsageSmartDataCache_{0}";
        private static readonly string TimeStampPrefix = "AccountUsageSmartTimeStampCache_{0}";
        private static readonly string DateFormat = "yyyy-MM-dd";
        #region Smart Meter
        public static void SaveToCache(string accountNumber, AccountUsageSmartResponseDataModel data)
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

        public static AccountUsageSmartResponseDataModel GetCachedData(string accountNumber)
        {
            if (!string.IsNullOrEmpty(accountNumber) && !string.IsNullOrWhiteSpace(accountNumber))
            {
                string key = string.Format(AccountDataPrefix, accountNumber);
                NSUserDefaults userDefaults = NSUserDefaults.StandardUserDefaults;
                string jsonData = userDefaults.StringForKey(key);
                if (!string.IsNullOrEmpty(jsonData) && !string.IsNullOrWhiteSpace(jsonData))
                {
                    AccountUsageSmartResponseDataModel d = JsonConvert.DeserializeObject<AccountUsageSmartResponseDataModel>(jsonData);
                    if (d != null && d.data != null)
                    {
                        RefreshDataModel = d;
                        TariffLegendList = d.data.TariffBlocksLegend;
                        ByMonthDateRange = d.data.ByMonth.Range;
                        ByMonthUsage = d.data.ByMonth.Months;
                        UsageMetrics = d.data.OtherUsageMetrics;
                        Tooltips = d.data.ToolTips;
                        return d;
                    }
                }
            }
            return new AccountUsageSmartResponseDataModel();
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
                    AccountUsageSmartResponseDataModel cachedData = GetCachedData(accountNumber);
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
    }
}
