using System;
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
                TariffLegendList = data.TariffBlocksLegend;
                ByMonthDateRange = data.ByMonth[0].Range; // TO DO: Need to revisit when the response data structure has changed
                ByMonthUsage = data.ByMonth[0].Months;// TO DO: Need to revisit when the response data structure has changed

                SaveToCache(accountNumber, data);
            }
            else
            {
                RefreshDataModel = response?.d ?? new AccountUsageSmartResponseDataModel();
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
        private static readonly string AccountDataPrefix = "AccountUsageSmartDataCache_{0}";
        private static readonly string TimeStampPrefix = "AccountUsageSmartTimeStampCache_{0}";
        private static readonly string DateFormat = "yyyy-MM-dd";
        #region Smart Meter
        public static void SaveToCache(string accountNumber, AccountUsageSmartDataModel data)
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

        public static AccountUsageSmartDataModel GetCachedData(string accountNumber)
        {
            if (!string.IsNullOrEmpty(accountNumber) && !string.IsNullOrWhiteSpace(accountNumber))
            {
                string key = string.Format(AccountDataPrefix, accountNumber);
                NSUserDefaults userDefaults = NSUserDefaults.StandardUserDefaults;
                string jsonData = userDefaults.StringForKey(key);
                if (!string.IsNullOrEmpty(jsonData) && !string.IsNullOrWhiteSpace(jsonData))
                {
                    AccountUsageSmartDataModel data = JsonConvert.DeserializeObject<AccountUsageSmartDataModel>(jsonData);
                    if (data != null)
                    {
                        TariffLegendList = data.TariffBlocksLegend;
                        ByMonthDateRange = data.ByMonth[0].Range; // TO DO: Need to revisit when the response data structure has changed
                        ByMonthUsage = data.ByMonth[0].Months;// TO DO: Need to revisit when the response data structure has changed
                        return data;
                    }
                }
            }
            return new AccountUsageSmartDataModel();
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
                    AccountUsageSmartDataModel cachedData = GetCachedData(accountNumber);
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
