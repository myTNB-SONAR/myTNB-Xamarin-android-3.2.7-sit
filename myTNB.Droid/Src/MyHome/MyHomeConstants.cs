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
        internal static readonly string ACTION_OPEN_FILE = "mytnbapp://action=openPDF";
        internal static readonly string ACTION_DOWNLOAD_FILE = "mytnbapp://action=downloadPDF";
        internal static readonly string ACTION_RATE_SUCCESSFUL = "mytnbapp://action=rateSuccessful";
        internal static readonly string ACTION_SHOW_LATEST_BILL = "mytnbapp://action=showLatestBill";
        internal static readonly string ACTION_SHOW_PAYMENT_DETAILS = "mytnbapp://action=showPaymentDetails";
        internal static readonly string ACTION_SHOW_PAYMENT_HISTORY = "mytnbapp://action=showPaymentHistory";
        internal static readonly string ACTION_SHOW_PAYMENT = "mytnbapp://action=showPayment";
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
        internal static readonly string PAYMENT_HISTORY_PAYMENT = "PAYMENT";
        internal static readonly string PAYMENT_CA = "CA";
        internal static readonly string PAYMENT_IS_OWNED = "IsOwned";
        internal static readonly string PAYMENT_IS_OWNER = "isOwner";
        internal static readonly string PAYMENT_ACCOUNT_NICKNAME = "accountDesc";
        internal static readonly string PAYMENT_ACCOUNT_PREMISE = "premise";
        internal static readonly string PAYMENT_ACCOUNT_TYPE = "accountType";
        internal static readonly string PAYMENT_DETAILS_MODEL = "paymentDetailsModel";
        internal static readonly int PAYMENT_ADD_CARD_REQUEST_CODE = 1001;
        internal static readonly string REGISTERED_CARDS = "registeredCards";
        internal static readonly string APPLICATION_PAYMENT_ACCOUNT_NAME = "accountName";
        internal static readonly string APPLICATION_PAYMENT_PREMISE = "premise";
        internal static readonly string APPLICATION_PAYMENT_DETAIL = "applicationPaymentDetail";
        internal static readonly string APPLICATION_PAYMENT_MOBILE_NO = "mobileNo";
        internal static readonly string APPLICATION_PAYMENT_TYPE = "applicationType";
        internal static readonly string APPLICATION_PAYMENT_SEARCH_TERM = "searchTerm";
        internal static readonly string APPLICATION_PAYMENT_SYSTEM = "system";
        internal static readonly string APPLICATION_PAYMENT_STATUS_ID = "statusId";
        internal static readonly string APPLICATION_PAYMENT_STATUS_CODE = "statusCode";
        internal static readonly string APPLICATION_PAYMENT_SR_NUMBER = "srNumber";
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
            Constants.BCRM_NOTIFICATION_MYHOME_COT_CURRENT_OWNER_OTP_VERIFY,
            Constants.BCRM_NOTIFICATION_MYHOME_COT_REQUEST,
            Constants.BCRM_NOTIFICATION_MYHOME_COT_REMINDER
        };
    }
}

