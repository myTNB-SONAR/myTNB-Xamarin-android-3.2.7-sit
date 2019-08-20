﻿using System;
using System.Collections.Generic;
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

        public static void SetData(string accountNumber, AccountUsageResponseModel response)
        {
            if (response != null && response.d != null
                && response.d.IsSuccess && response.d.data != null)
            {
                AccountUsageDataModel data = response.d.data.DeepClone();
                TariffLegendList = data.TariffBlocksLegend;
                ByMonthDateRange = data.ByMonth.Range;
                ByMonthUsage = data.ByMonth.Months;

                SaveToCache(accountNumber, data);
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
        public static void SaveToCache(string accountNumber, AccountUsageDataModel data)
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

        public static AccountUsageDataModel GetCachedData(string accountNumber)
        {
            if (!string.IsNullOrEmpty(accountNumber) && !string.IsNullOrWhiteSpace(accountNumber))
            {
                string key = string.Format(AccountDataPrefix, accountNumber);
                NSUserDefaults userDefaults = NSUserDefaults.StandardUserDefaults;
                string jsonData = userDefaults.StringForKey(key);
                if (!string.IsNullOrEmpty(jsonData) && !string.IsNullOrWhiteSpace(jsonData))
                {
                    AccountUsageDataModel data = JsonConvert.DeserializeObject<AccountUsageDataModel>(jsonData);
                    if (data != null)
                    {
                        TariffLegendList = data.TariffBlocksLegend;
                        ByMonthDateRange = data.ByMonth.Range;
                        ByMonthUsage = data.ByMonth.Months;
                        return data;
                    }
                }
            }
            return new AccountUsageDataModel();
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
                    AccountUsageDataModel cachedData = GetCachedData(accountNumber);
                    return newDay && cachedData.ByMonth != null;
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
        #endregion
    }
}
