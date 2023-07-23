using System.Collections.Generic;

namespace myTNB.Mobile
{
    public static class MobileConstants
    {
        //Headers
        internal const string Header_RoleID = "RoleId";
        internal const string Header_UserID = "UserId";
        internal const string Header_UserName = "UserName";
        internal const string Header_SecureKey = "SecureKey";
        internal const string Header_UserInfo = "UserInfo";
        internal const string Header_Lang = "Lang";

        //String Constants
        internal const string EMPTY = "empty";
        internal const string DEFAULT = "default";
        internal const string TIMEOUT = "timeout";
        internal const string EMPTY_FILTER = "emptyFilter";
        internal const string SUCCESS_CODE = "7200";

        //Todo: Set to 60000
        internal const int APITimeOut = 60000;
        internal const int SitecoreTimeOut = 5000;
        internal const string SitecoreDomain = "sitecore";
        public const string SitecoreUsername = "api_user";
        public const string SitecorePassword = "mytnbapiuser!3$@2";
        internal const string ApiUrlPath = "v7/mytnbws.asmx";
        internal const int MaxAccountList = 100000;

        //AWS ASMX Copy
        private const string AWS_DEV1 = "https://stagingapi.mytnb.com.my/asmx-97";
        private const string AWS_DEV2 = "https://stagingapi.mytnb.com.my/asmx-98";
        private const string AWS_SIT = "https://stagingapi.mytnb.com.my/asmx";

        private const string DEV1 = "http://10.215.128.191:88";
        private const string DEV2 = "http://10.215.128.191:89";
        private const string DEV3 = "http://10.215.128.191:99";
        private const string DEV4 = "http://10.215.128.191:97";
        private const string DEV5 = "http://10.215.128.162:99";

        private const string SIT_AWS = "https://stagingapi.mytnb.com.my/asmx";
        //ASMX
        private const string SIT = "https://mobiletestingws.tnb.com.my";
        //AWS ASMX Copy
        //private const string SIT = "https://stagingapi.mytnb.com.my/asmx-97";
        //AWS OVIS
        //private const string SIT = "http://mytnbwsovisstg.ap.ngrok.io";
        private const string PROD = "https://mytnbapp.tnb.com.my";
        private const string DEVUNIFIED = "http://dev.mytnb.com.my:8322";
        //Mark: http://tnbcsdevapp.tnb.my/
        private const string SitecoreDEV = "http://10.215.70.246/";
        //Mark: http://tnbcsstgapp.tnb.my/
        private const string SitecoreSIT = "http://10.215.70.248/";
        private const string SitecorePROD = "https://sitecore.tnb.com.my/";

        private const string ApiKeyIdDEV = "9515F2FA-C267-42C9-8087-FABA77CB84DF";
        private const string ApiKeyIdPROD = "E6148656-205B-494C-BC95-CC241423E72F";

        //private const string ApiKeyIdDEV = "c4bd133f-91e3-4a6e-823d-b986bf67648b";
        //private const string ApiKeyIdSIT = "01b7e51b-f777-4653-8658-6662585e4dca";
        //private const string ApiKeyIdPROD = "b24450d9-7045-473f-bec1-4549fe8c9fc6";

        //Access Token
        private const string SignKey_Staging = "2276B01152D94DF8A65944D3EFD70378";
        private const string SignKey_Prod = "134CED76E57C41C8B97E76D174B58B40";

        //Encrypt / Decrypt
        private const string SaltKey_DEV = "Salt-4NHF1XP910G8NN6GRH23PD12N9X6T5DW";
        private const string SaltKey_PROD = "Salt-IT9LJQ3LJEAK5G2R35L5V6A5FUDO7A5B";
        private const string Passphrase_DEV = "PW-myTNB-DEV";
        private const string Passphrase_PROD = "PW-myTNB-PROD";

        //AWS GetAccount
        //public static string AWSApiDomainSIT = "https://stagingapi.mytnb.com.my"; //"https://core.stg-mytnb.com/api"; 
        public static string AWSApiDomainSIT = "https://stagingapi.mytnb.com.my/core/api";
        public static string AWSAPIOnpremSIT = "http://mobiletestingws.tnb.com.my/api";
        public static string AWSApiDomainDEV = "https://mytnb-core-staging-362772745.ap-southeast-1.elb.amazonaws.com/api";
        public static string AWSApiDomainPROD = "https://api.mytnb.com.my/core/api";

        //OVIS Webview
        //public static string OvisWebviewBaseUrlDEV = "http://192.168.1.157:3000";
        //public static string OvisWebviewBaseUrlDEV = "http://mytnbwvovis.ap.ngrok.io";
        public static string OvisWebviewBaseUrlDEV = "http://dev-mytnbappwv.tnbovis.com";
        //public static string OvisWebviewBaseUrlSTG = "http://mytnbwvovisstg.ap.ngrok.io";
        public static string OvisWebviewBaseUrlSTG = "http://qa-mytnbappwv.tnbovis.com";
        //public static string OvisWebviewBaseUrlSTG = "https://stage-mytnbappwv.ovis.tnb.com.my";
        public static string OvisWebviewBaseUrlPROD = "https://mytnbappwv.ovis.tnb.com.my";

#if DEBUG
        public static string ApiDomain = DEV5;
        public const string ApiKeyId = ApiKeyIdDEV;
        public const string SitecoreURL = SitecorePROD;
        internal const string SaltKey = SaltKey_DEV;
        internal const string PassPhrase = Passphrase_DEV;
        public static string AWSApiDomain = AWSApiDomainSIT;
        public static string OvisWebviewBaseUrl = OvisWebviewBaseUrlSTG;
        internal const string SignKey = SignKey_Staging;
#elif MASTER || SIT || DEBUG
        public static string ApiDomain = SIT;
        public const string ApiKeyId = ApiKeyIdDEV;
        public const string SitecoreURL = SitecorePROD;
        internal const string SaltKey = SaltKey_DEV;
        internal const string PassPhrase = Passphrase_DEV;
        public static string AWSApiDomain = AWSApiDomainSIT;
        public static string OvisWebviewBaseUrl = OvisWebviewBaseUrlSTG;
        internal const string SignKey = SignKey_Staging;
#else
        public static string ApiDomain = PROD;
        public const string ApiKeyId = ApiKeyIdPROD;
        public const string SitecoreURL = SitecorePROD;
        internal const string SaltKey = SaltKey_PROD;
        internal const string PassPhrase = Passphrase_PROD;
        public static string AWSApiDomain = AWSApiDomainPROD;
        public static string OvisWebviewBaseUrl = OvisWebviewBaseUrlPROD;
        internal const string SignKey = SignKey_Prod;
#endif

        //Service Name
        internal const string Service_SearchApplicationType = "SearchApplicationType";
        internal const string Service_GetApplicationStatus = "ApplicationStatus";
        internal const string Service_SaveApplication = "SaveApplication";
        internal const string Service_GetAllApplications = "AllApplications";
        internal const string Service_GetApplicationDetail = "ApplicationDetail";
        internal const string Service_RemoveApplication = "RemoveApplication";
        internal const string Service_SearchApplicationByCA = "SearchApplicationByCA";
        internal const string Service_GetTaxInvoice = "GetTaxInvoiceForApplicationPayment";
        internal const string Service_TaxInvoice = "GetTaxInvoiceApplicationPayment";
        internal const string Service_GetCustomerRatingMaster = "CustomerRatingMaster";
        internal const string Service_PostSubmitRating = "SubmitRating";
        internal const string Service_GetAvailableAppointment = "AvailableAppointment";
        internal const string Service_PostSetAppointment = "SetAppointment";
        internal const string Service_SyncSRApplication = "SyncSRApplication";
        internal const string Service_SaveFeatureInfo = "SaveFeatureInfo";

        //Language File Constants
        internal const string LanguageFile_ServiceDetails = "ServiceDetails";
        internal const string LanguageFile_Services = "Services";

        //Mapping
        internal const string LanguageFile_Mapping = "Mapping";
        internal const string LanguageFile_ExcludedApplicationTypes = "ExcludedApplicationTypes";

        //HardCoded Values
        internal const string Constants_Currency = "RM";

        //API Key

#if MASTER || SIT || DEBUG
        internal const string APIKey = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJDaGFubmVsIjoibXlUTkJfQVBJX01vYmlsZSIsIkNoYW5uZWxLZXkiOiJGNUFEQjU0QzM1MkM0NzYwQjUzMkNEOUU1ODdBRTRGNiIsIm5iZiI6MTU5OTE5OTc0OSwiZXhwIjoxNTk5MjAzMzQ5LCJpYXQiOjE1OTkxOTk3NDksImlzcyI6Im15VE5CIEFQSSIsImF1ZCI6Im15VE5CIEFQSSBBdWRpZW5jZSJ9.Sy_xahwMgt2izUgztYq_BQeGECGsahP9oSNHeB1kwB0Ij8Grpg3kQZPCa_b_bbiyngzpjKy38_DFU12wToQAiA";
#else
        internal const string APIKey = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJDaGFubmVsIjoibXlUTkJfQVBJX01vYmlsZSIsIkNoYW5uZWxLZXkiOiIwRTBDRDFFOS04MDZDLTREMkEtQUM0NC1BQ0FGRjZCNDdCMDgiLCJuYmYiOjE2MTIzNTI0MjIsImV4cCI6MTYxMjM1NjAyMiwiaWF0IjoxNjEyMzUyNDIyLCJpc3MiOiJteVROQiBBUEkiLCJhdWQiOiJteVROQiBBUEkgQXVkaWVuY2UifQ.eWIvm3kznjBFt84Q79wlylYUTCnCt4L1sjTCI2QjbJMaS_EfSQ96F1ilbYamSmMLYdcNCFz2NCyfWLZJ4ThJyg";
#endif

        public struct SharePreferenceKey
        {
            public const string GetEligibilityTimeStamp = "GetEligibilityTimeStamp";
            public const string GetEligibilityData = "GetEligibilityData";
            public const string AccessToken = "AccessToken";
            public const string UserServiceAccessToken = "UserServiceAccessToken";
        }

        public struct OSType
        {
            public const string Android = "1";
            public const string iOS = "2";
            public const string Huawei = "3";

            public const int int_Android = 1;
            public const int int_iOS = 2;
            public const int int_Huawei = 3;
        }

        internal struct BillRenderingCodes
        {
            internal const string Owner_EBill = "ZV04";
            internal const string Owner_EMail = "ZV03";
            internal const string Owner_Paper = "ZV02";

            internal const string BC_EBill = "ZNTF";
            internal const string BC_EMail = "ZEML";
            internal const string BC_Paper = "ZINV";
        }

        internal static List<string> ResidentialTariffTypeList = new List<string>
        {
            "A_LV"
            , "A_LV_D"
            , "A_LV_D_I"
            , "A_LV_I"
        };
    }
}