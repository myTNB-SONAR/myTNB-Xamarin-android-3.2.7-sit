using System.IO;

namespace myTNB_Android.Src.Utils
{
    public sealed class Constants
    {
        internal static string LIBRARY_PATH => System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        internal static readonly string DB_NAME = "myTNB.db3";
        internal static string DB_PATH => Path.Combine(LIBRARY_PATH, DB_NAME);

        internal static readonly string DEVICE_PLATFORM = "1"; // 1 for Android (for iOS its 2)

        internal static readonly string FROM_ACTIVITY = ".fromActivity";

        internal static readonly int INACTIVE = 0;
        internal static readonly int ACTIVE = 1;

        internal static readonly string BARCODE_RESULT = ".barCodeResult";

        internal static readonly string SELECTED_ACCOUNT = ".selectedAccount";
        internal static readonly string SELECTED_FEEDBACK_STATE = ".selectedFeedbackState";
        internal static readonly string SELECTED_FEEDBACK_TYPE = ".selectedFeedbackType";
        internal static readonly string SELECTED_FEEDBACK = ".selectedFeedback";
        internal static readonly string SELECTED_ACCOUNT_POSITION = ".selectedAccountPosition";
        internal static readonly string SELECTED_BILL = ".selectedBill";
        internal static readonly string SELECTED_ACCOUNT_USAGE = ".selectedAccountUsage";
        internal static readonly string SELECTED_CHARTYPE = ".selectedChartType";
        internal static readonly string SELECTED_FEEDBACK_DETAIL_IMAGE = ".selectedFeedbackDetailImage";
        internal static readonly string NO_INTERNET_CONNECTION = ".noInternetConnection";
        internal static readonly string BILL_RESPONSE = ".billResponse";
        internal static readonly string PAYMENT_RESPONSE = ".paymentResponse";
        internal static readonly string USER_CREDENTIALS_ENTRY = ".userCredentialsEntry";
        internal static readonly string RETRIEVE_PIN_FROM_SMS = ".retrievedPinFromSMS";
        internal static readonly string SELECTED_NOTIFICATION_DETAIL_ITEM = ".selectedNotificationDetailItem";
        internal static readonly string SELECTED_NOTIFICATION_LIST_ITEM = ".selectedNotificationListItem";
        internal static readonly string SELECTED_NOTIFICATION_ITEM_POSITION = ".selectedNotificationItemPosition";
        internal static readonly string ACTION_IS_READ = ".action_is_read";
        internal static readonly string ACTION_IS_DELETE = ".action_is_deleted";
        internal static readonly string REMOVED_CREDIT_CARD = ".removedCreditCard";
        internal static readonly string CREDIT_CARD_LIST = ".creditCardList";
        internal static readonly string MANAGE_SUPPLY_ACCOUNT_REMOVE_ACTION = ".manageSupplyAccountRemoveAction";
        internal static readonly string MANAGE_SUPPLY_ACCOUNT_UPDATE_ACTION = ".manageSupplyAcccountUpdateAction";
        internal static readonly string HAS_NOTIFICATION = ".hasNotification";
        internal static readonly string SELECTED_WEBLINK = ".selectedWebLink";
        internal static readonly string SELECTED_RATING = ".selectedRating";
        internal static readonly string PROMOTIONS_LINK = ".promotionsLink";
        internal static readonly string SMART_METER_LINK = ".smartMeterLink";

        internal static readonly string RESPONSE_FEEDBACK_DATE = ".feedbackDate";
        internal static readonly string RESPONSE_FEEDBACK_ID = ".feedbackId";

        internal static readonly string ENTERED_USERNAME = ".enteredUsername";
        internal static readonly string ENTERED_PASSWORD = ".enteredPassword";

        internal static readonly string SELECTED_ERROR = ".errorMSg";
        internal static readonly string SELECTED_ERROR_MSG = ".errorMessage";

        internal static readonly int VIEW_TYPE_REAL_RECORD = 0;
        internal static readonly int VIEW_TYPE_DUMMY_RECORD = 1;

        internal static readonly int GRID_IMAGE_COUNT = 3;

        internal static readonly string ZERO_INDEX_FILTER = "0000000";
        internal static readonly string ZERO_INDEX_TITLE = "All Notifications";

        internal static readonly string NO_SM_DATA_FOUND = ".noDataFound";

        /// <summary>
        /// BARCODE REQUEST TYPE
        /// </summary>
        internal static readonly int BARCODE_REQUEST_CODE = 0x01;

        internal static readonly int SELECT_ACCOUNT_REQUEST_CODE = 0x02;
        internal static readonly int SELECT_FEEDBACK_STATE = 0x02;
        internal static readonly int SELECT_FEEDBACK_TYPE = 0x02;

        internal static readonly int NOTIFICATION_DETAILS_REQUEST_CODE = 0x03;

        internal static readonly int NOTIFICATION_FILTER_REQUEST_CODE = 0x04;

        internal static readonly int UPDATE_MOBILE_NO_REQUEST = 0x05;

        internal static readonly int UPDATE_PASSWORD_REQUEST = 0x06;

        internal static readonly int MANAGE_CARDS_REQUEST = 0x07;

        internal static readonly int MANAGE_SUPPLY_ACCOUNT_REQUEST = 0x08;

        internal static readonly int REQUEST_ATTACHED_CAMERA_IMAGE = 0x09;
        internal static readonly int REQUEST_ATTACHED_GALLERY_IMAGE = 0x0a;

        internal static readonly int REQUEST_FEEDBACK_SUCCESS_VIEW = 0x00;
        internal static readonly int REQUEST_FEEDBACK_FAIL_VIEW = 0x01;



        internal static readonly int UPDATE_NICKNAME_REQUEST = 0x09;
        internal static readonly int RUNTIME_PERMISSION_SMS_REQUEST_CODE = 0x03;
        internal static readonly int RUNTIME_PERMISSION_PHONE_REQUEST_CODE = 0x04;
        internal static readonly int RUNTIME_PERMISSION_STORAGE_REQUEST_CODE = 0x05;
        internal static readonly int RUNTIME_PERMISSION_CAMERA_REQUEST_CODE = 0x06;
        internal static readonly int RUNTIME_PERMISSION_GALLERY_REQUEST_CODE = 0x07;
        internal static readonly int RUNTIME_PERMISSION_LOCATION_REQUEST_CODE = 0x08;



        //internal static readonly string API_KEY_ID = "9515F2FA-C267-42C9-8087-FABA77CB84DF";

        internal static readonly int PLAY_SERVICES_RESOLUTION_REQUEST = 9000;

        internal struct SERVER_URL
        {
#if DEBUG || STUB
            // internal static readonly string END_POINT = "http://10.215.128.191:89";
            // internal static readonly string FPX_PAYMENT = "http://10.215.128.191:89/v5/PayRedirect.aspx?Param1=3&Param2=";
            internal static readonly string END_POINT = "https://mobiletestingws.tnb.com.my";
            internal static readonly string FPX_PAYMENT = "https://mobiletestingws.tnb.com.my/v5/PayRedirect.aspx?Param1=3&Param2=";
#elif DEVELOP || SIT
            internal static readonly string END_POINT = "https://mobiletestingws.tnb.com.my";
            internal static readonly string FPX_PAYMENT = "https://mobiletestingws.tnb.com.my/v5/PayRedirect.aspx?Param1=3&Param2=";
            //internal static readonly string END_POINT = "https://mytnbapp.tnb.com.my";
            //internal static readonly string FPX_PAYMENT = "https://mytnbapp.tnb.com.my/v5/PayRedirect.aspx?Param1=3&Param2=";
#else 
           internal static readonly string END_POINT = "https://mytnbapp.tnb.com.my";
            internal static readonly string FPX_PAYMENT = "https://mytnbapp.tnb.com.my/v5/PayRedirect.aspx?Param1=3&Param2=";

#endif

        }

        internal struct APP_CONFIG
        {
#if DEBUG || STUB || DEVELOP || SIT
            internal static readonly string API_KEY_ID = "9515F2FA-C267-42C9-8087-FABA77CB84DF";
            //internal static readonly string API_KEY_ID = "E6148656-205B-494C-BC95-CC241423E72F";
            internal static readonly int MAX_IMAGE_QUALITY_IN_PERCENT = 100;
            internal static readonly int IN_SAMPLE_SIZE = 4;
#else //create new flavour called PROD, SIT, UAT
                internal static readonly string API_KEY_ID = "E6148656-205B-494C-BC95-CC241423E72F";
                internal static readonly int MAX_IMAGE_QUALITY_IN_PERCENT = 100;
                internal static readonly int IN_SAMPLE_SIZE = 4;
#endif
        }

        //Notification Codes
        internal static string NOTIFICATION_CODE_BP = "BP";
        internal static string NOTIFICATION_CODE_ACC = "ACC";
        internal static string NOTIFICATION_CODE_PO = "PO";
        internal static string NOTIFICATION_CODE_PRO = "PRO";
        internal static string NOTIFICATION_CODE_REW = "REW";

        //Feedback
        internal static readonly int FEEDBACK_CHAR_LIMIT = 250;

        //Update phone number
        internal static readonly string FORCE_UPDATE_PHONE_NO = ".updatePhoneNo";
        internal static readonly string NEW_PHONE_NO = ".newPhoneNo";
        internal static readonly string FROM_APP_LAUNCH = ".fromAppLaunch";
        internal static readonly int REQUEST_VERIFICATION_SMS_TOEKN_CODE = 4812;

        //Promotions 
#if DEBUG || STUB || DEVELOP || SIT
        internal static readonly int PROMOTION_DAYS_COUNTER_LIMIT = 1;
#else
        internal static readonly int PROMOTION_DAYS_COUNTER_LIMIT = 15;
#endif

        internal static readonly string PROMOTION_NOTIFICATION_VIEW = ".viewPromotion";

        //Rate Us Feedback
        internal static readonly string QUESTION_ID_CATEGORY = ".questionIDCategory";
        internal static readonly string QUESTION_TYPE_RATING = "Rating";
        internal static readonly string QUESTION_TYPE_COMMENTS = "Multiline Comment";
        internal static readonly string DEVICE_ID_PARAM = ".deviceID";
        internal static readonly string MERCHANT_TRANS_ID = ".merchantTransId";

        //Downtime 
        internal static readonly string BCRM_SYSTEM = "BCRM";
        internal static readonly string SSP_SYSTEM = "SSP";
        internal static readonly string SMART_METER_SYSTEM = "SmartMeter";
        internal static readonly string PG_CC_SYSTEM = "PG_CC";
        internal static readonly string PG_FPX_SYSTEM = "PG_FPX";

        internal static readonly string ACCOUNT_REMOVED_FLAG = "ACCOUNT_REMOVED";


        //RE account date increment
        internal static int RE_ACCOUNT_DATE_INCREMENT_DAYS = 30;

        //FAQ ID
        internal static readonly string FAQ_ID_PARAM = ".faqID";

        //Summary dashboard
        internal static readonly int SUMMARY_DASHBOARD_PAGE_COUNT = 5;

        //Add account limit
        internal static readonly int ADD_ACCOUNT_LIMIT = 10;

        //Maintenance
        internal static readonly string MAINTENANCE_MODE = "MAINTENANCE";
        internal static readonly string MAINTENANCE_TITLE_KEY = "maintenanceTitle";
        internal static readonly string MAINTENANCE_MESSAGE_KEY = "maintenanceMessage";

        //Itemized Billing
        internal static readonly string ITEMZIED_BILLING_VIEW_KEY = "itemizedBilling";


        //Refresh View
        internal static readonly string REFRESH_MODE = "FAILED";
        internal static readonly string REFRESH_MSG = "refreshMsg";
        internal static readonly string REFRESH_BTN_MSG = "refreshBtnTxt";

        internal static readonly string AMOUNT_DUE_FAILED_KEY = "AmountDueFailedKey";

        //Home Menu Scroll
        internal static readonly int ACCOUNT_LIST_CARD_DP = 60;
        internal static readonly int ACCOUNT_LIST_INDICATOR_DP = 10;
        internal static readonly int ACCOUNT_LIST_SERVICE_MAX_BOUNDARY = 3;
        internal static readonly int MY_SERVICE_CARD_DP = 90;
        internal static readonly int MY_SERVICE_NO_CARD_DP = 5;
        internal static readonly int ACCOUNT_LIST_HELP_NO_ACC_DP_LIMIT = 110;
        internal static readonly int ACCOUNT_LIST_HELP_MAX_BOUNDARY = 5;

        //Language
        internal static readonly string DEFAULT_LANG = "EN";

        // SMR
        internal static readonly string SMR_RESPONSE_KEY = "smrReponse";
        internal static readonly string SMR_SUBMIT_METER_KEY = "S";
        internal static readonly string SMR_VIEW_METER_KEY = "V";
        internal static readonly string SMR_TERMINATION_REASON_KEY = "smrTerminationReason";
        internal static readonly string SMR_TERMINATION_KEY = "smrTermination";

        internal static readonly string AMOUNT_DUE_RESPONSE_KEY = ".amountDueResponse";

        internal static readonly string SELECTED_ACCOUNT_USAGE_RESPONSE = ".selectedAccountUsageResponse";

        internal static readonly string IS_BILLING_AVAILABLE_KEY = ".isBillingAvailable";

        public enum GREETING
        {
            MORNING,
            AFTERNOON,
            EVENING
        }
    }
}