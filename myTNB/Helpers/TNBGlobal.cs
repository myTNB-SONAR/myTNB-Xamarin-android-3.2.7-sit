using System.Collections.Generic;

namespace myTNB
{
    public static class TNBGlobal
    {
#if DEBUG || MASTER
        public static bool IsProduction = false;
#else
        public static bool IsProduction = true;
#endif
        public static bool IsChartEmissionEnabled = false;
        public static string OS = "ios";
        public static string DEFAULT_LANGUAGE = "en";
        public static string DB_NAME = "myTNB.db";

        public static string API_KEY_ID = GetAPIKeyID();
        public static string SITECORE_URL = "https://sitecore.tnb.com.my/";
        public static string SITECORE_USERNAME = "api_user";
        public static string SITECORE_PASSWORD = "mytnbapiuser!3$@2";

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
        public static string UNIT_ENERGY = "kWh";
        public static string UNIT_EMISSION = "kg";

        public static string ACCOUNT_NAME_PATTERN = @"^[A-Za-z0-9 ]*$";

        /// <summary>
        /// Gets the payment URL.
        /// </summary>
        /// <returns>The payment URL.</returns>
        public static string GetPaymentURL()
        {
            return IsProduction ? "https://mytnbapp.tnb.com.my/v5/PayRedirect.aspx"
                    : "https://mobiletestingws.tnb.com.my/v5/PayRedirect.aspx";
        }

        /// <summary>
        /// Gets the API Key identifier.
        /// </summary>
        /// <returns>The APIK ey identifier.</returns>
        static string GetAPIKeyID()
        {
            return IsProduction ? "E6148656-205B-494C-BC95-CC241423E72F"
                    : "9515F2FA-C267-42C9-8087-FABA77CB84DF";
        }

        /// <summary>
        /// The feedback max char count.
        /// </summary>
        public static int FeedbackMaxCharCount = 250;

        /// <summary>
        /// UI Tags.
        /// </summary>
        public static class Tags
        {
            public static int RangeLabel = 2;
            public static int DashboardToast = 100;
        }

        /// <summary>
        /// UI Tags.
        /// </summary>
        public static class Errors
        {
            public static string FetchingSmartData = "204";
            public static string NoSmartData = "201";
        }


    }
}