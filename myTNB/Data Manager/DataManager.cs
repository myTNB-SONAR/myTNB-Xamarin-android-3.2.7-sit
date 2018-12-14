using System;
using System.Collections.Generic;
using Foundation;
using myTNB.Enums;
using myTNB.Model;
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

        //Notification Service Response
        public NotificationTypeResponseModel NotificationTypeResponse = new NotificationTypeResponseModel();
        public NotificationChannelResponseModel NotificationChannelResponse = new NotificationChannelResponseModel();
        public NotificationTypeResponseModel NotificationGeneralTypes = new NotificationTypeResponseModel();

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

        //Account Type
        public int CurrentSelectedAccountTypeIndex = 0;
        public int CurrentSelectedFeedAccountNoIndex = 0;
        public int CurrentSelectedStateForFeedbackIndex = 0;
        public int CurrentSelectedFeedbackTypeIndex = 0;

        //Find Us
        public int CurrentStoreTypeIndex = 0;
        public int PreviousStoreTypeIndex = 0;
        public bool IsSameStoreType = false;
        public string SelectedLocationTypeID = "all";
        public string SelectedLocationTypeTitle = "All";
        public bool isLocationSearch = false;

        //App launch
        public WebLinksResponseModel WebLinks = new WebLinksResponseModel();
        public LocationTypesResponseModel LocationTypes = new LocationTypesResponseModel();

        //Feedback
        public StatesForFeedbackResponseModel StatesForFeedBack = new StatesForFeedbackResponseModel();
        public FeedbackCategoryResponseModel FeedbackCategory = new FeedbackCategoryResponseModel();
        public OtherFeedbackTypeResponseModel OtherFeedbackType = new OtherFeedbackTypeResponseModel();
        public bool IsPreloginFeedback = false;

        //Promotion
        public bool IsPromotionFirstLoad = false;

        //Payment
        public bool IsPaymentDone = false;

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
            UserEntity uManager = new UserEntity();
            uManager.DeleteTable();
            UserAccountsEntity uaManager = new UserAccountsEntity();
            uaManager.DeleteTable();
            ChartEntity.DeleteTable();
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            sharedPreference.SetBool(false, "isLogin");
            sharedPreference.Synchronize();

            User = new UserModel();
            AccountRecordsList = new CustomerAccountRecordListModel();
            AccountsToBeAddedList = new CustomerAccountRecordListModel();
            AccountNumber = string.Empty;

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

            CurrentSelectedAccountTypeIndex = 0;
            CurrentSelectedFeedAccountNoIndex = 0;
            CurrentSelectedStateForFeedbackIndex = 0;
            CurrentSelectedFeedbackTypeIndex = 0;

            CurrentStoreTypeIndex = 0;
            PreviousStoreTypeIndex = 0;
            IsSameStoreType = false;
            SelectedLocationTypeID = "all";
            SelectedLocationTypeTitle = "All";
            isLocationSearch = false;

            IsPromotionFirstLoad = false;
        }

        /// <summary>
        /// Clears the credit card info.
        /// </summary>
        public void ClearCreditCardInfo()
        {
            CreditCardInfo = new CreditCardInfoModel();
        }

        /// <summary>
        /// Creates the usage history table.
        /// </summary>
        public void CreateUsageHistoryTable()
        {
            ChartEntity.CreateTable();
        }

        /// <summary>
        /// Saves to usage history.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <param name="key">Key.</param>
        public void SaveToUsageHistory(SmartChartDataModel model, string key)
        {
            if (model != null && !string.IsNullOrEmpty(key))
            {
                var jsonStr = JsonConvert.SerializeObject(model);
                var entity = new ChartEntity();
                entity.AccNum = key;
                entity.Data = jsonStr;
                entity.DateUpdated = DateHelper.FormatToUtc(DateTime.UtcNow);
                ChartEntity.InsertItem(entity);
            }
        }

        /// <summary>
        /// Gets the account usage history.
        /// </summary>
        /// <returns>The account usage history.</returns>
        /// <param name="key">Key.</param>
        /// <param name="lastUpdate">Last update.</param>
        public SmartChartDataModel GetAccountUsageHistory(string key, ref DateTime lastUpdate)
        {
            SmartChartDataModel model = null;

            if (!string.IsNullOrEmpty(key))
            {
                var entity = ChartEntity.GetItem(key);
                if (entity != null)
                {
                    model = JsonConvert.DeserializeObject<SmartChartDataModel>(entity.Data);
                    lastUpdate = DateTime.Parse(entity.DateUpdated).ToLocalTime();
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
    }
}
