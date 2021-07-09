using System.Collections.Generic;
using Android.App;
using Android.Content;
using Java.Lang;
using Java.Text;
using Java.Util;
using myTNB.Mobile;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Utils
{
    public sealed class UserSessions
    {
        private UserSessions() { }

        private static ISharedPreferences mPreferences;
        internal static ApplicationStatusNotificationModel ApplicationStatusNotification { private set; get; }
        internal static NotificationModel Notification { private set; get; }

        public static void SetCurrentImageCount(ISharedPreferences prefs, int count)
        {

            SimpleDateFormat simpleDateFormatter = new SimpleDateFormat("dd-MM-yyyy");
            Calendar calendarNow = Calendar.GetInstance(Locale.Default);
            Calendar calendarYesterday = calendarNow.Clone() as Calendar;
            calendarYesterday.Add(CalendarField.Date, -1);

            ISharedPreferencesEditor editor = prefs.Edit();
            editor.Remove(string.Format("{0}{1}", "currentImageCount", simpleDateFormatter.Format(calendarYesterday.TimeInMillis)));
            editor.PutInt(string.Format("{0}{1}", "currentImageCount", simpleDateFormatter.Format(calendarNow.TimeInMillis)), count);
            editor.Apply();
        }

        public static int GetCurrentImageCount(ISharedPreferences prefs)
        {

            SimpleDateFormat simpleDateFormatter = new SimpleDateFormat("dd-MM-yyyy");
            Calendar calendarNow = Calendar.GetInstance(Locale.Default);
            return prefs.GetInt(string.Format("{0}{1}", "currentImageCount", simpleDateFormatter.Format(calendarNow.TimeInMillis)), 0);
        }

        public static void SetHasNotification(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasNotification", true);
            editor.Apply();
        }

        public static bool HasNotification(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasNotification", false);
        }

        public static void SaveUserEmailNotification(ISharedPreferences prefs, string email)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("notificationEmail", email);
            editor.Apply();
        }

        public static string GetUserEmailNotification(ISharedPreferences prefs)
        {
            return prefs.GetString("notificationEmail", null);
        }

        public static void SaveNotificationType(ISharedPreferences prefs, string notifType)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("notificationType", notifType);
            editor.Apply();
        }

        public static string GetNotificationType(ISharedPreferences prefs)
        {
            return prefs.GetString("notificationType", null);
        }
        
        internal static void SetApplicationStatusNotification(string saveID
            , string applciationID
            , string applicationType
            , string system)
        {
            if (!string.IsNullOrEmpty(applicationType) && (!string.IsNullOrEmpty(saveID) || !string.IsNullOrEmpty(applciationID)))
            {
                ApplicationStatusNotification = new ApplicationStatusNotificationModel
                {
                    SaveApplicationID = saveID,
                    ApplicationID = applciationID,
                    ApplicationType = applicationType,
                    System = system
                };
            }
            else
            {
                ApplicationStatusNotification = null;
            }
        }

        public static void ClearNotification()
        {
            Notification = null;
        }

        internal static void SetNotification(
              string type
            , string requestTransID
            , string eventID)
        {
            if (!string.IsNullOrEmpty(type) && (!string.IsNullOrEmpty(requestTransID) || !string.IsNullOrEmpty(eventID)))
            {
                Notification = new NotificationModel
                {
                    Type = type,
                    RequestTransId = requestTransID,
                    EventId = eventID
                   
                };
            }
            else
            {
                Notification = null;
            }
        }

        public static void RemoveNotificationSession(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.Remove("hasNotification").Remove("notificationEmail").Apply();
        }

        public static bool HasSkipped(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasSkipped", false);
        }

        public static void DoSkipped(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasSkipped", true);
            editor.Apply();
        }

        public static bool HasUpdateSkipped(ISharedPreferences prefs)
        {
            return prefs.GetBoolean(Utility.GetAppUpdateId(), false);
        }

        public static void DoUpdateSkipped(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean(Utility.GetAppUpdateId(), true);
            editor.Apply();
        }

        public static bool HasSMROnboardingShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasSMROnboardingShown", false);
        }

        public static void DoSMROnboardingShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasSMROnboardingShown", true);
            editor.Apply();
        }

        public static bool HasPayBillShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasPayBillShown", false);
        }

        public static void DoPayBillShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasPayBillShown", true);
            editor.Apply();
        }

        public static bool HasViewBillShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasViewBillShown", false);
        }

        public static void DoViewBillShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasViewBillShown", true);
            editor.Apply();
        }

        public static void DoCleanUpdateReceiveCache(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasCleanUpdateReceiveCache", true);
            editor.Apply();
        }

        public static bool HasCleanUpdateReceiveCache(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasCleanUpdateReceiveCache", false);
        }

        public static bool HasRewardsShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasRewardsShown", false);
        }

        public static void DoRewardsShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasRewardsShown", true);
            editor.Apply();
        }

        public static bool HasRewardShown(ISharedPreferences prefs)
        {
            bool flag = prefs.GetBoolean("hasRewardShown", false);

            if (UserSessions.HasRewardsShown(prefs) && !flag)
            {
                DoRewardShown(prefs);
                flag = true;
            }
            return flag;
        }

        public static void DoRewardShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasRewardShown", true);
            editor.Apply();
        }

        public static bool HasRewardsDetailShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasRewardsDetailShown", false);
        }
        public static bool HasApplicationStatusShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasApplicationStatusShown", false);
        }
        public static void DoApplicationStatusShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasApplicationStatusShown", true);
            editor.Apply();
        }

        public static bool HasSmartMeterShown(ISharedPreferences prefs)             //energy budget
        {
            return prefs.GetBoolean("hasSmartMeterShown", false);
        }
        public static void DoSmartMeterShown(ISharedPreferences prefs)              //energy budget
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasSmartMeterShown", true);
            editor.Apply();
        }

        public static bool HasApplicationDetailShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasApplicationDetailShown", false);
        }
        public static void DoApplicationDetailShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasApplicationDetailShown", true);
            editor.Apply();
        }
        public static void DoRewardsDetailShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasRewardsDetailShown", true);
            editor.Apply();
        }

        public static bool HasWhatsNewShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasWhatsNewShown", false);
        }

        public static void DoWhatsNewShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasWhatsNewShown", true);
            editor.Apply();
        }

        public static bool HasWhatNewShown(ISharedPreferences prefs)
        {
            bool flag = prefs.GetBoolean("hasWhatNewShown", false);

            if (UserSessions.HasWhatsNewShown(prefs) && !flag)
            {
                DoWhatNewShown(prefs);
                flag = true;
            }
            return flag;
        }

        public static void DoWhatNewShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasWhatNewShown", true);
            editor.Apply();
        }

        public static bool HasHomeTutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasHomeTutorialShown", false);
        }

        public static void DoHomeTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasHomeTutorialShown", true);
            editor.Apply();
        }

        public static bool HasItemizedBillingNMSMTutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasItemizedBillingNMSMTutorialShown", false);
        }

        public static void DoItemizedBillingNMSMTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasItemizedBillingNMSMTutorialShown", true);
            editor.Apply();
        }

        public static bool HasItemizedBillingRETutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasItemizedBillingRETutorialShown", false);
        }

        public static void DoItemizedBillingRETutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasItemizedBillingRETutorialShown", true);
            editor.Apply();
        }

        public static bool HasItemizedBillingDetailTutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasItemizedBillingDetailTutorialShown", false);
        }

        public static void DoItemizedBillingDetailTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasItemizedBillingDetailTutorialShown", true);
            editor.Apply();
        }

        public static bool HasSMRMeterHistoryTutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasSMRMeterHistoryTutorialShown", false);
        }

        public static void DoSMRMeterHistoryTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasSMRMeterHistoryTutorialShown", true);
            editor.Apply();
        }

        public static bool HasSMRSubmitMeterTutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasSMRSubmitMeterTutorialShown", false);
        }

        public static void DoSMRSubmitMeterTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasSMRSubmitMeterTutorialShown", true);
            editor.Apply();
        }

        public static bool HasSMRDashboardTutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasSMRDashboardTutorialShown", false);
        }

        public static void DoSMRDashboardTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasSMRDashboardTutorialShown", true);
            editor.Apply();
        }

        internal static void DoFlagResetPassword(ISharedPreferences mSharedPref)
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.PutBoolean("resetPasswordEnabled", true);
            editor.Apply();
        }

        internal static void DoUnflagResetPassword(ISharedPreferences mSharedPref)
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.Remove("resetPasswordEnabled");
            editor.Apply();
        }

        internal static bool HasResetFlag(ISharedPreferences mSharedPref)
        {
            return mSharedPref.GetBoolean("resetPasswordEnabled", false);
        }
        [Deprecated]
        internal static void PersistPassword(ISharedPreferences mSharedPref, string password)
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.PutString("currentPassword", password);
            editor.Apply();
        }
        [Deprecated]
        internal static string GetPersistPassword(ISharedPreferences mSharedPref)
        {
            return mSharedPref.GetString("currentPassword", null);
        }

        internal static void RemovePersistPassword(ISharedPreferences mSharedPref)
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.Remove("currentPassword");
            editor.Apply();
        }

        internal static void SaveSelectedFeedback(ISharedPreferences mSharedPref, string largeJsonString)
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.PutString(Constants.SELECTED_FEEDBACK, largeJsonString);
            editor.Apply();
        }

        internal static void SaveAdapterType(ISharedPreferences mSharedPref, string largeJsonString)
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.PutString(Constants.ADAPTER_TYPE, largeJsonString);
            editor.Apply();
        }

        internal static string GetAdapterType(ISharedPreferences mSharePref)
        {
            return mSharePref.GetString(Constants.ADAPTER_TYPE, null);
        }

        internal static string GetSelectedFeedback(ISharedPreferences mSharePref)
        {
            return mSharePref.GetString(Constants.SELECTED_FEEDBACK, null);
        }

        internal static void UpdateDeviceId(ISharedPreferences mSharedPref)
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.PutBoolean("deviceIDUpdated", true);
            editor.Apply();
        }

        public static bool IsDeviceIdUpdated(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("deviceIDUpdated", false);
        }

        public static void SaveDeviceId(ISharedPreferences prefs, string deviceID)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("deviceID", deviceID);
            editor.Apply();
        }

        public static string GetDeviceId(ISharedPreferences prefs)
        {
            return prefs.GetString("deviceID", "");
        }

        public static void SaveFeedbackUpdateDetailDisabled(ISharedPreferences prefs, string data)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("IsFeedbackUpdateDetailDisabled", data);
            editor.Apply();
        }

        public static string GetFeedbackUpdateDetailDisabled(ISharedPreferences prefs)
        {
            return prefs.GetString("IsFeedbackUpdateDetailDisabled", null);
        }

        public static void SaveGetAccountIsExist(ISharedPreferences prefs, string data)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("IsAccountExist", data);
            editor.Apply();
        }

        public static string GetAccountIsExist(ISharedPreferences prefs)
        {
            return prefs.GetString("IsAccountExist", null);
        }

        public static void SaveLogoutFlag(ISharedPreferences prefs, bool flag)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("loggedOut", flag);
            editor.Apply();
        }

        public static bool GetLogoutFlag(ISharedPreferences preferences)
        {
            return preferences.GetBoolean("loggedOut", false);
        }

        public static void SavePhoneVerified(ISharedPreferences prefs, bool flag)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("phoneVerified", flag);
            editor.Apply();
        }

        public static bool GetPhoneVerifiedFlag(ISharedPreferences preferences)
        {
            return preferences.GetBoolean("phoneVerified", false);
        }

        public static void SaveUserEmail(ISharedPreferences prefs, string email)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("loginEmail", email);
            editor.Apply();
        }

        public static void SavePopUpCountEB(ISharedPreferences prefs, string count)    //count EB popup
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("popupEB", count);
            editor.Apply();
        }

        public static string GetSavePopUpCountEB(ISharedPreferences preferences)    //count EB popup
        {
            return preferences.GetString("popupEB", "");
        }

        public static int GetPrevAppVersionCode(ISharedPreferences preferences)
        {
            return preferences.GetInt("PREV_APP_VERSION_CODE", 0);
        }

        public static void SetAppVersionCode(ISharedPreferences preferences, int appVersionCode)
        {
            ISharedPreferencesEditor editor = preferences.Edit();
            editor.PutInt("PREV_APP_VERSION_CODE", appVersionCode);
            editor.Apply();
        }

        public static string GetUserEmail(ISharedPreferences preferences)
        {
            return preferences.GetString("loginEmail", "");
        }

        public static void SetSelectAccountList(List<CustomerBillingAccount> accountList)
        {
            if (accountList.Count > 0)
            {
                ISharedPreferencesEditor editor = mPreferences.Edit();
                string jsonAccountList = JsonConvert.SerializeObject(accountList);
                editor.PutString("SELECT_ACCOUNT_LIST", jsonAccountList);
                editor.Apply();
            }
        }

        public static List<CustomerBillingAccount> GetSelectAccountList()
        {
            string accountList = mPreferences.GetString("SELECT_ACCOUNT_LIST", null);
            List<CustomerBillingAccount> selectAccountList = new List<CustomerBillingAccount>();
            if (accountList != null)
            {
                selectAccountList = JsonConvert.DeserializeObject<List<CustomerBillingAccount>>(accountList);
            }
            return selectAccountList;
        }

        public static void SetSMRAccountList(List<SMRAccount> sMRAccounts)
        {
            ISharedPreferencesEditor editor = mPreferences.Edit();
            string jsonAccountList = JsonConvert.SerializeObject(sMRAccounts);
            editor.PutString("SMR_ACCOUNT_LIST", jsonAccountList);
            editor.Apply();
        }

        public static List<SMRAccount> GetSMRAccountList()
        {
            string accountList = mPreferences.GetString("SMR_ACCOUNT_LIST", null);
            List<SMRAccount> selectAccountList = new List<SMRAccount>();
            if (accountList != null)
            {
                selectAccountList = JsonConvert.DeserializeObject<List<SMRAccount>>(accountList);
            }
            return selectAccountList;
        }

        public static void EnergyBudget(List<SMRAccount> sMRAccounts)
        {
            ISharedPreferencesEditor editor = mPreferences.Edit();
            string jsonAccountList = JsonConvert.SerializeObject(sMRAccounts);
            editor.PutString("SMR_ACCOUNT_LIST_ENERGY_BUDGET", jsonAccountList);
            editor.Apply();
        }

        public static List<SMRAccount> GetEnergyBudgetList()
        {
            string accountList = mPreferences.GetString("SMR_ACCOUNT_LIST_ENERGY_BUDGET", null);
            List<SMRAccount> selectAccountList = new List<SMRAccount>();
            if (accountList != null)
            {
                selectAccountList = JsonConvert.DeserializeObject<List<SMRAccount>>(accountList);
            }
            return selectAccountList;
        }

        public static void DeleteEnergyBudgetList(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.Remove("SMR_ACCOUNT_LIST_ENERGY_BUDGET").Apply();
        }
    
        public static void SetSMREligibilityAccountList(List<SMRAccount> sMRAccounts)
        {
            ISharedPreferencesEditor editor = mPreferences.Edit();
            string jsonAccountList = JsonConvert.SerializeObject(sMRAccounts);
            editor.PutString("SMR_ELIGIBILITY_ACCOUNT_LIST", jsonAccountList);
            editor.Apply();
        }

        public static List<SMRAccount> GetSMREligibilityAccountList()
        {
            string accountList = mPreferences.GetString("SMR_ELIGIBILITY_ACCOUNT_LIST", null);
            List<SMRAccount> selectAccountList = new List<SMRAccount>();
            if (accountList != null)
            {
                selectAccountList = JsonConvert.DeserializeObject<List<SMRAccount>>(accountList);
            }
            return selectAccountList;
        }

        public static void SetRealSMREligibilityAccountList(List<SMRAccount> sMRAccounts)
        {
            ISharedPreferencesEditor editor = mPreferences.Edit();
            string jsonAccountList = JsonConvert.SerializeObject(sMRAccounts);
            editor.PutString("SMR_REAL_ELIGIBILITY_ACCOUNT_LIST", jsonAccountList);
            editor.Apply();
        }

        public static List<SMRAccount> GetRealSMREligibilityAccountList()
        {
            string accountList = mPreferences.GetString("SMR_REAL_ELIGIBILITY_ACCOUNT_LIST", null);
            List<SMRAccount> selectAccountList = null;
            if (accountList != null)
            {
                selectAccountList = new List<SMRAccount>();
                selectAccountList = JsonConvert.DeserializeObject<List<SMRAccount>>(accountList);
            }
            return selectAccountList;
        }

        public static void SetSharedPreference(ISharedPreferences preferences)
        {
            mPreferences = preferences;
        }

        public static void RemoveSessionData()
        {
            SetSMRAccountList(new List<SMRAccount>());
            SetSMREligibilityAccountList(new List<SMRAccount>());
            SetRealSMREligibilityAccountList(new List<SMRAccount>());
        }

        public static void SetAccountActivityInfoList(List<SMRAccountActivityInfo> smrAccountActivityList)
        {
            ISharedPreferencesEditor editor = mPreferences.Edit();
            string jsonAccountList = JsonConvert.SerializeObject(smrAccountActivityList);
            editor.PutString("SMR_ACCOUNT_ACTIVITY_INFO_LIST", jsonAccountList);
            editor.Apply();
        }

        public static List<SMRAccountActivityInfo> GetAccountActivityInfoList()
        {
            string accountInfoListString = mPreferences.GetString("SMR_ACCOUNT_ACTIVITY_INFO_LIST", null);
            List<SMRAccountActivityInfo> selectAccountList = new List<SMRAccountActivityInfo>();
            if (accountInfoListString != null)
            {
                selectAccountList = JsonConvert.DeserializeObject<List<SMRAccountActivityInfo>>(accountInfoListString);
            }
            return selectAccountList;
        }

        public static void SaveAppLanguage(string language)
        {
            ISharedPreferences sharedPreferences = Application.Context.GetSharedPreferences(Constants.ACCOUNT_SHARED_PREF_ID, FileCreationMode.Private);
            ISharedPreferencesEditor editor = sharedPreferences.Edit();
            editor.PutString(Constants.SHARED_PREF_LANGUAGE_KEY, language);
            editor.Apply();
        }

        public static string GetAppLanguage()
        {
            ISharedPreferences sharedPreferences = Application.Context.GetSharedPreferences(Constants.ACCOUNT_SHARED_PREF_ID, FileCreationMode.Private);
            return sharedPreferences.GetString(Constants.SHARED_PREF_LANGUAGE_KEY, null);
        }

        public static void SaveIsAppLanguageChanged(bool isChanged)
        {
            ISharedPreferences sharedPreferences = Application.Context.GetSharedPreferences(Constants.ACCOUNT_SHARED_PREF_ID, FileCreationMode.Private);
            ISharedPreferencesEditor editor = sharedPreferences.Edit();
            editor.PutBoolean(Constants.SHARED_PREF_LANGUAGE_IS_CHANGE_KEY, isChanged);
            editor.Apply();
        }

        public static bool GetIsAppLanguageChanged()
        {
            ISharedPreferences sharedPreferences = Application.Context.GetSharedPreferences(Constants.ACCOUNT_SHARED_PREF_ID, FileCreationMode.Private);
            return sharedPreferences.GetBoolean(Constants.SHARED_PREF_LANGUAGE_IS_CHANGE_KEY, false);
        }

        public static void SaveDeviceId(string deviceId)
        {
            ISharedPreferences sharedPreferences = Application.Context.GetSharedPreferences(Constants.ACCOUNT_SHARED_PREF_ID, FileCreationMode.Private);
            ISharedPreferencesEditor editor = sharedPreferences.Edit();
            editor.PutString(Constants.SHARED_PREF_DEVICE_ID_KEY, deviceId);
            editor.Apply();
        }

        public static string GetDeviceId()
        {
            ISharedPreferences sharedPreferences = Application.Context.GetSharedPreferences(Constants.ACCOUNT_SHARED_PREF_ID, FileCreationMode.Private);
            return sharedPreferences.GetString(Constants.SHARED_PREF_DEVICE_ID_KEY, null);
        }

        public static void SavedLanguagePrefResult(bool isSuccess)
        {
            ISharedPreferences sharedPreferences = Application.Context.GetSharedPreferences(Constants.ACCOUNT_SHARED_PREF_ID, FileCreationMode.Private);
            ISharedPreferencesEditor editor = sharedPreferences.Edit();
            editor.PutBoolean(Constants.SHARED_PREF_SAVED_LANG_PREF_RESULT_KEY, isSuccess);
            editor.Apply();
        }

        public static bool IsSavedLanguagePrefResultSuccess()
        {
            ISharedPreferences sharedPreferences = Application.Context.GetSharedPreferences(Constants.ACCOUNT_SHARED_PREF_ID, FileCreationMode.Private);
            return sharedPreferences.GetBoolean(Constants.SHARED_PREF_SAVED_LANG_PREF_RESULT_KEY, false);
        }
    }
}