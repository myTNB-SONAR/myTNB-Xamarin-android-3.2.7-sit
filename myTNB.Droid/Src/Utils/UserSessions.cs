using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Preferences;
using Java.Lang;
using Java.Text;
using Java.Util;

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



        public static void SetCurrentImageCount(ISharedPreferences prefs ,  int count)
        {

            SimpleDateFormat simpleDateFormatter = new SimpleDateFormat("dd-MM-yyyy");
            Calendar calendarNow = Calendar.GetInstance(Locale.Default);
            Calendar calendarYesterday = calendarNow.Clone() as Calendar;
            calendarYesterday.Add(CalendarField.Date, -1);

            ISharedPreferencesEditor editor = prefs.Edit();
            editor.Remove(string.Format("{0}{1}", "currentImageCount" , simpleDateFormatter.Format(calendarYesterday.TimeInMillis)));
            editor.PutInt(string.Format("{0}{1}", "currentImageCount", simpleDateFormatter.Format(calendarNow.TimeInMillis)) , count);
            editor.Apply();
        }

        public static int GetCurrentImageCount(ISharedPreferences prefs)
        {

            SimpleDateFormat simpleDateFormatter = new SimpleDateFormat("dd-MM-yyyy");
            Calendar calendarNow = Calendar.GetInstance(Locale.Default);
            return prefs.GetInt(string.Format("{0}{1}", "currentImageCount", simpleDateFormatter.Format(calendarNow.TimeInMillis)) , 0);
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
            return prefs.GetString("notificationEmail" , null);
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
            return mSharedPref.GetBoolean("resetPasswordEnabled" , false);
        }
        [Deprecated]
        internal static void PersistPassword(ISharedPreferences mSharedPref , string password)
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.PutString("currentPassword", password);
            editor.Apply();
        }
        [Deprecated]
        internal static string GetPersistPassword(ISharedPreferences mSharedPref)
        {
            return mSharedPref.GetString("currentPassword" , null);
        }

        internal static void RemovePersistPassword(ISharedPreferences mSharedPref)
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.Remove("currentPassword");
            editor.Apply();
        }

        internal static void SaveSelectedFeedback(ISharedPreferences mSharedPref , string largeJsonString)
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.PutString(Constants.SELECTED_FEEDBACK, largeJsonString);
            editor.Apply();
        }

        internal static string GetSelectedFeedback(ISharedPreferences mSharePref)
        {
            return mSharePref.GetString(Constants.SELECTED_FEEDBACK , null);
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

        public static int GetPrevAppVersionCode(ISharedPreferences preferences) {
            return preferences.GetInt("PREV_APP_VERSION_CODE", 0);
        }

        public static void SetAppVersionCode(ISharedPreferences preferences, int appVersionCode) {
            ISharedPreferencesEditor editor = preferences.Edit();
            editor.PutInt("PREV_APP_VERSION_CODE", appVersionCode);
            editor.Apply();
        }

        public static string GetUserEmail(ISharedPreferences preferences)
        {
            return preferences.GetString("loginEmail", "");
        }
    }
}