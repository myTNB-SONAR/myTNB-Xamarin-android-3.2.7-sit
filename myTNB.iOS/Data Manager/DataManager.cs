using System;
using System.Collections.Generic;
using Foundation;
using myTNB.Enums;
using myTNB.Model;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using Newtonsoft.Json;

namespace myTNB.DataManager
{
    public class DataManager
    {
        static DataManager sharedInstance;
        DataManager() { }

        //Registration Related Data
        public UserModel User = new UserModel();
        //Contains list of accounts
        public CustomerAccountRecordListModel AccountRecordsList = new CustomerAccountRecordListModel();
        public CustomerAccountRecordListModel AccountsToBeAddedList = new CustomerAccountRecordListModel();
        public string AccountNumber = string.Empty;
        public int AccountsAddedCount = 0;
        public bool SummaryNeedsRefresh = false;
        public int AccountRecordIndex = -1;
        public bool AccountIsSSMR = false;

        //Dashboard Home
        public List<HelpModel> HelpList = new List<HelpModel>();
        public List<ServiceItemModel> ServicesList = new List<ServiceItemModel>();
        public List<ServiceItemModel> ActiveServicesList = new List<ServiceItemModel>();
        public List<CustomerAccountRecordModel> CurrentAccountList = new List<CustomerAccountRecordModel>();
        public List<DueAmountDataModel> ActiveAccountList = new List<DueAmountDataModel>();
        public bool AccountListIsLoaded;
        public bool IsOnSearchMode;

        //Chart Related Data
        //Contains selected account from AccountRecordsList
        public CustomerAccountRecordModel SelectedAccount = new CustomerAccountRecordModel();
        //Contains details of SelectedAccount
        public BillingAccountDetailsDataModel BillingAccountDetails = new BillingAccountDetailsDataModel();
        public bool IsSameAccount = false;
        public bool IsBillUpdateNeeded = true;
        public int PreviousSelectedAccountIndex = 0;
        public int CurrentSelectedAccountIndex = 0;
        public ChartDataModelBase CurrentChart = new ChartDataModelBase();
        public bool IsMontView = true; //Default to Month View
        public ChartModeEnum CurrentChartMode = ChartModeEnum.Cost;
        public int CurrentChartIndex = 0; //Default to current chart
        public bool IsSmartMeterAvailable = false;
        /// <summary>
        /// Account Number as key and chart response data as value
        /// </summary>
        public Dictionary<string, ChartDataModelBase> AccountChartDictionary = new Dictionary<string, ChartDataModelBase>();
        //Credit Card
        public CreditCardInfoModel CreditCardInfo = new CreditCardInfoModel();
        public RegisteredCardsResponseModel RegisteredCards = new RegisteredCardsResponseModel();
        public CustomerAccountResponseModel CustomerAccounts = new CustomerAccountResponseModel();
        public int selectedTag = 0;

        //Login Response
        public List<UserEntity> UserEntity = new List<UserEntity>();

        //Notification
        public int CurrentSelectedNotificationTypeIndex = 0;
        public bool IsRegisteredForRemoteNotification = false;
        public string FCMToken = string.Empty;
        public bool IsFromPushNotification = false;
        public UserNotificationResponseModel UserNotificationResponse = new UserNotificationResponseModel();

        //Notification Service Response
        public NotificationTypeResponseModel NotificationTypeResponse = new NotificationTypeResponseModel();
        public NotificationChannelResponseModel NotificationChannelResponse = new NotificationChannelResponseModel();
        public List<NotificationPreferenceModel> NotificationGeneralTypes = new List<NotificationPreferenceModel>();

        //User Notification
        public List<UserNotificationDataModel> UserNotifications = new List<UserNotificationDataModel>();
        public bool HasNewNotification = false;
        public bool NotificationNeedsUpdate = false;
        public bool IsNotificationDeleted = false;

        //Device
        public string UDID = string.Empty;

        //Account Updates
        public bool IsMobileNumberUpdated = false;
        public bool IsNickNameUpdated = false;
        public bool IsAccountDeleted = false;
        public bool IsPasswordUpdated = false;
        public List<string> AccountsDeleted = new List<string>();

        //Account Type
        public int CurrentSelectedAccountTypeIndex = 0;
        public int CurrentSelectedFeedAccountNoIndex = 0;
        public int CurrentSelectedStateForFeedbackIndex = -1;
        public int CurrentSelectedFeedbackTypeIndex = 0;

        //Find Us
        public int CurrentStoreTypeIndex = 0;
        public int PreviousStoreTypeIndex = 0;
        public bool IsSameStoreType = false;
        public string SelectedLocationTypeID = "all";
        public string SelectedLocationTypeTitle = "All";
        public bool isLocationSearch = false;

        //App launch
        public List<WebLinksDataModel> WebLinks = new List<WebLinksDataModel>();
        public List<LocationTypeDataModel> LocationTypes = new List<LocationTypeDataModel>();
        public List<DowntimeDataModel> SystemStatus = new List<DowntimeDataModel>();
        public string LatestAppVersion;
        public bool IsBcrmAvailable = true;
        public bool IsPaymentFPXAvailable = true;
        public bool IsPaymentCreditCardAvailable = true;

        //Feedback
        public List<StatesForFeedbackDataModel> StatesForFeedBack = new List<StatesForFeedbackDataModel>();
        public List<FeedbackCategoryDataModel> FeedbackCategory = new List<FeedbackCategoryDataModel>();
        public List<OtherFeedbackTypeDataModel> OtherFeedbackType = new List<OtherFeedbackTypeDataModel>();
        public bool IsPreloginFeedback = false;

        //Promotion
        public bool IsPromotionFirstLoad = false;

        //Payment
        private List<string> AccountNumbersForPaymentList;

        //Account Related
        public InstallationDetailDataModel InstallationDetails = new InstallationDetailDataModel();
        public bool AccountIsActive = false;

        //Language
        public Dictionary<string, string> CommonI18NDictionary;
        public Dictionary<string, string> HintI18NDictionary;
        public Dictionary<string, string> ErrorI18NDictionary;

        //ImageSize
        public string ImageSize = string.Empty;

        public static DataManager SharedInstance
        {
            get
            {
                if (sharedInstance == null)
                {
                    sharedInstance = new DataManager();
                }
                return sharedInstance;
            }
        }

        /// <summary>
        /// Clears the login state and reset to default.
        /// </summary>
        public void ClearLoginState()
        {
            AccountUsageCache.ClearCache();
            AccountUsageSmartCache.ClearCache();
            UserEntity uManager = new UserEntity();
            uManager.DeleteTable();
            UserAccountsEntity uaManager = new UserAccountsEntity();
            uaManager.DeleteTable();
            BillHistoryEntity.DeleteTable();
            BillingAccountEntity.DeleteTable();
            ChartEntity.DeleteTable();
            DueEntity.DeleteTable();
            PaymentHistoryEntity.DeleteTable();
            PromotionsEntity.DeleteTable();
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            sharedPreference.SetBool(false, TNBGlobal.PreferenceKeys.LoginState);
            sharedPreference.SetString("", "SiteCorePromotionTimeStamp");
            sharedPreference.SetBool(false, TNBGlobal.PreferenceKeys.PhoneVerification);
            sharedPreference.Synchronize();

            User = new UserModel();
            AccountRecordsList = new CustomerAccountRecordListModel();
            AccountsToBeAddedList = new CustomerAccountRecordListModel();
            AccountNumber = string.Empty;
            AccountsAddedCount = 0;

            HelpList = new List<HelpModel>();
            ServicesList = new List<ServiceItemModel>();
            ActiveServicesList = new List<ServiceItemModel>();
            CurrentAccountList = new List<CustomerAccountRecordModel>();
            ActiveAccountList = new List<DueAmountDataModel>();
            AccountListIsLoaded = false;
            IsOnSearchMode = false;

            SelectedAccount = new CustomerAccountRecordModel();
            BillingAccountDetails = new BillingAccountDetailsDataModel();
            IsSameAccount = false;
            IsBillUpdateNeeded = true;
            PreviousSelectedAccountIndex = 0;
            CurrentSelectedAccountIndex = 0;
            CurrentChart = new ChartDataModelBase();
            IsMontView = true; //Default to Month View
            CurrentChartMode = ChartModeEnum.Cost;
            CurrentChartIndex = 0; //Default to current chart

            AccountChartDictionary?.Clear();
            AccountChartDictionary = new Dictionary<string, ChartDataModelBase>();
            CreditCardInfo = new CreditCardInfoModel();
            RegisteredCards = new RegisteredCardsResponseModel();
            CustomerAccounts = new CustomerAccountResponseModel();
            selectedTag = 0;

            UserEntity = new List<UserEntity>();

            CurrentSelectedNotificationTypeIndex = 0;
            IsRegisteredForRemoteNotification = false;
            IsFromPushNotification = false;

            UserNotifications = new List<UserNotificationDataModel>();
            HasNewNotification = false;
            NotificationNeedsUpdate = false;
            IsNotificationDeleted = false;

            IsMobileNumberUpdated = false;
            IsNickNameUpdated = false;
            IsAccountDeleted = false;
            IsPasswordUpdated = false;
            AccountsDeleted?.Clear();

            CurrentSelectedAccountTypeIndex = 0;
            CurrentSelectedFeedAccountNoIndex = 0;
            CurrentSelectedStateForFeedbackIndex = -1;
            CurrentSelectedFeedbackTypeIndex = 0;

            CurrentStoreTypeIndex = 0;
            PreviousStoreTypeIndex = 0;
            IsSameStoreType = false;
            SelectedLocationTypeID = "all";
            SelectedLocationTypeTitle = "All";
            isLocationSearch = false;

            IsPromotionFirstLoad = false;

            //Account Related
            InstallationDetails = new InstallationDetailDataModel();
            AccountIsActive = false;

            //ResetAmountDues
            AmountDueCache.Reset();

            //Reset SSMR Onboarding
            SSMRAccounts.IsHideOnboarding = false;
            SSMRActivityInfoCache.IsPhotoToolTipDisplayed = false;
        }

        /// <summary>
        /// Refreshs the data from account update.
        /// </summary>
        public void RefreshDataFromAccountUpdate(bool fromUpdateNickName = false)
        {
            UserAccountsEntity uaManager = new UserAccountsEntity();
            AccountRecordsList = uaManager.GetCustomerAccountRecordList();
            string currentSelectedAccountNum = SelectedAccount.accNum;
            int selectedAccountIndex = AccountRecordsList.d.FindIndex(x => x.accNum == currentSelectedAccountNum);

            if (selectedAccountIndex > -1)
            {
                CurrentSelectedAccountIndex = selectedAccountIndex;
                IsSameAccount = (fromUpdateNickName) ? false : true;
            }
            else
            {
                CurrentSelectedAccountIndex = 0;
                IsSameAccount = false;
            }
            if (AccountRecordsList?.d.Count > 0)
            {
                SelectedAccount = AccountRecordsList.d[CurrentSelectedAccountIndex];
            }
            else
            {
                SelectedAccount = new CustomerAccountRecordModel();
            }
            SummaryNeedsRefresh = true;
        }

        /// <summary>
        /// Gets the accounts count.
        /// </summary>
        /// <returns>The accounts count.</returns>
        public int GetAccountsCount()
        {
            int recordCount = AccountRecordsList != null && AccountRecordsList.d != null
                                         ? AccountRecordsList.d.Count : 0;
            return recordCount;
        }

        /// <summary>
        /// Clears the credit card info.
        /// </summary>
        public void ClearCreditCardInfo()
        {
            CreditCardInfo = new CreditCardInfoModel();
        }

        /// <summary>
        /// Selects the account.
        /// </summary>
        /// <param name="accNum">Acc number.</param>
        public void SelectAccount(string accNum)
        {
            if (string.Compare(accNum, SelectedAccount?.accNum) != 0)
            {
                var index = AccountRecordsList?.d?.FindIndex(x => x.accNum == accNum) ?? -1;
                if (index > -1)
                {
                    PreviousSelectedAccountIndex = CurrentSelectedAccountIndex;

                    //var item = AccountRecordsList.d[index];
                    //AccountRecordsList.d.RemoveAt(index);
                    //AccountRecordsList.d.Insert(0, item);
                    //SelectedAccount = AccountRecordsList.d[0];
                    SelectedAccount = AccountRecordsList.d[index];
                    CurrentSelectedAccountIndex = index;
                    IsSameAccount = false;
                }

            }

        }

        /// <summary>
        /// Checks if the account nickname is unique.
        /// </summary>
        /// <returns><c>true</c>, if account nickname unique was ised, <c>false</c> otherwise.</returns>
        /// <param name="accountNickname">Account nickname.</param>
        public bool IsAccountNicknameUnique(string accountNickname)
        {
            accountNickname = accountNickname?.Trim();
            int index = AccountRecordsList?.d?.FindIndex(item => string.Compare(item.accountNickName?.Trim(), accountNickname) == 0) ?? -1;
            bool res = index < 0;

            if (res && AccountsToBeAddedList?.d?.Count > 0)
            {
                index = AccountsToBeAddedList?.d?.FindIndex(item => string.Compare(item.accountNickName?.Trim(), accountNickname) == 0) ?? -1;
                res = index < 0;
            }

            return res;
        }

        /// <summary>
        /// Checks if another account uses the same nickname apart from itself
        /// </summary>
        /// <returns><c>true</c>, if account nickname unique was ised, <c>false</c> otherwise.</returns>
        /// <param name="accountNickname">Account nickname.</param>
        /// <param name="accountNumber">Account number.</param>
        public bool IsAccountNicknameUnique(string accountNickname, string accountNumber)
        {

            accountNickname = accountNickname?.Trim();
            accountNumber = accountNumber?.Trim();
            int index = AccountRecordsList?.d?.FindIndex(item => string.Compare(item.accountNickName?.Trim(), accountNickname) == 0) ?? -1;
            bool res = index < 0;

            // different account number, but same nickname
            if (res && AccountsToBeAddedList?.d?.Count > 0)
            {
                index = AccountsToBeAddedList?.d?.FindIndex(item => string.Compare(item.accountNickName?.Trim(), accountNickname) == 0
                                                            && string.Compare(item.accNum?.Trim(), accountNumber) != 0)
                                              ?? -1;
                res = index < 0;
            }

            return res;
        }

        /// <summary>
        /// Sets the systems availability.
        /// </summary>
        public void SetSystemsAvailability()
        {
            var status = SystemStatus?.Find(x => x.SystemType == SystemEnum.BCRM);
            IsBcrmAvailable = status?.IsAvailable ?? true;

            status = SystemStatus?.Find(x => x.SystemType == SystemEnum.PaymentCreditCard);
            IsPaymentCreditCardAvailable = status?.IsAvailable ?? true;

            status = SystemStatus?.Find(x => x.SystemType == SystemEnum.PaymentFPX);
            IsPaymentFPXAvailable = status?.IsAvailable ?? true;
        }

        /// <summary>
        /// Indicates if logged in.
        /// </summary>
        /// <returns><c>true</c>, if logged in was ised, <c>false</c> otherwise.</returns>
        public bool IsLoggedIn()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var isLogin = sharedPreference.BoolForKey(TNBGlobal.PreferenceKeys.LoginState);
            return isLogin;
        }

        #region Usage History Chart

        /// <summary>
        /// Creates the usage history table.
        /// </summary>
        public void CreateUsageHistoryTable()
        {
            ChartEntity.CreateTable();
        }

        /// <summary>
        /// Saves smart chart to usage history.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <param name="key">Key.</param>
        public void SaveSmartChartToUsageHistory(SmartChartDataModel model, string key)
        {
            if (model != null && !string.IsNullOrEmpty(key))
            {
                var jsonStr = JsonConvert.SerializeObject(model);
                SaveToUsageHistory(jsonStr, key);
            }
        }

        /// <summary>
        /// Saves the chart to usage history.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <param name="key">Key.</param>
        public void SaveChartToUsageHistory(ChartDataModel model, string key)
        {
            if (model != null && !string.IsNullOrEmpty(key))
            {
                var jsonStr = JsonConvert.SerializeObject(model);
                SaveToUsageHistory(jsonStr, key);
            }
        }

        /// <summary>
        /// Saves to usage history.
        /// </summary>
        /// <param name="jsonStr">Json string.</param>
        /// <param name="key">Key.</param>
        private void SaveToUsageHistory(string jsonStr, string key)
        {
            if (!string.IsNullOrEmpty(jsonStr) && !string.IsNullOrEmpty(key))
            {
                var entity = new ChartEntity();
                entity.AccNum = key;
                entity.Data = jsonStr;
                entity.DateUpdated = DateHelper.FormatToUtc(DateTime.UtcNow);
                entity.IsRefreshNeeded = false;
                ChartEntity.InsertItem(entity);
            }
        }

        /// <summary>
        /// Gets the smart account usage history.
        /// </summary>
        /// <returns>The smart account usage history.</returns>
        /// <param name="key">Key.</param>
        /// <param name="lastUpdate">Last update.</param>
        /// <param name="isRefreshNeeded">If set to <c>true</c> refresh is needed.</param>
        public SmartChartDataModel GetSmartAccountUsageHistory(string key, ref DateTime lastUpdate, ref bool isRefreshNeeded)
        {
            SmartChartDataModel model = null;

            if (!string.IsNullOrEmpty(key))
            {
                var entity = ChartEntity.GetItem(key);
                if (entity != null)
                {
                    model = JsonConvert.DeserializeObject<SmartChartDataModel>(entity.Data);
                    if (!string.IsNullOrEmpty(entity.DateUpdated))
                    {
                        lastUpdate = DateTime.Parse(entity.DateUpdated, System.Globalization.CultureInfo.InvariantCulture).ToLocalTime();
                    }
                    isRefreshNeeded = entity.IsRefreshNeeded;
                }
            }

            return model;
        }

        /// <summary>
        /// Gets the account usage history.
        /// </summary>
        /// <returns>The account usage history.</returns>
        /// <param name="key">Key.</param>
        /// <param name="lastUpdate">Last update.</param>
        /// <param name="isRefreshNeeded">If set to <c>true</c> refresh is needed.</param>
        public ChartDataModel GetAccountUsageHistory(string key, ref DateTime lastUpdate, ref bool isRefreshNeeded)
        {
            ChartDataModel model = null;

            if (!string.IsNullOrEmpty(key))
            {
                var entity = ChartEntity.GetItem(key);
                if (entity != null)
                {
                    model = JsonConvert.DeserializeObject<ChartDataModel>(entity.Data);
                    if (!string.IsNullOrEmpty(entity.DateUpdated))
                    {
                        lastUpdate = DateTime.Parse(entity.DateUpdated, System.Globalization.CultureInfo.InvariantCulture).ToLocalTime();
                    }
                    isRefreshNeeded = entity.IsRefreshNeeded;
                }
            }

            return model;
        }

        /// <summary>
        /// Deletes the account usage history.
        /// </summary>
        /// <param name="key">Key.</param>
        public void DeleteAccountUsageHistory(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                ChartEntity.DeleteItem(key);
            }
        }

        /// <summary>
        /// Sets the chart refresh status.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="shouldRefresh">If set to <c>true</c> should refresh.</param>
        public void SetChartRefreshStatus(string key, bool shouldRefresh)
        {
            var entity = ChartEntity.GetItem(key);
            if (entity != null)
            {
                entity.IsRefreshNeeded = shouldRefresh;
                ChartEntity.UpdateItem(entity);
            }
        }

        #endregion

        /// <summary>
        /// Updates the promos database.
        /// </summary>
        /// <param name="promotions">Promotions.</param>
        public void UpdatePromosDb(List<PromotionsModelV2> promotions)
        {
            if (promotions == null)
                return;

            foreach (var promo in promotions)
            {
                var entity = promo.ToEntity();
                PromotionsEntity.UpdateItem(entity);
            }
        }

        #region Dues
        /// <summary>
        /// Gets the due.
        /// </summary>
        /// <returns>The due.</returns>
        /// <param name="key">Key.</param>
        public DueAmountDataModel GetDue(string key)
        {
            DueAmountDataModel model = null;
            if (!string.IsNullOrEmpty(key))
            {
                model = AmountDueCache.GetDues(key);
            }
            return model;
        }

        /// <summary>
        /// Saves the dues.
        /// </summary>
        /// <param name="accountDues">Account dues.</param>
        public void SaveDues(List<DueAmountDataModel> accountDues)
        {
            foreach (var item in accountDues)
            {
                AmountDueCache.SaveDues(item);
            }
        }

        /// <summary>
        /// Saves the due.
        /// </summary>
        /// <param name="item">Item.</param>
        public void SaveDue(DueAmountDataModel item)
        {
            AmountDueCache.SaveDues(item);
        }

        /// <summary>
        /// Updates the due account nickname.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="newName">New name.</param>
        public void UpdateDueAccountNickname(string key, string newName)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(newName))
            {
                AmountDueCache.UpdateNickname(key, newName);
            }
        }

        public void UpdateDueIsSSMR(string key, string flag)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(flag))
            {
                AmountDueCache.UpdateIsSSMR(key, flag);
            }
        }

        /// <summary>
        /// Deletes the due.
        /// </summary>
        /// <param name="key">Key.</param>
        public void DeleteDue(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                AmountDueCache.DeleteDue(key);
            }
        }

        #endregion

        #region Billing Account Details
        /// <summary>
        /// Creates the billing accounts table.
        /// </summary>
        public void CreateBillingAccountsTable()
        {
            BillingAccountEntity.CreateTable();
        }

        /// <summary>
        /// Saves to billing accounts.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <param name="key">Key.</param>
        public void SaveToBillingAccounts(BillingAccountDetailsDataModel model, string key)
        {
            if (model != null && !string.IsNullOrEmpty(key))
            {
                var jsonStr = JsonConvert.SerializeObject(model);
                if (!string.IsNullOrEmpty(jsonStr))
                {
                    var entity = new BillingAccountEntity();
                    entity.AccNum = key;
                    entity.Data = jsonStr;
                    entity.DateUpdated = DateHelper.FormatToUtc(DateTime.UtcNow);
                    entity.IsRefreshNeeded = false;
                    BillingAccountEntity.InsertItem(entity);
                }
            }
        }

        /// <summary>
        /// Gets the details from billing account.
        /// </summary>
        /// <returns>The details from billing account.</returns>
        /// <param name="key">Key.</param>
        /// <param name="lastUpdate">Last update.</param>
        /// <param name="isRefreshNeeded">If set to <c>true</c> is refresh needed.</param>
        public BillingAccountDetailsDataModel GetDetailsFromBillingAccount(string key, ref DateTime lastUpdate, ref bool isRefreshNeeded)
        {
            BillingAccountDetailsDataModel model = null;

            if (!string.IsNullOrEmpty(key))
            {
                var entity = BillingAccountEntity.GetItem(key);
                if (entity != null)
                {
                    model = JsonConvert.DeserializeObject<BillingAccountDetailsDataModel>(entity.Data);
                    if (!string.IsNullOrEmpty(entity.DateUpdated))
                    {
                        lastUpdate = DateTime.Parse(entity.DateUpdated, System.Globalization.CultureInfo.InvariantCulture).ToLocalTime();
                    }
                    isRefreshNeeded = entity.IsRefreshNeeded;
                }
            }

            return model;
        }

        /// <summary>
        /// Gets the cached billing account details.
        /// </summary>
        /// <returns>The cached billing account details.</returns>
        /// <param name="accountNum">Account number.</param>
        public BillingAccountDetailsDataModel GetCachedBillingAccountDetails(string accountNum)
        {
            DateTime lastUpdate = default(DateTime);
            bool isRefreshNeeded = default(bool);
            var model = GetDetailsFromBillingAccount(accountNum, ref lastUpdate, ref isRefreshNeeded);

            if (model != null && lastUpdate.Date == DateTime.Today && !isRefreshNeeded)
            {
                return model;
            }
            return null;
        }

        /// <summary>
        /// Sets the billing refresh status.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="shouldRefresh">If set to <c>true</c> should refresh.</param>
        public void SetBillingRefreshStatus(string key, bool shouldRefresh)
        {
            var entity = BillingAccountEntity.GetItem(key);
            if (entity != null)
            {
                entity.IsRefreshNeeded = shouldRefresh;
                BillingAccountEntity.UpdateItem(entity);
            }
        }

        /// <summary>
        /// Deletes the details from billing account.
        /// </summary>
        /// <param name="key">Key.</param>
        public void DeleteDetailsFromBillingAccount(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                BillingAccountEntity.DeleteItem(key);
            }
        }

        #endregion

        #region Billing History

        /// <summary>
        /// Creates the bill history table.
        /// </summary>
        public void CreateBillHistoryTable()
        {
            BillHistoryEntity.CreateTable();
        }

        /// <summary>
        /// Saves to bill history.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <param name="key">Key.</param>
        public void SaveToBillHistory(BillHistoryModel model, string key)
        {
            if (model != null && !string.IsNullOrEmpty(key))
            {
                var jsonStr = JsonConvert.SerializeObject(model);
                if (!string.IsNullOrEmpty(jsonStr))
                {
                    var entity = new BillHistoryEntity();
                    entity.AccNum = key;
                    entity.Data = jsonStr;
                    entity.DateUpdated = DateHelper.FormatToUtc(DateTime.UtcNow);
                    entity.IsRefreshNeeded = false;
                    BillHistoryEntity.InsertItem(entity);
                }
            }
        }

        /// <summary>
        /// Gets the details from bill history.
        /// </summary>
        /// <returns>The details from bill history.</returns>
        /// <param name="key">Key.</param>
        /// <param name="lastUpdate">Last update.</param>
        /// <param name="isRefreshNeeded">If set to <c>true</c> is refresh needed.</param>
        public BillHistoryModel GetDetailsFromBillHistory(string key, ref DateTime lastUpdate, ref bool isRefreshNeeded)
        {
            BillHistoryModel model = null;

            if (!string.IsNullOrEmpty(key))
            {
                var entity = BillHistoryEntity.GetItem(key);
                if (entity != null)
                {
                    model = JsonConvert.DeserializeObject<BillHistoryModel>(entity.Data);
                    if (!string.IsNullOrEmpty(entity.DateUpdated))
                    {
                        lastUpdate = DateTime.Parse(entity.DateUpdated, System.Globalization.CultureInfo.InvariantCulture).ToLocalTime();
                    }
                    isRefreshNeeded = entity.IsRefreshNeeded;
                }
            }

            return model;
        }

        /// <summary>
        /// Gets the cached bill history.
        /// </summary>
        /// <returns>The cached bill history.</returns>
        /// <param name="accountNum">Account number.</param>
        public BillHistoryModel GetCachedBillHistory(string accountNum)
        {
            DateTime lastUpdate = default(DateTime);
            bool isRefreshNeeded = default(bool);
            var model = GetDetailsFromBillHistory(accountNum, ref lastUpdate, ref isRefreshNeeded);

            if (model != null && lastUpdate.Date == DateTime.Today && !isRefreshNeeded)
            {
                return model;
            }
            return null;
        }

        /// <summary>
        /// Sets the bill history refresh status.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="shouldRefresh">If set to <c>true</c> should refresh.</param>
        public void SetBillHistoryRefreshStatus(string key, bool shouldRefresh)
        {
            var entity = BillHistoryEntity.GetItem(key);
            if (entity != null)
            {
                entity.IsRefreshNeeded = shouldRefresh;
                BillHistoryEntity.UpdateItem(entity);
            }
        }

        /// <summary>
        /// Deletes the details from bill history.
        /// </summary>
        /// <param name="key">Key.</param>
        public void DeleteDetailsFromBillHistory(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                BillHistoryEntity.DeleteItem(key);
            }
        }

        #endregion

        #region Payment History

        /// <summary>
        /// Creates the payment history table.
        /// </summary>
        public void CreatePaymentHistoryTable()
        {
            PaymentHistoryEntity.CreateTable();
        }

        /// <summary>
        /// Saves to payment history.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <param name="key">Key.</param>
        public void SaveToPaymentHistory(PaymentHistoryModel model, string key)
        {
            if (model != null && !string.IsNullOrEmpty(key))
            {
                var jsonStr = JsonConvert.SerializeObject(model);
                if (!string.IsNullOrEmpty(jsonStr))
                {
                    var entity = new PaymentHistoryEntity();
                    entity.AccNum = key;
                    entity.Data = jsonStr;
                    entity.DateUpdated = DateHelper.FormatToUtc(DateTime.UtcNow);
                    entity.IsRefreshNeeded = false;
                    PaymentHistoryEntity.InsertItem(entity);
                }
            }
        }

        /// <summary>
        /// Gets the details from payment history.
        /// </summary>
        /// <returns>The details from payment history.</returns>
        /// <param name="key">Key.</param>
        /// <param name="lastUpdate">Last update.</param>
        /// <param name="isRefreshNeeded">If set to <c>true</c> is refresh needed.</param>
        public PaymentHistoryModel GetDetailsFromPaymentHistory(string key, ref DateTime lastUpdate, ref bool isRefreshNeeded)
        {
            PaymentHistoryModel model = null;

            if (!string.IsNullOrEmpty(key))
            {
                var entity = PaymentHistoryEntity.GetItem(key);
                if (entity != null)
                {
                    model = JsonConvert.DeserializeObject<PaymentHistoryModel>(entity.Data);
                    if (!string.IsNullOrEmpty(entity.DateUpdated))
                    {
                        lastUpdate = DateTime.Parse(entity.DateUpdated, System.Globalization.CultureInfo.InvariantCulture).ToLocalTime();
                    }
                    isRefreshNeeded = entity.IsRefreshNeeded;
                }
            }

            return model;
        }

        /// <summary>
        /// Gets the cached payment history.
        /// </summary>
        /// <returns>The cached payment history.</returns>
        /// <param name="accountNum">Account number.</param>
        public PaymentHistoryModel GetCachedPaymentHistory(string accountNum)
        {
            DateTime lastUpdate = default(DateTime);
            bool isRefreshNeeded = default(bool);
            var model = GetDetailsFromPaymentHistory(accountNum, ref lastUpdate, ref isRefreshNeeded);

            if (model != null && lastUpdate.Date == DateTime.Today && !isRefreshNeeded)
            {
                return model;
            }
            return null;
        }

        /// <summary>
        /// Sets the payment history refresh status.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="shouldRefresh">If set to <c>true</c> should refresh.</param>
        public void SetPaymentHistoryRefreshStatus(string key, bool shouldRefresh)
        {
            var entity = PaymentHistoryEntity.GetItem(key);
            if (entity != null)
            {
                entity.IsRefreshNeeded = shouldRefresh;
                PaymentHistoryEntity.UpdateItem(entity);
            }
        }

        /// <summary>
        /// Deletes the details from payment history.
        /// </summary>
        /// <param name="key">Key.</param>
        public void DeleteDetailsFromPaymentHistory(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                PaymentHistoryEntity.DeleteItem(key);
            }
        }

        /// <summary>
        /// Sets the account number for payment.
        /// </summary>
        /// <param name="accountNumber">Account number.</param>
        public void SetAccountNumberForPayment(string accountNumber)
        {
            if (AccountNumbersForPaymentList == null)
            {
                AccountNumbersForPaymentList = new List<string>();
            }
            AccountNumbersForPaymentList.Add(accountNumber);
        }

        /// <summary>
        /// Ises the paid account number.
        /// </summary>
        /// <returns><c>true</c>, if paid account number was ised, <c>false</c> otherwise.</returns>
        /// <param name="accountNumber">Account number.</param>
        public bool IsPaidAccountNumber(string accountNumber)
        {
            if (AccountNumbersForPaymentList == null)
            {
                return false;
            }
            return AccountNumbersForPaymentList.Contains(accountNumber);
        }

        /// <summary>
        /// Clears the paid list.
        /// </summary>
        public void ClearPaidList()
        {
            if (AccountNumbersForPaymentList != null)
            {
                AccountNumbersForPaymentList.Clear();
            }
        }

        #endregion


    }
}
