using System;
using System.Collections.Generic;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.MyHome
{
    public class MyHomeConstants
    {
        internal static readonly string MYHOME_MODEL = "myHomeDrawerModel";
        internal static readonly string ACCESS_TOKEN = "accessToken";
        internal static readonly string APPLICATION_DETAIL_RESPONSE = "applicationStatusResponse";
        internal static readonly string CANCEL_TOAST_MESSAGE = "cancelToastMessage";
        internal static readonly string ACTION_BACK_TO_APP = "mytnbapp://action=backToApp";
        internal static readonly string ACTION_BACK_TO_HOME = "mytnbapp://action=backToHome";
        internal static readonly string ACTION_BACK_TO_APPLICATION_STATUS_LANDING = "mytnbapp://action=applicationStatusLanding";
        internal static readonly string ACTION_EXTERNAL_BROWSER = "mytnbapp://action=externalBrowser";
        internal static readonly string ACTION_INAPP_BROWSER = "mytnbapp://action=inAppBrowser";
        internal static readonly string ACTION_OPEN_FILE = "mytnbapp://action=openPDF";
        internal static readonly string ACTION_DOWNLOAD_FILE = "mytnbapp://action=downloadPDF";
        internal static readonly string ACTION_RATE_SUCCESSFUL = "mytnbapp://action=rateSuccessful";
        internal static readonly string ACTION_SHOW_LATEST_BILL = "mytnbapp://action=showLatestBill";
        internal static readonly string PATTERN = "\\b{0}.*\\b";
        internal static readonly string REPLACE_KEY = "{0}/";
        internal static readonly string FULL_STOP = ".";
        internal static readonly string FILE_KEY = "file=";
        internal static readonly string MYHOME = "myhome";
        internal static readonly string DEFAULT_FILENAME = "document";
        internal static readonly string TITLE = "title";
        internal static readonly string EXTENSION = "extension";
        internal static readonly string EXTENSION_PDF = "pdf";
        internal static readonly string EXTENSION_PNG = "png";
        internal static readonly string EXTENSION_JPG = "jpg";
        internal static readonly string USER_SESSION_NC_RESUME_POPUP_KEY = "userSessionNCResumePopUpKey";
        internal static readonly string PAYMENT_CA = "ca";
        internal static readonly string PAYMENT_IS_OWNER = "isOwner";
        internal static readonly List<string> MYHOME_NOTIFS = new List<string>()
        {
            Constants.BCRM_NOTIFICATION_MYHOME_NC_ADDRESS_SEARCH_COMPLETED,
            Constants.BCRM_NOTIFICATION_MYHOME_NC_RESUME_APPLICATION,
            Constants.BCRM_NOTIFICATION_MYHOME_NC_APPLICATION_COMPLETED,
            Constants.BCRM_NOTIFICATION_MYHOME_NC_APPLICATION_CONTRACTOR_COMPLETED,
            Constants.BCRM_NOTIFICATION_MYHOME_NC_OTP_VERIFY,
            Constants.BCRM_NOTIFICATION_MYHOME_NC_CONTRACTOR_ACCEPTED,
            Constants.BCRM_NOTIFICATION_MYHOME_NC_CONTRACTOR_REJECTED,
            Constants.BCRM_NOTIFICATION_MYHOME_NC_CONTRACTOR_NO_RESPONSE,
            Constants.BCRM_NOTIFICATION_MYHOME_NC_APPLICATION_REQUIRES_UPDATE,
            Constants.BCRM_NOTIFICATION_MYHOME_COA_APPLICATION_COMPLETED,
            Constants.BCRM_NOTIFICATION_MYHOME_COA_APPLICATION_CANCELLED,
            Constants.BCRM_NOTIFICATION_MYHOME_COA_OTP_VERIFY,
            Constants.BCRM_NOTIFICATION_MYHOME_COT_CURRENT_OWNER_SUBMITTED,
            Constants.BCRM_NOTIFICATION_MYHOME_COT_OTP_VERIFY,
            Constants.BCRM_NOTIFICATION_MYHOME_COT_REQUEST,
            Constants.BCRM_NOTIFICATION_MYHOME_COT_REMINDER
        };
    }
}

