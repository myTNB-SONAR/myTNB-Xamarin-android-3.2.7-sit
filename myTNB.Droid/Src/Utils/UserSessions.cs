using System.Collections.Generic;
using Android.App;
using Android.Content;
using Java.Lang;
using Java.Text;
using Java.Util;
using myTNB.Mobile;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.DBR.DBRApplication.MVP;
using Newtonsoft.Json;
using myTNB_Android.Src.MyHome;

namespace myTNB_Android.Src.Utils
{
    public sealed class UserSessions
    {
        private UserSessions() { }

        private static ISharedPreferences mPreferences;
        internal static ApplicationStatusNotificationModel ApplicationStatusNotification { private set; get; }
        public static MobileEnums.DBRTypeEnum ManageBillDelivery { set; get; }
        internal static NotificationOpenDirectDetails Notification { private set; get; }
        internal static string DBROwnerNotificationAccountNumber { set; get; } = string.Empty;

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
                Notification = new NotificationOpenDirectDetails
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
            editor.Remove("hasNotification")
                .Remove("notificationEmail")
                .Remove("notificationType")
                .Apply();
        }

        public static bool HasDynamicLink(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasDynamicLink", false);
        }

        public static void DoflagDynamicLink(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasDynamicLink", true);
            editor.Apply();
        }

        internal static void DoUnflagDynamicLink(ISharedPreferences mSharedPref)
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.Remove("hasDynamicLink");
            editor.Apply();
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

        public static bool MyHomeDashboardTutorialHasShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("myHomeDashboardTutorialHasShown", false);
        }

        public static void SetShownMyHomeDashboardTutorial(ISharedPreferences prefs)
        {
            if (MyHomeUtility.Instance.IsAccountEligible)
            {
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.PutBoolean("myHomeDashboardTutorialHasShown", true);
                editor.Apply();
            }
        }

        public static bool HomeDashboardTutorialHasShownBefore(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("HomeDashboardTutorialHasShownBefore", false);
        }

        public static void SetShownBeforeHomeDashboardTutorial(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("HomeDashboardTutorialHasShownBefore", true);
            editor.Apply();
        }

        internal static void UpdateNCTutorialShown(ISharedPreferences mSharedPref)       //for update flag overlay
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.Remove("hasHomeTutorialShown");
            editor.Apply();
        }

        internal static void UpdateHomeTutorialShown(ISharedPreferences mSharedPref) //to show home tutorial again for myHome
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.Remove("hasHomeTutorialShown");
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

        public static System.Boolean HasManageAccessIconTutorialShown(ISharedPreferences prefs)            //new manage access tutorial icon
        {
            return prefs.GetBoolean("hasManageAccessIconTutorialShown", false);
        }

        public static void DoManageAccessIconTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasManageAccessIconTutorialShown", true);
            editor.Apply();
        }

        public static System.Boolean HasManageAccessPageTutorialShown(ISharedPreferences prefs)            //new manage access tutorial page
        {
            return prefs.GetBoolean("hasManageAccessPageTutorialShown", false);
        }

        public static void DoManageAccessPageTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasManageAccessPageTutorialShown", true);
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
        public static void DoManageSupplyAccountTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasManageSupplyAccountTutorialShown", true);
            editor.Apply();
        }
        public static void DoManageEBillDeliveryTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasManageEBillDeliveryTutorialShown", true);
            editor.Apply();
        }
        public static bool HasManageEBillDeliveryTutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasManageEBillDeliveryTutorialShown", false);
        }
        public static void DoManagepoptedEBillDeliveryTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasManageOptedEBillDeliveryTutorialShown", true);
            editor.Apply();
        }
        public static bool HasManageOptedEBillDeliveryTutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasManageOptedEBillDeliveryTutorialShown", false);
        }
        public static void DoManageEmailBillDeliveryTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasManageEmailBillDeliveryTutorialShown", true);
            editor.Apply();
        }
        public static bool HasManageEmailBillDeliveryTutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasManageEmailBillDeliveryTutorialShown", false);
        }
        public static void DoManageParallelEmailBillDeliveryTutorialShown(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("hasManageParallelEmailBillDeliveryTutorialShown", true);
            editor.Apply();
        }
        public static bool HasManageParallelEmailBillDeliveryTutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasManageParallelEmailBillDeliveryTutorialShown", false);
        }

        public static bool HasManageSupplyAccountTutorialShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("hasManageSupplyAccountTutorialShown", false);
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

        internal static void SaveSelectedCountry(ISharedPreferences mSharedPref, string largeJsonString)     //pref for selected country
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.PutString("selectedcountry", largeJsonString);
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

        internal static string GetSelectedCountry(ISharedPreferences mSharePref)    //get pref for selected country
        {
            return mSharePref.GetString("selectedcountry", null);
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

        internal static void SetUpdateIdPopUp(ISharedPreferences mSharedPref)                 //for save ID dialog
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.PutBoolean("PopUpIDUpdated", true);
            editor.Apply();
        }

        public static bool GetUpdateIdPopUp(ISharedPreferences prefs)            //for get ID dialog
        {
            return prefs.GetBoolean("PopUpIDUpdated", false);
        }

        internal static void SetUpdateIdDialog(ISharedPreferences mSharedPref)                 //for save ID dialog
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.PutBoolean("DialogIDUpdated", true);
            editor.Apply();
        }

        public static bool GetUpdateIdDialog(ISharedPreferences prefs)            //for get ID dialog
        {
            return prefs.GetBoolean("DialogIDUpdated", false);
        }

        internal static void UpdateUpdateIdDialog(ISharedPreferences mSharedPref)  //for Update ID dialog
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.Remove("DialogIDUpdated");
            editor.Apply();
        }

        public static void SaveCheckEmailVerified(ISharedPreferences prefs, string data)    //for Check Email Verified
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("IsFeedbackUpdateDetailDisabled", data);
            editor.Apply();
        }

        public static string GetCheckEmailVerified(ISharedPreferences prefs)              //for Check Email Verified
        {
            return prefs.GetString("IsFeedbackUpdateDetailDisabled", null);
        }

        public static void SaveUserIDEmailVerified(ISharedPreferences prefs, string data)    //for Save userID Email Verified Dynamic link
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("UserIDEmailVerified", data);
            editor.Apply();
        }

        internal static void UpdateUserIDEmailVerified(ISharedPreferences mSharedPref)       //for update userID Email Verified Dynamic link
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.Remove("UserIDEmailVerified");
            editor.Apply();
        }

        public static string GetUserIDEmailVerified(ISharedPreferences prefs)              //for Check userID Email Verified Dynamic link
        {
            return prefs.GetString("UserIDEmailVerified", "");
        }

        public static void SaveLoginflag(ISharedPreferences prefs, bool flag)    //for Save login flag for dynamic link
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("Loginflag", flag);
            editor.Apply();
        }

        internal static void UpdateLoginflag(ISharedPreferences mSharedPref)       //for update login flag for dynamic link
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.Remove("Loginflag");
            editor.Apply();
        }

        public static bool GetLoginflag(ISharedPreferences prefs)              //for Check login flag for dynamic link
        {
            return prefs.GetBoolean("Loginflag",false);
        }

        public static void SaveEmailflag(ISharedPreferences prefs, bool flag)    //for Save email flag for add account
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("Emailflag", flag);
            editor.Apply();
        }

        public static bool GetEmailflag(ISharedPreferences prefs)              //for Check email flag for add account
        {
            return prefs.GetBoolean("Emailflag", false);
        }

        internal static void UpdateEmailflag(ISharedPreferences mSharedPref)       //for update email flag for add account
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.Remove("Emailflag");
            editor.Apply();
        }


        public static int GetNCFlag(ISharedPreferences prefs)              
        {
            return prefs.GetInt("NCFlag", 0);
        }

        public static void SaveNCFlag(ISharedPreferences prefs, int data)    
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutInt("NCFlag", data);
            editor.Apply();
        }

        internal static void UpdateNCFlag(ISharedPreferences mSharedPref)
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.Remove("NCFlag");
            editor.Apply();
        }

        public static void SaveNewNCFlag(ISharedPreferences prefs, bool flag)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("NewNCFlag", flag);
            editor.Apply();
        }

        public static bool GetNewNCFlag(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("NewNCFlag", false);
        }

        internal static void UpdateNewNCFlag(ISharedPreferences mSharedPref)
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.Remove("NewNCFlag");
            editor.Apply();
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
            editor.PutString("IsFeedbackUpdateDetailDisabled2", data);
            editor.Apply();
        }

        public static string GetFeedbackUpdateDetailDisabled(ISharedPreferences prefs)
        {
            var error = prefs.GetString("IsFeedbackUpdateDetailDisabled2", null);
            return prefs.GetString("IsFeedbackUpdateDetailDisabled2", null);
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

        //maskingaddress setter | yana
        //public static void SaveAddress(List<NewAccount> newAccount)
        //{
        //    ISharedPreferencesEditor editor = mPreferences.Edit();
        //    string jsonAccountList = JsonConvert.SerializeObject(newAccount);
        //    editor.PutString("BILL_MASKING", jsonAccountList);
        //    editor.Apply();
        //}

        ////maskingaddress getter | yana
        //public static List<NewAccount> GetAddress()                                  
        //{
        //    string accountList = mPreferences.GetString("BILL_MASKING", null);
        //    List<NewAccount> selectAccountList = new List<NewAccount>();
        //    if (accountList != null)
        //    {
        //        selectAccountList = JsonConvert.DeserializeObject<List<NewAccount>>(accountList);
        //    }
        //    return selectAccountList;
        //}

        
        //whitelist setter | yana
        public static void SaveWhiteList(ISharedPreferences prefs, bool flag)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("IsWhiteList", flag);
            editor.Apply();
        }

        //whitelist getter | yana
        public static bool GetWhiteList(ISharedPreferences preferences)
        {
            return preferences.GetBoolean("IsWhiteList", false);
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

        public static void SavePopUpCountUpdate(ISharedPreferences prefs, string count)    //count Update App popup
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("popupUpdate", count);
            editor.Apply();
        }

        public static string GetSavePopUpCountUpdate(ISharedPreferences preferences)    //count Update App popup
        {
            return preferences.GetString("popupUpdate", "");
        }

        public static void SaveNeedPopup(ISharedPreferences prefs, bool flag)           //Save need reset App popup
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("IsNeedResetPopup", flag);
            editor.Apply();
        }

        public static bool GetSaveNeedPopup(ISharedPreferences preferences)             //Get need reset App popup
        {
            return preferences.GetBoolean("IsNeedResetPopup", false);
        }

        public static void SavePopUpDateReset(ISharedPreferences prefs, string date)    //count reset date update App popup
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("popupUpdateDate", date);
            editor.Apply();
        }

        public static string GetSavePopUpDateReset(ISharedPreferences preferences)    //count reset date update App popup
        {
            return preferences.GetString("popupUpdateDate", "");
        }

        public static void SavePopUpCountForTheCurrentPeriod(ISharedPreferences prefs, string count)    //count total for the current period
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("popupCountForTheCurrentPeriod", count);
            editor.Apply();
        }

        public static string GetSavePopUpCountForTheCurrentPeriod(ISharedPreferences preferences)    //count total for the current period
        {
            return preferences.GetString("popupCountForTheCurrentPeriod", "");
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

        internal static void SetCommercialList(List<CustomerBillingAccount> Accounts)                 //for Commercial dialog | yana
        {
            ISharedPreferencesEditor editor = mPreferences.Edit();
            string jsonAccountList = JsonConvert.SerializeObject(Accounts);
            editor.PutString("COMMERCIAL_ACCOUNT_LIST", jsonAccountList);
            editor.Apply();
        }

        public static List<CustomerBillingAccount> GetCommercialList()                                  //for Commercial dialog | yana
        {
            string accountList = mPreferences.GetString("COMMERCIAL_ACCOUNT_LIST", null);
            List<CustomerBillingAccount> selectCommercialAccountList = new List<CustomerBillingAccount>();
            if (accountList != null)
            {
                selectCommercialAccountList = JsonConvert.DeserializeObject<List<CustomerBillingAccount>>(accountList);
            }
            return selectCommercialAccountList;
        }

        public static void SetNCDate(ISharedPreferences prefs, string date)                  //for NC Add account| yana
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("NC_CREATED_DATE", date);
            editor.Apply();
        }

        public static string GetNCDate(ISharedPreferences prefs)
        {
            return prefs.GetString("NC_CREATED_DATE", null);
        }

        internal static void UpdateNCDate(ISharedPreferences mSharedPref)
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.Remove("NC_CREATED_DATE");
            editor.Apply();
        }

        public static void SetFromBRCard(ISharedPreferences prefs, bool flag)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("BRCard", flag);
            editor.Apply();
        }

        //whitelist getter | yana
        public static bool GetFromBRCard(ISharedPreferences preferences)
        {
            return preferences.GetBoolean("BRCard", false);
        }

        internal static void UpdateFromBRCard(ISharedPreferences mSharedPref)
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.Remove("BRCard");
            editor.Apply();
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
        public static void SetRealDBREligibilityAccountList(List<DBRAccount> dBRccounts)
        {
            ISharedPreferencesEditor editor = mPreferences.Edit();
            string jsonAccountList = JsonConvert.SerializeObject(dBRccounts);
            editor.PutString("DBR_REAL_ELIGIBILITY_ACCOUNT_LIST", jsonAccountList);
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

        internal static void RemoveEligibleData(ISharedPreferences mSharedPref)
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.PutString(MobileConstants.SharePreferenceKey.GetEligibilityData, string.Empty);
            editor.PutString(MobileConstants.SharePreferenceKey.GetEligibilityTimeStamp, string.Empty);
            editor.PutString(MobileConstants.SharePreferenceKey.AccessToken, string.Empty);
            editor.Apply();
        }

        public static int GetUploadFileNameCounter(ISharedPreferences preferences)
        {
            return preferences.GetInt("UPLOAD_FILE_NAME_COUNTER", 0);
        }

        public static void SetUploadFileNameCounter(ISharedPreferences preferences, int counter)
        {
            ISharedPreferencesEditor editor = preferences.Edit();
            editor.PutInt("UPLOAD_FILE_NAME_COUNTER", counter);
            editor.Apply();
        }

        public static void SaveDBRPopUpFlag(ISharedPreferences prefs, bool flag)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("DBRPopUpHasShown", flag);
            editor.Apply();
        }

        public static bool GetDBRPopUpFlag(ISharedPreferences preferences)
        {
            return preferences.GetBoolean("DBRPopUpHasShown", false);
        }

        public static void SaveDBRMarketingPopUpFlag(ISharedPreferences prefs, bool flag)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("DBRMarketingPopUpHasShown", flag);
            editor.Apply();
        }

        public static bool GetDBRMarketingPopUpFlag(ISharedPreferences preferences)
        {
            return preferences.GetBoolean("DBRMarketingPopUpHasShown", false);
        }
        
        public static bool MyHomeDrawerTutorialHasShown(ISharedPreferences prefs, string key)
        {
            return prefs.GetBoolean(key, false);
        }

        public static void SetShownMyHomeDrawerTutorial(ISharedPreferences prefs, string key)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean(key, true);
            editor.Apply();
        }

        public static bool MyHomeMarketingPopUpHasShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("myHomeMarketingPopUpHasShown", false);
        }

        public static void SetShownMyHomeMarketingPopUp(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("myHomeMarketingPopUpHasShown", true);
            editor.Apply();
        }

        public static bool MyHomeQuickLinkHasShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("myHomeQuickLinkHasShown", false);
        }

        public static void SetShownMyHomeQuickLink(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("myHomeQuickLinkHasShown", true);
            editor.Apply();
        }

        public static bool ConnectMyPremiseHasShown(ISharedPreferences prefs)
        {
            return prefs.GetBoolean("connectMyPremiseHasShown", false);
        }

        public static void SetShownConnectMyPremise(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("connectMyPremiseHasShown", true);
            editor.Apply();
        }

        public static string GetServicesTimeStamp(ISharedPreferences prefs)
        {
            return prefs.GetString(PreferenceKey.Home.QuickLinkImagesTimestamp, string.Empty);
        }

        public static void SetGetServicesTimeStamp(ISharedPreferences prefs, string timeStamp)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString(PreferenceKey.Home.QuickLinkImagesTimestamp, timeStamp);
            editor.Apply();
        }

        public static string GetNCResumePopUpRefNos(ISharedPreferences prefs)
        {
            return prefs.GetString(MyHomeConstants.USER_SESSION_NC_RESUME_POPUP_KEY, string.Empty);
        }

        public static void SetNCResumePopUpRefNos(ISharedPreferences prefs, string refNos)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString(MyHomeConstants.USER_SESSION_NC_RESUME_POPUP_KEY, refNos);
            editor.Apply();
        }

        public static void SaveLanguageFBFlag(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("FBLangIsUpdated", true);
            editor.Apply();
        }

        public static bool GetLanguageFBFlag(ISharedPreferences preferences)
        {
            return preferences.GetBoolean("FBLangIsUpdated", false);
        }

        internal static void UpdateLanguageFBFlag(ISharedPreferences mSharedPref)
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.Remove("FBLangIsUpdated");
            editor.Apply();
        }

        public static void SaveLanguageFBContentFlag(ISharedPreferences prefs)
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("FBContentLangIsUpdated", true);
            editor.Apply();
        }

        public static bool GetLanguageFBContentFlag(ISharedPreferences preferences)
        {
            return preferences.GetBoolean("FBContentLangIsUpdated", false);
        }

        internal static void UpdateLanguageFBContentFlag(ISharedPreferences mSharedPref)
        {
            ISharedPreferencesEditor editor = mSharedPref.Edit();
            editor.Remove("FBContentLangIsUpdated");
            editor.Apply();
        }

        public static void SaveUserNotificationFirstTimeInstallFlag(ISharedPreferences prefs, bool flag)           //Save need to ask notification permission popup
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("NotificationFirstTime", flag);
            editor.Apply();
        }

        public static bool GetUserNotificationFirstTimeInstallFlag(ISharedPreferences preferences)                  //Get flag to ask notification permission popup
        {
            return preferences.GetBoolean("NotificationFirstTime", false);
        }
        
        public static void SetSMRAccountListOwner(List<SMRAccount> sMRAccounts)
        {
            ISharedPreferencesEditor editor = mPreferences.Edit();
            string jsonAccountList = JsonConvert.SerializeObject(sMRAccounts);
            editor.PutString("SMR_ACCOUNT_LIST_OWNER", jsonAccountList);
            editor.Apply();
        }

        public static List<SMRAccount> GetSMRAccountListOwner()
        {
            string accountList = mPreferences.GetString("SMR_ACCOUNT_LIST_OWNER", null);
            List<SMRAccount> selectAccountList = new List<SMRAccount>();
            if (accountList != null)
            {
                selectAccountList = JsonConvert.DeserializeObject<List<SMRAccount>>(accountList);
            }
            return selectAccountList;
        }

        public static void SetSMRAccountListOwnerCanApply(List<string> sMRAccounts)
        {
            ISharedPreferencesEditor editor = mPreferences.Edit();
            string jsonAccountList = JsonConvert.SerializeObject(sMRAccounts);
            editor.PutString("SMR_ACCOUNT_LIST_OWNER_APPLY", jsonAccountList);
            editor.Apply();
        }

        public static List<string> GetSMRAccountListOwnerCanApply()
        {
            string accountList = mPreferences.GetString("SMR_ACCOUNT_LIST_OWNER_APPLY", null);
            List<string> selectAccountList = new List<string>();
            if (accountList != null)
            {
                selectAccountList = JsonConvert.DeserializeObject<List<string>>(accountList);
            }
            return selectAccountList;
        }
    }
}