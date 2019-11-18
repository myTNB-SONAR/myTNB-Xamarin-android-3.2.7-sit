﻿using System.Collections.Generic;
using Android.Content;
using Java.Lang;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Utils
{
    public sealed class UserSessions
    {
        //public static Boolean HasSkipped(Context context)
        //{
        //    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
        //    return prefs.GetBoolean("hasSkipped" , false);
        //}

        //public static void DoSkipped(Context context)
        //{
        //    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
        //    ISharedPreferencesEditor editor = prefs.Edit();
        //    editor.PutBoolean("hasSkipped", true);
        //    editor.Apply();
        //}

        private static ISharedPreferences mPreferences;

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

        public static void RemoveNotificationSession(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.Remove("hasNotification").Remove("notificationEmail").Apply();
        }

        public static System.Boolean HasSkipped(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasSkipped", false);
        }

        public static void DoSkipped(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasSkipped", true);
            editor.Apply();
        }

        public static System.Boolean HasUpdateSkipped(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasUpdateSkipped", false);
        }

        public static void DoUpdateSkipped(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasUpdateSkipped", true);
            editor.Apply();
        }

        public static System.Boolean HasSMROnboardingShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasSMROnboardingShown", false);
        }

        public static void DoSMROnboardingShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasSMROnboardingShown", true);
            editor.Apply();
        }

        public static System.Boolean HasPayBillShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasPayBillShown", false);
        }

        public static void DoPayBillShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasPayBillShown", true);
            editor.Apply();
        }

        public static System.Boolean HasViewBillShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasViewBillShown", false);
        }

        public static void DoViewBillShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasViewBillShown", true);
            editor.Apply();
        }

        public static System.Boolean HasWhatNewShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasWhatNewShown", false);
        }

        public static void DoWhatNewShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasWhatNewShown", true);
            editor.Apply();
        }

        public static System.Boolean HasHomeTutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasHomeTutorialShown", false);
        }

        public static void DoHomeTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasHomeTutorialShown", true);
            editor.Apply();
        }

        // Lin Siong TODO:  To enable once iOS Implementation done
        /*public static System.Boolean HasItemizedBillingNMSMTutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasItemizedBillingNMSMTutorialShown", false);
        }

        public static void DoItemizedBillingNMSMTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasItemizedBillingNMSMTutorialShown", true);
            editor.Apply();
        }

        public static System.Boolean HasItemizedBillingRETutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasItemizedBillingRETutorialShown", false);
        }

        public static void DoItemizedBillingRETutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasItemizedBillingRETutorialShown", true);
            editor.Apply();
        }*/

        public static System.Boolean HasItemizedBillingTutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasItemizedBillingTutorialShown", false);
        }
        public static void DoItemizedBillingTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasItemizedBillingTutorialShown", true);
            editor.Apply();
        }

        public static System.Boolean HasItemizedBillingDetailTutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasItemizedBillingDetailTutorialShown", false);
        }

        public static void DoItemizedBillingDetailTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasItemizedBillingDetailTutorialShown", true);
            editor.Apply();
        }

        public static System.Boolean HasSMRMeterHistoryTutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasSMRMeterHistoryTutorialShown", false);
        }

        public static void DoSMRMeterHistoryTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasSMRMeterHistoryTutorialShown", true);
            editor.Apply();
        }

        public static System.Boolean HasSMRSubmitMeterTutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasSMRSubmitMeterTutorialShown", false);
        }

        public static void DoSMRSubmitMeterTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasSMRSubmitMeterTutorialShown", true);
            editor.Apply();
        }

        public static System.Boolean HasSMRDashboardTutorialShown(ISharedPreferences prefs)
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

        internal static System.Boolean HasResetFlag(ISharedPreferences mSharedPref)
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

        public static System.Boolean IsDeviceIdUpdated(ISharedPreferences prefs)
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
    }
}