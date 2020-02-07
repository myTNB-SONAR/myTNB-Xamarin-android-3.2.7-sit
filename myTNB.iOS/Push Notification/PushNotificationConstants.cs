using System.Collections.Generic;
using myTNB.Enums;

namespace myTNB.PushNotification
{
    public static class PushNotificationConstants
    {
        //Pagename
        public static string Pagename_PushNotificationList = "PushNotificationList";
        public static string Pagename_PushNotificationDetails = "PushNotificationDetails";

        //Cell
        public static string Cell_PushNotificationCell = "pushNotificationCell";

        //Service
        public static string Service_DeleteNotification = "DeleteUserNotification";
        public static string Service_ReadUserNotification = "ReadUserNotification";
        public static string Service_GetSMRAccountActivityInfo = "GetSMRAccountActivityInfo";
        public static string Service_GetUserNotificationTypePreferences = "GetUserNotificationTypePreferences";
        public static string Service_GetUserNotificationChannelPreferences = "GetUserNotificationChannelPreferences";
        public static string Service_GetAppNotificationTypes = "GetAppNotificationTypes";
        public static string Service_GetUserNotifications = "GetUserNotifications";
        public static string Service_GetNotificationDetailedInfo = "GetNotificationDetails";

        //Image
        public static string IMG_Select = "Notification-Select";
        public static string IMG_MarkAsRead = "Notification-MarkAsRead";
        public static string IMG_MarkAsReadInactive = "Notification-MarkAsReadDisabled";
        public static string IMG_RefreshErrorNormal = "Refresh-Error-Normal";
        public static string IMG_Empty = "Notification-Empty";
        public static string IMG_Cancel = "IC-Header-Cancel";
        public static string IMG_ChkInactive = "Payment-Checkbox-Inactive";
        public static string IMG_ChkActive = "Payment-Checkbox-Active";
        public static string IMG_Delete = "Notification-Delete";
        public static string IMG_DeleteInactive = "Notification-DeleteDisabled";

        //Banner
        public static string Banner_SSMRRemainder = "Notification-Banner-SSMR-Remainder";
        public static string Banner_SSMROpen = "Notification-Banner-SSMR-Open";
        public static string Banner_SSMRMissed = "Notification-Banner-SSMR-Missed";

        //Banner Dictionary
        public static Dictionary<BCRMNotificationEnum, string> BannerImageDictionary = new Dictionary<BCRMNotificationEnum, string> {
            { BCRMNotificationEnum.NewBill, "Notification-Banner-New-Bill" }
            , { BCRMNotificationEnum.BillDue, "Notification-Banner-Bill-Due" }
            , { BCRMNotificationEnum.Dunning, "Notification-Banner-Dunning" }
            , { BCRMNotificationEnum.Disconnection, "Notification-Banner-Disconnection" }
            , { BCRMNotificationEnum.Reconnection, "Notification-Banner-Reconnection" }
            , { BCRMNotificationEnum.Maintenance, "Notification-Banner-Maintenance" }
            , { BCRMNotificationEnum.SSMR, "SSMR-Reading-History-Banner" }
            , { BCRMNotificationEnum.None, "Notification-Banner-Generic" }
        };

        //SSMR Banner Dictionary
        public static Dictionary<SSMRNotificationEnum, string> SSMRBannerImageDictionary = new Dictionary<SSMRNotificationEnum, string> {
            { SSMRNotificationEnum.RegistrationCompleted, Banner_SSMRRemainder }
            , { SSMRNotificationEnum.RegistrationCancelled, Banner_SSMRMissed }
            , { SSMRNotificationEnum.OpenMeterReadingPeriod, Banner_SSMROpen }
            , { SSMRNotificationEnum.NoSubmissionReminder, Banner_SSMRRemainder }
            , { SSMRNotificationEnum.MissedSubmission, Banner_SSMROpen }
            , { SSMRNotificationEnum.TerminationCompleted, Banner_SSMRRemainder }
            , { SSMRNotificationEnum.TerminationCancelled, Banner_SSMRMissed }
        };

        //Icon Image Dictionary
        public static Dictionary<BCRMNotificationEnum, string> IconDictionary = new Dictionary<BCRMNotificationEnum, string> {
            { BCRMNotificationEnum.NewBill, "Notification-New-Bill" }
            , { BCRMNotificationEnum.BillDue, "Notification-Bill-Due" }
            , { BCRMNotificationEnum.Dunning, "Notification-Dunning" }
            , { BCRMNotificationEnum.Disconnection, "Notification-Disconnection" }
            , { BCRMNotificationEnum.Reconnection, "Notification-Reconnection" }
            , { BCRMNotificationEnum.Maintenance, "Notification-Maintenance" }
            , { BCRMNotificationEnum.SSMR, "Notification-SSMR" }
            , { BCRMNotificationEnum.None, "Notification-Generic" }
        };

        //I18N
        //Details
        public static string I18N_ContactTNB = "contactTNB";
        public static string I18N_ViewBill = "viewBill";
        public static string I18N_Paynow = "payNow";
        public static string I18N_ViewMyUsage = "viewMyUsage";
        public static string I18N_SubmitMeterReading = "submitMeterReading";
        public static string I18N_ReenableSSMR = "reenableSSMR";
        public static string I18N_ViewReadingHistory = "viewReadingHistory";
        //List
        public static string I18N_AllNotifications = "allNotifications";
        public static string I18N_DeleteMessage = "deleteMessage";
        public static string I18N_DeleteTitle = "deleteTitle";
        public static string I18N_DeleteAllMessage = "deleteAllMessage";
        public static string I18N_DeleteAllTitle = "deleteAllTitle";
        public static string I18N_DeleteMessageMultiple = "deleteMessageMultiple";
        public static string I18N_DeleteTitleMultiple = "deleteTitleMultiple";
        public static string I18N_NoNotification = "noNotification";
        public static string I18N_NotificationDeleted = "notificationDeleted";
        public static string I18N_NotificationsDeleted = "notificationsDeleted";
        public static string I18N_Select = "select";
        public static string I18N_Selected = "selected";
        public static string I18N_SelectNotification = "selectNotification";
        public static string I18N_Title = "title";

        //Common
        public static string I18N_CustomerAccountNumber = "customerAccountNumber";

        //Regex Constants
        public static string REGEX_AccountNickname = "#accountNickname#";
    }
}