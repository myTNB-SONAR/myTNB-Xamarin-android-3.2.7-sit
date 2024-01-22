using System.IO;

namespace myTNB_Android.Src.Utils
{
    public sealed class Constants
    {
        internal static string LIBRARY_PATH => System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        internal static readonly string DB_NAME = "myTNB.db3";
        internal static string DB_PATH => Path.Combine(LIBRARY_PATH, DB_NAME);

        internal static readonly string DEVICE_PLATFORM = "1"; // 1 for Android (for iOS its 2)

        internal static readonly int INACTIVE = 0;
        internal static readonly int ACTIVE = 1;

        // Activity Intent IDs
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
        internal static readonly string SELECTED_FROMDASHBOARD_NOTIFICATION_DETAIL_ITEM = ".selectedFromDashboardNotificationDetailItem";
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
        internal static readonly string APP_NAVIGATION_KEY = ".appNavigation";
        internal static readonly string FROM_BILL_DETAILS_PAGE = ".fromBillDetailsPage";
        internal static readonly string SELECT_COUNTRY_CODE = ".selectCountryCode";
        internal static readonly string SELECT_REGISTERED_OWNER = ".registedOwner";
        internal static readonly string OWNER_RELATIONSHIP = ".ownerRelationship";
        internal static readonly string ACCOUNT_OWNER_NAME = ".accOwnerName";
        internal static readonly string ACCOUNT_MOBILE_NUMBER = ".mobileNumber";
        internal static readonly string ACCOUNT_EMAIL_ADDRESS = ".emailAddress";
        internal static readonly string ACCOUNT_MAILING_ADDRESS = ".mailingAddress";
        internal static readonly string ACCOUNT_PREMISE_ADDRESS = ".premiseAddress";
        internal static readonly string ACCOUNT_IC_NUMBER = ".icNumber";
        internal static readonly string ADAPTER_TYPE = ".adapterType";
        internal static readonly string IMAGE_OWNER = ".imageOwner";
        internal static readonly string IMAGE_OWN = ".imageOwn";
        internal static readonly string IMAGE_SUPPORTING_DOC = ".imageSupportingDoc";
        internal static readonly string IMAGE_PERMISES = ".imagePermises";
        internal static readonly string ENTERED_NAME = ".enteredName";

        internal static readonly string PAGE_TITLE = ".pageTitle";
        internal static readonly string PAGE_STEP_TITLE = ".pageStepTitle";
        internal static readonly string ACCOUNT_NUMBER = ".accountNumber";
        internal static readonly string IS_OWNER = ".isOwner";
        internal static readonly string ENQUIRYID = ".enquiryId";
        internal static readonly string ENQUIRYNAME = ".enquiryName";
        internal static readonly string REQ_EMAIL = ".requesterEmail";
        internal static readonly string REQ_IC = ".requesterIC";

        internal static readonly int VIEW_TYPE_REAL_RECORD = 0;
        internal static readonly int VIEW_TYPE_DUMMY_RECORD = 1;

        internal static readonly int GRID_IMAGE_COUNT = 3;

        internal static readonly string ZERO_INDEX_FILTER = "0000000";
        internal static readonly string ZERO_INDEX_TITLE = "All Notifications";

        internal static readonly string NO_SM_DATA_FOUND = ".noDataFound";

        internal static readonly string ACCT_STMNT_EMPTY = ".accountStatementEmpty";
        internal static readonly string ACCT_STMNT_PDF_FILE_PATH = ".accountStatementFilePath";
        internal static readonly string BILL_HISTORY_IS_EMPTY = ".billHistoryIsEmpty";

        internal static readonly string DELETE_DRAFT_MESSAGE = ".deleteDraftMessage";

        internal static readonly string PDF_IMAGE_VIWER_FILE_PATH = ".pdfImageViewerfilePath";
        internal static readonly string PDF_IMAGE_VIEWER_EXTENSION = ".pdfImageViewerFileExtension";
        internal static readonly string PDF_FILE_TITLE = ".pdfImageViewerFileTitle";
        internal static readonly string PDF_FILE_EXTENSION = "pdf";

        /// <summary>
        /// BARCODE REQUEST TYPE
        /// </summary>
        internal static readonly int BARCODE_REQUEST_CODE = 0x01;

        internal static readonly int SELECT_ACCOUNT_REQUEST_CODE = 0x02;
        internal static readonly int NEW_BILL_REDESIGN_REQUEST_CODE = 0x12;
        internal static readonly int ACCTSTMNT_PDFVIEW_REQUEST_CODE = 0x14;
        internal static readonly int SELECT_FEEDBACK_STATE = 0x02;
        internal static readonly int SELECT_FEEDBACK_TYPE = 0x02;
        internal static readonly int MYHOME_MICROSITE_REQUEST_CODE = 0x16;

        internal static readonly int NOTIFICATION_LISTING_REQUEST_CODE = 29813;
        internal static readonly int NOTIFICATION_DETAILS_REQUEST_CODE = 0x03;

        internal static readonly int NOTIFICATION_FILTER_REQUEST_CODE = 0x04;

        internal static readonly int UPDATE_MOBILE_NO_REQUEST = 0x05;

        internal static readonly int UPDATE_PASSWORD_REQUEST = 0x06;

        internal static readonly int MANAGE_CARDS_REQUEST = 0x07;

        internal static readonly int UPDATE_EMAIL_REQUEST = 0x010;

        internal static readonly int UPDATE_NAME_REQUEST = 0x011;

        internal static readonly int UPDATE_IC_REQUEST = 0x15;

        internal static readonly int MANAGE_SUPPLY_ACCOUNT_REQUEST = 0x08;

        internal static readonly int UPDATE_ID_REQUEST = 0x013;

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
        internal static readonly int RUNTIME_PERMISSION_GALLERY_PDF_REQUEST_CODE = 0x10;
        internal static readonly int RUNTIME_PERMISSION_CALENDAR_REQUEST_CODE = 0x11;
        internal static readonly int RUNTIME_PERMISSION_NOTIFICATION_REQUEST_CODE = 0x12;
        internal static readonly int SELECT_ENQUIRY_REQUEST_CODE = 0x13;

        internal static readonly int PLAY_SERVICES_RESOLUTION_REQUEST = 9000;



        internal struct SERVER_URL
        {
            internal static readonly string END_POINT = myTNB.Mobile.MobileConstants.ApiDomain;
            internal static readonly string END_POINT_AWS = myTNB.Mobile.MobileConstants.AWSApiDomain;
            //internal static readonly string END_POINT_ISUSERAUTH_AWS = myTNB.Mobile.MobileConstants.IsUserAuthAWSApiDomain;

#if DEBUG || STUB
            //internal static readonly string END_POINT = "http://10.215.128.191:89";  //dev
            //internal static readonly string END_POINT = "https://mobiletestingws.tnb.com.my";
            //internal static readonly string END_POINT = "http://10.215.128.191:97";  //dev cep
            internal static readonly string FIREBASE_DEEP_LINK_END_POINT = "https://mytnbappsit.page.link"; //sit
#elif DEVELOP || SIT
            //internal static readonly string END_POINT = "http://10.215.128.191:99";  //dev
            //internal static readonly string END_POINT = "https://mobiletestingws.tnb.com.my";
            internal static readonly string FIREBASE_DEEP_LINK_END_POINT = "https://mytnbappsit.page.link";
#else
            internal static readonly string FIREBASE_DEEP_LINK_END_POINT = "https://mytnbapp.page.link";
#endif
        }

        internal struct APP_CONFIG
        {
#if DEBUG || STUB || DEVELOP || SIT
            internal static readonly string API_KEY_ID = "9515F2FA-C267-42C9-8087-FABA77CB84DF";  // DEV && SIT
            //internal static readonly string API_KEY_ID = "E6148656-205B-494C-BC95-CC241423E72F";  // PROD
            internal static readonly int MAX_IMAGE_QUALITY_IN_PERCENT = 100;
            internal static readonly int IN_SAMPLE_SIZE = 4;
            internal static readonly string ENV = "SIT";
#else //create new flavour called PROD, SIT, UAT
            internal static readonly string API_KEY_ID = myTNB.Mobile.MobileConstants.ApiKeyId;
            internal static readonly int MAX_IMAGE_QUALITY_IN_PERCENT = 100;
            internal static readonly int IN_SAMPLE_SIZE = 4;
            internal static readonly string ENV = "PROD";
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
        internal static readonly string QUESTION_TYPE_RATING_EB_FEEDBACK_ONE = "RatingFeedBackOne";
        internal static readonly string QUESTION_TYPE_RATING_EB_FEEDBACK_TWO = "RatingFeedBackTwo";

        //Downtime
        internal static readonly string BCRM_SYSTEM = "BCRM";
        internal static readonly string SSP_SYSTEM = "SSP";
        internal static readonly string SMART_METER_SYSTEM = "SmartMeter";
        internal static readonly string PG_CC_SYSTEM = "PG_CC";
        internal static readonly string RPS_CC_SYSTEM = "RPS_CC";
        internal static readonly string PG_FPX_SYSTEM = "PG_FPX";
        internal static readonly string PG_TNG_SYSTEM = "PG_TNG";
        internal static readonly string Smart_Meter_Daily_SYSTEM = "SmartMeterDaily";
        internal static readonly string PG_SYSTEM = "PG";
        internal static readonly string EB_SYSTEM = "EB";
        internal static readonly string TRIL_SYSTEM = "TRIL";
        internal static readonly string SAGE_SYSTEM = "SAGE";
        internal static readonly string CatchupCell = "CatchupCell";
        internal static readonly string CatchupRF = "CatchupRF";
        internal static readonly string CatchupPLC = "CatchupPLC";
        internal static readonly string BCRM_RS_SYSTEM = "BCRM_RS";
        internal static readonly string PG_TNG_REMINDER = "PG_TNG_Reminder";


        internal static readonly string ACCOUNT_REMOVED_FLAG = "ACCOUNT_REMOVED";

        //RE account date increment
        internal static int RE_ACCOUNT_DATE_INCREMENT_DAYS = 30;

        //FAQ ID
        internal static readonly string FAQ_ID_PARAM = ".faqID";

        //Add User Access 
        internal static readonly int ADD_USER = 1;

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
        internal static readonly string DEFAULT_COUNTRY_CODE = "MY";

        // SMR
        internal static readonly string SMR_RESPONSE_KEY = "smrReponse";
        internal static readonly string SMR_SUBMIT_METER_KEY = "S";
        internal static readonly string SMR_VIEW_METER_KEY = "V";
        internal static readonly string SMR_TERMINATION_REASON_KEY = "smrTerminationReason";
        internal static readonly string SMR_TERMINATION_KEY = "smrTermination";
        internal static readonly string SMR_ENABLE_FLAG = "ENABLE_SMR";
        internal static readonly string SMR_DISABLE_FLAG = "DISABLE_SMR";

        internal static readonly string SMR_METER_UNIT_KWH = "KWH";
        internal static readonly string SMR_METER_UNIT_KVAR = "KVARH";
        internal static readonly string SMR_METER_UNIT_KW = "KW";

        internal static readonly string AMOUNT_DUE_RESPONSE_KEY = ".amountDueResponse";

        internal static readonly string SELECTED_ACCOUNT_USAGE_RESPONSE = ".selectedAccountUsageResponse";

        internal static readonly string SELECTED_SM_ACCOUNT_USAGE_RESPONSE = ".selectedSMAccountUsageResponse";

        internal static readonly string IS_BILLING_AVAILABLE_KEY = ".isBillingAvailable";

        internal static readonly string PROJECTED_COST_KEY = "PROJECTEDCOST";

        internal static readonly string CURRENT_COST_KEY = "CURRENTCOST";

        internal static readonly string AVERAGE_USAGE_KEY = "AVERAGEUSAGE";

        internal static readonly string CURRENT_USAGE_KEY = "CURRENTUSAGE";

        internal static readonly string MISSING_READING_KEY = "MISSINGREADING";

        internal static readonly string ENERGY_DISCONNECTION_KEY = "AVAILABLE";

        internal static readonly int SELECT_ACCOUNT_PDF_REQUEST_CODE = 9078;
        internal static readonly int SELECT_ACCOUNT_DBR_REQUEST_CODE = 9079;

        internal static readonly string CODE_KEY = "CODE_KEY";
        internal static readonly string DBR_KEY = "DBR_KEY";

        public enum GREETING
        {
            MORNING,
            AFTERNOON,
            EVENING
        }

        public enum SUPPORTED_LANGUAGES
        {
            EN, MS
        }

        //User Notification BCRM IDs
        public const string BCRM_NOTIFICATION_NEW_BILL_ID = "01";
        public const string BCRM_NOTIFICATION_BILL_DUE_ID = "02";
        public const string BCRM_NOTIFICATION_DISCONNECT_NOTICE_ID = "03";
        public const string BCRM_NOTIFICATION_DISCONNECTED_ID = "04";
        public const string BCRM_NOTIFICATION_RECONNECTED_ID = "05";
        public const string BCRM_NOTIFICATION_MAINTENANCE_ID = "99";
        public const string BCRM_NOTIFICATION_METER_READING_OPEN_ID = "0009";
        public const string BCRM_NOTIFICATION_METER_READING_REMIND_ID = "0010";
        public const string BCRM_NOTIFICATION_SMR_DISABLED_ID = "0011";
        public const string BCRM_NOTIFICATION_SMR_APPLY_SUCCESS_ID = "50";
        public const string BCRM_NOTIFICATION_SMR_APPLY_FAILED_ID = "51";
        public const string BCRM_NOTIFICATION_SMR_DISABLED_SUCCESS_ID = "52";
        public const string BCRM_NOTIFICATION_SMR_DISABLED_FAILED_ID = "53";
        public const string BCRM_NOTIFICATION_PAYMENT_FAILED_ID = "71";
        public const string BCRM_NOTIFICATION_PAYMENT_SUCCESS_ID = "72";

        


        //DBR
        public const string BCRM_NOTIFICATION_DBR_EMAIL = "21003";
        public const string BCRM_NOTIFICATION_DBR_EBILL = "21001";
        public const string BCRM_NOTIFICATION_DBR_EBILL_TENANT = "31002";
        public const string BCRM_NOTIFICATION_DBR_EBILL_OWNER = "31001";
        public const string BCRM_NOTIFICATION_DBR_PAPER = "21002";
        public const string BCRM_NOTIFICATION_DBR_EMAIL_REMOVED = "21004";
        public const string BCRM_NOTIFICATION_DBR_AUTO_OPTIN = "21011";

        //Energy Budget
        public const string BCRM_NOTIFICATION_ENERGY_BUDGET = "11001";
        public const string BCRM_NOTIFICATION_ENERGY_BUDGET_80 = "CEP AT";
        public const string BCRM_NOTIFICATION_ENERGY_BUDGET_100 = "CEP TH";
        public const string BCRM_NOTIFICATION_ENERGY_BUDGET_TC = "CEP TC";
        public const string BCRM_NOTIFICATION_ENERGY_BUDGET_RC = "CEP RC";
        public const string BCRM_NOTIFICATION_ENERGY_BUDGET_NEWS = "98";
        public const string NOTIFICATION_TYPE_ID_EB = "1000020";

        //Account Statement
        public const string BCRM_NOTIFICATION_ACCT_STATEMENT_READY = "20008";

        //EKYC
        public const string BCRM_NOTIFICATION_EKYC_ID_NOT_MATCHING = "20010";
        public const string BCRM_NOTIFICATION_EKYC_FAILED = "20011";
        public const string BCRM_NOTIFICATION_EKYC_THREE_TIMES_FAILURE = "20012";
        public const string BCRM_NOTIFICATION_EKYC_SUCCESSFUL = "20013";
        public const string BCRM_NOTIFICATION_EKYC_FIRST_NOTIFICATION = "20014";
        public const string BCRM_NOTIFICATION_EKYC_SECOND_NOTIFICATION = "20015";
        public const string BCRM_NOTIFICATION_EKYC_THIRD_PARTY_FAILED = "20018";
        public const string BCRM_NOTIFICATION_EKYC_THIRD_PARTY_THREE_TIMES_FAILURE = "20019";
        public const string BCRM_NOTIFICATION_EKYC_THIRD_PARTY_SUCCESSFUL = "20020";
        public const string BCRM_NOTIFICATION_EKYC_THIRD_PARTY_ID_NO_TMATCHING = "20021";

        //App Update
        public const string BCRM_NOTIFICATION_APP_UPDATE = "20006";
        public const string BCRM_NOTIFICATION_APP_UPDATE_2 = "20007";
        public const string BCRM_NOTIFICATION_APP_UPDATE_OT = "20009";
        public const string BCRM_NOTIFICATION_APP_UPDATE_DBR = "21010";

        //myHome
        
        public const string BCRM_NOTIFICATION_MYHOME_NC_RESUME_APPLICATION = "41002";
        public const string BCRM_NOTIFICATION_MYHOME_NC_OTP_VERIFY = "41005";
        public const string BCRM_NOTIFICATION_MYHOME_COA_OTP_VERIFY = "42005";
        public const string BCRM_NOTIFICATION_MYHOME_COT_CURRENT_OWNER_SUBMITTED = "43004";
        public const string BCRM_NOTIFICATION_MYHOME_COT_OTP_VERIFY = "43005";
        public const string BCRM_NOTIFICATION_MYHOME_COT_REQUEST = "43006";
        public const string BCRM_NOTIFICATION_MYHOME_COT_REMINDER = "43007";
        public const string BCRM_NOTIFICATION_MYHOME_COT_NEW_OWNER_RESUME_APPLICATION = "43008"; 
        public const string BCRM_NOTIFICATION_MYHOME_COA_RESUME_APPLICATION = "42004";
        public const string BCRM_NOTIFICATION_MYHOME_APP_UPDATE_GTM2 = "44001";
        public const string BCRM_NOTIFICATION_MYHOME_APP_UPDATE_GTM2_FOLLOW_UP_AFTER_UPDATE = "44002";
        public const string BCRM_NOTIFICATION_MYHOME_APP_UPDATE_GTM2_FOLLOW_UP_NOT_UPDATE = "44003";
        public const string BCRM_NOTIFICATION_MYHOME_COT_EO_OTP_VERIFY = "43003";

        //Bill Estimation
        public const string BCRM_NOTIFICATION_BILL_ESTIMATION_NEWS = "09";

        //Service Disruption
        public const string BCRM_NOTIFICATION_SERVICE_DISTRUPT_NEWS = "98";
        public const string BCRM_NOTIFICATION_SERVICE_DISTRUPT_OUTAGE = "SDOUT";
        public const string BCRM_NOTIFICATION_SERVICE_DISTRUPT_INPROGRESS = "SDIP";
        public const string BCRM_NOTIFICATION_SERVICE_DISTRUPT_RESTORATION = "SDOR";
        public const string BCRM_NOTIFICATION_SERVICE_DISTRUPT_UPDATE_NOW = "SDRC";
        public const string BCRM_NOTIFICATION_SERVICE_DISTRUPT_HEARTBEAT_INI = "SDHBI";
        public const string BCRM_NOTIFICATION_SERVICE_DISTRUPT_HEARTBEAT_UPDATE1 = "SDHBU1";
        public const string BCRM_NOTIFICATION_SERVICE_DISTRUPT_HEARTBEAT_UPDATE2 = "SDHBU2";
        public const string BCRM_NOTIFICATION_SERVICE_DISTRUPT_HEARTBEAT_UPDATE3 = "SDHBU3";
        public const string BCRM_NOTIFICATION_SERVICE_DISTRUPT_HEARTBEAT_UPDATE4 = "SDHBU4";
        public const string BCRM_NOTIFICATION_SERVICE_DISTRUPT_HEARTBEAT_FEEDBACK = "SDOR";
        public const string BCRM_NOTIFICATION_SERVICE_DISTRUPT_HEARTBEAT_FEEDBACK2 = "SDHBR";
        public const string BCRM_NOTIFICATION_SERVICE_DISTRUPT_HEARTBEAT_FEEDBACK3 = "SDFB";
        public const string NOTIFICATION_TYPE_ID_SD = "1000028";

        //OwnerTenant
        public const string BCRM_NOTIFICATION_NEW_ACCOUNT_ADDED = "10001";
        public const string ACCOUNT_PROFILENAME_PATTERN = "#profileName#";
        public const string ACCOUNT_ACCNO_PATTERN = "#accno#";
        public const string ACCOUNT_ACCNO_PATTERNS = "#accnos#";

        public const string BCRM_NOTIFICATION_REMOVE_ACCESS = "10002";
        public const string BCRM_NOTIFICATION_NEW_ACCESS_ADDED = "10003";
        public const string BCRM_NOTIFICATION_UPDATE_ACCESS = "10004";

        public const string ACCOUNT_NICKNAME_PATTERN = "#accountNickname#";
        public const string ACCOUNT_FULLNAME_EMAIL_PATTERN = "#accountFullname/accountEmailAddress#";
        public const string ACCOUNT_FULLNAME_PATTERN = "#OwnerName#";
        public const string ACCOUNT_ADDRESS_PATTERN = "#MaskedAddress#";
        //public const string ACCOUNT_FULLNAME_PATTERN = "#accountFullname/accountEmailAddress#";

        public const string APP_TUTORIAL_PATTERN = "#dropdown#";

        public const string REWARDS_ITEM_LIST_MODE = ".rewardItemList";
        public const string REWARDS_ITEM_LIST_SEARCH_STRING_KEY = ".rewardSearchKey";

        public const string WHATSNEW_ITEM_LIST_MODE = ".whatsNewItemList";
        public const string WHATSNEW_ITEM_LIST_SEARCH_STRING_KEY = ".whatsNewSearchKey";

        public const string REWARD_DETAIL_TITLE_KEY = ".rewardDetailTitle";
        public const string REWARD_DETAIL_ITEM_KEY = ".rewardDetailItem";

        public const string WHATS_NEW_DETAIL_TITLE_KEY = ".whatsNewDetailTitle";
        public const string WHATS_NEW_DETAIL_ITEM_KEY = ".whatsNewDetailItem";

        public const string IN_APP_LINK = ".inAppLink";
        public const string IN_APP_TITLE = ".inAppTitle";

        public enum REWARDSITEMLISTMODE
        {
            INITIATE,
            LOADED
        }

        public enum WHATSNEWITEMLISTMODE
        {
            INITIATE,
            LOADED
        }

        //Timeout Duration
        internal static readonly int SERVICE_TIMEOUT_DEFAULT = 60000;
        internal static readonly int ACCOUNT_LIST_TIMEOUT = 8000;
        internal static readonly int APP_LAUNCH_MASTER_DATA_TIMEOUT = 3000;
        internal static readonly int APP_LAUNCH_MASTER_DATA_RETRY_TIMEOUT = 2000;
        internal static readonly int PAYMENT_RECEIPT_TIMEOUT = 30000;
        internal static readonly int SPLASHSCREEN_DOWNLOAD_TIMEOUT_MILISEC = 4000;

        //SharedPreference ids
        public const string ACCOUNT_SHARED_PREF_ID = "myTNB.account.pref";
        public const string SHARED_PREF_LANGUAGE_KEY = "myTNB.account.pref.language";
        public const string SHARED_PREF_LANGUAGE_IS_CHANGE_KEY = "myTNB.account.pref.language.is.changed";
        public const string SHARED_PREF_DEVICE_ID_KEY = "myTNB.account.pref.device.id";
        public const string SHARED_PREF_SAVED_LANG_PREF_RESULT_KEY = "myTNB.account.pref.saved.lang.pref.result";

        //Service Error Codes
        internal static readonly string SERVICE_CODE_SUCCESS = "7200";
        internal static readonly string SERVICE_CODE_MAINTENANCE = "7000";

        // Firebase Dynamic Link
        internal static readonly int DYNAMIC_LINK_ANDROID_MIN_VER_CODE = 171;
        internal static readonly string DYNAMIC_LINK_IOS_MIN_VER_CODE = "2.1.0";
        internal static readonly string DYNAMIC_LINK_IOS_APP_ID = "1297089591";

        internal static readonly string ITEMIZED_BILLING_ADVICE_KEY = "ADVICE";
        internal static readonly string ITEMIZED_BILLING_BILL_KEY = "BILL";
        internal static readonly string ITEMIZED_BILLING_PAYMENT_KEY = "PAYMENT";

        internal static readonly int LANGUAGE_MASTER_DATA_CHECK_TIMEOUT = 500;
        internal static readonly int FONT_MASTER_DATA_CHECK_TIMEOUT = 500;

        internal static readonly int REWARDS_DATA_CHECK_TIMEOUT = 500;

        internal static readonly string APPLICATION_STATUS_FILTER_TYPE_KEY = "applicationType";
        internal static readonly string APPLICATION_STATUS_FILTER_STATUS_KEY = "status";
        internal static readonly string APPLICATION_STATUS_FILTER_DATE_KEY = "date";
        internal static readonly string APPLICATION_STATUS_FILTER_FROM_DATE_KEY = "fromdate";
        internal static readonly string APPLICATION_STATUS_FILTER_TO_DATE_KEY = "todate";
        internal static readonly string APPLICATION_STATUS_TYPE_LIST_KEY = "typeList";
        internal static readonly string APPLICATION_STATUS_STATUS_LIST_KEY = "statusList";
        internal static readonly string APPLICATION_STATUS_SEARCH_BY_LIST_KEY = "searchByList";
        internal static readonly string APPLICATION_STATUS_FILTER_REQUEST_KEY = "filterRequest";
        internal static readonly string APPLICATION_STATUS_FILTER_INDIVIDUAL_CLEAR_KEY = "individualClear";
        internal static readonly string APPLICATION_STATUS_DETAIL_TITLE_KEY = "applicationStatusTitle";
        internal static readonly string APPLICATION_STATUS_DETAIL_RELOAD = "applicationStatusToBeReloaded";
        internal static readonly string APPLICATION_STATUS_DETAIL_RATED_TOAST_MESSAGE = "applicationStatusRatedToastMessage";
        internal static readonly string APPLICATION_STATUS_SMRTYPE_LIST_KEY = "SMRtypeList";
        internal static readonly int APPLICATION_STATUS_FILTER_REQUEST_CODE = 29800;
        internal static readonly int APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE = 29801;
        internal static readonly int APPLICATION_STATUS_FILTER_STATUS_REQUEST_CODE = 29802;
        internal static readonly int APPLICATION_STATUS_FILTER_DATE_REQUEST_CODE = 29803;
        internal static readonly int APPLICATION_STATUS_FILTER_SEARCH_BY_REQUEST_CODE = 29804;
        internal static readonly int APPLICATION_STATUS_SEARCH_DETAILS_REQUEST_CODE = 29805;
        internal static readonly int APPLICATION_STATUS_RATING_REQUEST_CODE = 29806;
        internal static readonly int APPLICATION_STATUS_DETAILS_REQUEST_CODE = 29807;
        internal static readonly int APPLICATION_STATUS_DETAILS_SCHEDULER_REQUEST_CODE = 29808;
        internal static readonly int APPLICATION_STATUS_SUBMIT_APPLICATION_RATING_REQUEST_CODE = 29809;
        internal static readonly int APPLICATION_STATUS_START_RESUME_REQUEST_CODE = 29810;
        internal static readonly int APPLICATION_STATUS_DETAILS_FROM_NOTIFICATION_DETAILS_REQUEST_CODE = 29811;
        internal static readonly int APPLICATION_STATUS_LANDING_FROM_DASHBOARD_REQUEST_CODE = 29812;
        internal static readonly int APPLICATION_STATUS_DETAIL_FROM_DASHBOARD_REQUEST_CODE = 29814;
        internal static readonly int APPLICATION_STATUS_FILTER_SMRTYPE_REQUEST_CODE = 29815;

        internal static readonly string WEBVIEW_PAYMENT = "WebViewPayment";
        internal static readonly string WEBVIEW_PAYMENT_FPX = "WebViewPaymentFPX";
        internal static readonly string WEBVIEW_PAYMENT_CC = "WebViewPaymentCC";
        internal static readonly string WEBVIEW_PAYMENT_SUCCESS = "WebViewPayment_Success";
        internal static readonly string WEBVIEW_PAYMENT_FAIL = "WebViewPayment_Fail";
        internal static readonly string WEBVIEW_PAYMENT_FINISH_DASHBOARD = "WebViewPaymentFINISH_DASHBOARD";
        internal static readonly string DYNA_SITECORE_REFFER_LOCAL = "sitecore_refer_local";
        internal static readonly string DYNA_SITECORE_REFFER_ONLINE = "sitecore_refer_online";
        internal static readonly string DYNA_WHATS_NEW_DEFAULT = "WhatsNewClicked";

        //Utility
        internal static readonly string PATTERN = "\\b{0}.*\\b";
        internal static readonly string REPLACE_KEY = "{0}=";
        internal static readonly string SLASH = "/";
        internal static readonly string AMPERSAND = "&";

#if MASTER || SIT || DEBUG
        internal static readonly string EB_in_app_notification = "EB_in_app_notification";
        internal static readonly string EB_view_tips_reached = "EB_view_tips_reached";
        internal static readonly string EB_view_tips_reaching = "EB_view_tips_reaching";
        internal static readonly string EB_view_budget_reached = "EB_view_budget_reached";
        internal static readonly string EB_view_budget_reaching = "EB_view_budget_reaching";
        internal static readonly string EB_view_notification_duration_reaching = "EB_view_notification_duration_reaching";
        internal static readonly string EB_view_notification_duration_reached = "EB_view_notification_duration_reached";
        internal static readonly string EB_initiate_Later = "EB_initiate_Later";
        internal static readonly string EB_initiate_Start = "EB_initiate_Start";
        internal static readonly string EB_start = "EB_start";
        internal static readonly string EB_setup_success = "EB_setup_success";
        internal static readonly string EB_edit_budget = "EB_edit_budget";
        internal static readonly string EB_tooltip = "EB_tooltip";
        internal static readonly string EB_view_budget_duration = "EB_view_budget_duration";
        internal static readonly string EB_view_tips = "EB_view_tips";
        internal static readonly string EB_initiate_Duration = "EB_initiate_Duration";
        internal static readonly string SMR_icon_click = "SMR_ICON_CLICKED";
#else
        internal static readonly string EB_in_app_notification = "EB_in_app_notification";
        internal static readonly string EB_view_tips_reached = "EB_view_tips_reached";
        internal static readonly string EB_view_tips_reaching = "EB_view_tips_reaching";
        internal static readonly string EB_view_budget_reached = "EB_view_budget_reached";
        internal static readonly string EB_view_budget_reaching = "EB_view_budget_reaching";
        internal static readonly string EB_view_notification_duration_reaching = "EB_view_notification_duration_reaching";
        internal static readonly string EB_view_notification_duration_reached = "EB_view_notification_duration_reached";
        internal static readonly string EB_initiate_Later = "EB_initiate_Later";
        internal static readonly string EB_initiate_Start = "EB_initiate_Start";
        internal static readonly string EB_start = "EB_start";
        internal static readonly string EB_setup_success = "EB_setup_success";
        internal static readonly string EB_edit_budget = "EB_edit_budget";
        internal static readonly string EB_tooltip = "EB_tooltip";
        internal static readonly string EB_view_budget_duration = "EB_view_budget_duration";
        internal static readonly string EB_view_tips = "EB_view_tips";
        internal static readonly string EB_initiate_Duration = "EB_initiate_Duration";
        internal static readonly string SMR_icon_click = "SMR_ICON_CLICKED";
#endif

        //Dynatrce Test
        public static string TOUCH_ON_VIEW_SUBMITTED_ENQUIRY = "VIEW_SUBMITTED_ENQUIRY"; //Touch on View Submitted Enquiry
        public static string TOUCH_ON_SUBMIT_AND_TRACK_ENQUIRY = "SUBMIT_AND_TRACK_ENQUIRY";//Touch on Submit & Track Enquiry
        public static string TOUCH_ON_SUBMIT_NEW_ENQUIRY = "SUBMIT_NEW_ENQUIRY"; //Touch on Submit New Enquiry
        public static string TOUCH_ON_VIEW_OVERVOLTAGE_CLAIM_FROM_LIST = "VIEW_OVERVOLTAGE_CLAIM_FROM_LIST";
        public static string TOUCH_ON_SUBMIT_OVERVOLTAGE_CLAIM = "SUBMIT_OVERVOLTAGE_CLAIM";
    }
}
