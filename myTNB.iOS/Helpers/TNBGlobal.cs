using System.Collections.Generic;

namespace myTNB
{
    public static class TNBGlobal
    {
#if DEBUG || MASTER
        public static bool IsProduction = false;
        public static int PromoOverlayDisplayIntervalDays = 1;
#else
        public static bool IsProduction = true;
        public static int PromoOverlayDisplayIntervalDays = 15;
#endif
        public static bool IsChartEmissionEnabled = false;
        public static string OS = "ios";
        public static string APP_LANGUAGE = "EN";
        public static string DB_NAME = "myTNB.db";
        public static string APP_COUNTRY = "MY";

        public static string API_KEY_ID = GetAPIKeyID;
        public static string DEVICE_PLATFORM_IOS = "2";
        public static string SITECORE_URL
        {
            get
            {
#if DEBUG
                return "http://10.215.70.246/";//"http://tnbcsdevapp.tnb.my/";
#elif MASTER
                return "http://10.215.70.248/";//"http://tnbcsstgapp.tnb.my/";
#else
                return "https://sitecore.tnb.com.my/";
#endif
            }
        }
        public static string SITECORE_USERNAME = "api_user";
        public static string SITECORE_PASSWORD = "mytnbapiuser!3$@2";
        public static string MobileNoPrefix = "+60";
        public static int ToastZPosition = 1; //increment the value when showing on a component with higher z-position

        public static List<string> STORE_TYPE_LIST = new List<string>
        {
            "All"
            , "7-Eleven"
            , "Kedai Tenaga"
        };
        public static List<string> CONVINIENT_STORE_LIST = new List<string>{
            "7"
            , "seven"
            , "eleven"
            , "11"
            , "711"
            , "7-eleven"
            , "7-11"
        };

        /// <summary>
        /// The measurement units
        /// </summary>
        public static string UNIT_CURRENCY = "RM";

        public static string ACCOUNT_NAME_PATTERN = @"^.*$"; //@"^[A-Za-z0-9 ]*$";
        public static string AmountPattern = @"^[0-9.]*$";
        public static string CustomerNamePattern = @"^[A-Za-z0-9 ]*$"; //@"(?i)^[a-z0-9]+(?:[ ]?[a-z0-9]+)*$"; 
        public static string MobileNoPattern = @"^[0-9]*$";
        public static string NumbersOnlyPattern = @"^[0-9]*$";
        public static string ACCOUNT_NO_PATTERN = @"^[0-9]{12}$";
        public static string IC_NO_PATTERN = @"^[a-zA-Z0-9]+$";
        public static string PasswordPattern = @"^(?=.*[A-Za-z])(?=.*\d)[^.]{8,}$";
        public static string ROC_NO_PATTERN = @"^[a-zA-Z0-9]+$";


        /// <summary>
        /// Gets the payment URL.
        /// </summary>
        /// <returns>The payment URL.</returns>
        public static string GetPaymentURL
        {
            get
            {
#if DEBUG
                return "http://10.215.128.191:89/v5/PayRedirect.aspx";
#else
                return IsProduction ? "https://mytnbapp.tnb.com.my/v5/PayRedirect.aspx"
                    : "https://mobiletestingws.tnb.com.my/v5/PayRedirect.aspx";
#endif
            }
        }

        /// <summary>
        /// Gets the API Key identifier.
        /// </summary>
        /// <returns>The APIK ey identifier.</returns>
        static string GetAPIKeyID
        {
            get
            {
                return IsProduction ? "E6148656-205B-494C-BC95-CC241423E72F"
                    : "9515F2FA-C267-42C9-8087-FABA77CB84DF";
            }
        }

        #region Character Limits

        /// <summary>
        /// The input characters limits.
        /// </summary>
        public static int FeedbackMaxCharCount = 250;
        public static int FEEDBACK_FIELD_MAX_HEIGHT = 113;
        public static int ENQUIRY_FIELD_MAX_HEIGHT = 72;
        public static int AccountNumberLowCharLimit = 12;
        public static int AccountNumberHighCharLimit = 14;
        public static int PaymentMinAmnt = 1;
        public static int MobileNumberMaxCharCount = 13;
        public static int[] MobileNumberLimits = { 9, 10 };

        #endregion

        /// <summary>
        /// UI Tags.
        /// </summary>
        public static class Tags
        {
            public static int RangeLabel = 2;
            public static int DashboardToast = 100;
            public static int LoadingOverlay = 101;
            public static int ToastMessageLabel = 102;
        }

        /// <summary>
        /// UI Tags.
        /// </summary>
        public static class Errors
        {
            public static string FetchingSmartData = "204";
            public static string NoSmartData = "201";
        }

        /// <summary>
        /// UI Text strings.
        /// </summary>
        public static class Texts
        {
            public static string InfoLoading = "We’ll be there in a flash…";
        }

        /// <summary>
        /// System codes.
        /// </summary>
        public static class SystemCodes
        {
            public static string BCRM = "BCRM";
            public static string PaymentCC = "PG_CC";
            public static string PaymentFPX = "PG_FPX";
            public static string SmartMeter = "SmartMeter";
            public static string SSP = "SSP";
        }

        /// <summary>
        /// Preference keys.
        /// </summary>
        public static class PreferenceKeys
        {
            public static string PhoneVerification = "isPhoneVerified";
            public static string RememberEmail = "userEmail";
            public static string LoginState = "isLogin";
        }

        public static string DEFAULT_VALUE = "0.00";
        public static string ZERO = "0";
        public static string EMPTY_AMOUNT = "--";
        public static string EMPTY_DATE = "--";
        public static string EMPTY_ADDRESS = "- - -";
        public static string PERCENTAGE = "%";
    }
}