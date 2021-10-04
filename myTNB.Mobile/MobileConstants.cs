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
        internal const string EMPTY_FILTER = "emptyFilter";
        internal const string SUCCESS_CODE = "7200";

        //Todo: Set to 60000
        internal const int APITimeOut = 60000;
        internal const int SitecoreTimeOut = 5000;
        internal const string SitecoreDomain = "sitecore";
        public const string SitecoreUsername = "api_user";
        public const string SitecorePassword = "mytnbapiuser!3$@2";
        internal const string ApiUrlPath = "v6/mytnbappws.asmx";

        private const string DEV1 = "http://10.215.128.191:88";
        private const string DEV2 = "http://10.215.128.191:89";
        private const string DEV3 = "http://10.215.128.191:97";
        private const string SIT = "https://mobiletestingws.tnb.com.my";
        private const string PROD = "https://mytnbapp.tnb.com.my";
        private const string DEVUNIFIED = "http://dev.mytnb.com.my:8322";
        //Mark: http://tnbcsdevapp.tnb.my/
        private const string SitecoreDEV = "http://10.215.70.246/";
        //Mark: http://tnbcsstgapp.tnb.my/
        private const string SitecoreSIT = "http://10.215.70.248/";
        private const string SitecorePROD = "https://sitecore.tnb.com.my/";
        private const string ApiKeyIdDEV = "9515F2FA-C267-42C9-8087-FABA77CB84DF";
        private const string ApiKeyIdPROD = "E6148656-205B-494C-BC95-CC241423E72F";

        //Encrypt / Decrypt
        private const string SaltKey_DEV = "Salt-4NHF1XP910G8NN6GRH23PD12N9X6T5DW";
        private const string SaltKey_PROD = "Salt-IT9LJQ3LJEAK5G2R35L5V6A5FUDO7A5B";
        private const string Passphrase_DEV = "PW-myTNB-DEV";
        private const string Passphrase_PROD = "PW-myTNB-PROD";

        //AWS GetAccount
        public static string AWSApiDomainSIT = "https://core.stg-mytnb.com/api"; 
        public static string AWSApiDomainDEV = "https://mytnb-core-staging-362772745.ap-southeast-1.elb.amazonaws.com/api";
        public static string AWSApiDomainPROD = "https://core.prod-mytnb.com/api";

#if DEBUG
        public static string ApiDomain = SIT;
        public const string ApiKeyId = ApiKeyIdDEV;
        public const string SitecoreURL = SitecorePROD;
        internal const string SaltKey = SaltKey_DEV;
        internal const string PassPhrase = Passphrase_DEV;
        public static string AWSApiDomain = AWSApiDomainSIT;

#elif MASTER || SIT
        public static string ApiDomain = SIT;
        public const string ApiKeyId = ApiKeyIdDEV;
        public const string SitecoreURL = SitecorePROD;
        internal const string SaltKey = SaltKey_DEV;
        internal const string PassPhrase = Passphrase_DEV;
        public static string AWSApiDomain = AWSApiDomainSIT;
#else
        public static string ApiDomain = PROD;
        public const string ApiKeyId = ApiKeyIdPROD;
        public const string SitecoreURL = SitecorePROD;
        internal const string SaltKey = SaltKey_PROD;
        internal const string PassPhrase = Passphrase_PROD;
        public static string AWSApiDomain = AWSApiDomainPROD;
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

        //Language File Constants
        internal const string LanguageFile_ServiceDetails = "ServiceDetails";
        internal const string LanguageFile_Services = "Services";

        //Mapping
        internal const string LanguageFile_Mapping = "Mapping";
        internal const string LanguageFile_ExcludedApplicationTypes = "ExcludedApplicationTypes";

        //HardCoded Values
        internal const string Constants_Currency = "RM";

        //API Key
#if RELEASE 
        internal const string APIKey = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJDaGFubmVsIjoibXlUTkJfQVBJX01vYmlsZSIsIkNoYW5uZWxLZXkiOiIwRTBDRDFFOS04MDZDLTREMkEtQUM0NC1BQ0FGRjZCNDdCMDgiLCJuYmYiOjE2MTIzNTI0MjIsImV4cCI6MTYxMjM1NjAyMiwiaWF0IjoxNjEyMzUyNDIyLCJpc3MiOiJteVROQiBBUEkiLCJhdWQiOiJteVROQiBBUEkgQXVkaWVuY2UifQ.eWIvm3kznjBFt84Q79wlylYUTCnCt4L1sjTCI2QjbJMaS_EfSQ96F1ilbYamSmMLYdcNCFz2NCyfWLZJ4ThJyg";
#else
        internal const string APIKey = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJDaGFubmVsIjoibXlUTkJfQVBJX01vYmlsZSIsIkNoYW5uZWxLZXkiOiJGNUFEQjU0QzM1MkM0NzYwQjUzMkNEOUU1ODdBRTRGNiIsIm5iZiI6MTU5OTE5OTc0OSwiZXhwIjoxNTk5MjAzMzQ5LCJpYXQiOjE1OTkxOTk3NDksImlzcyI6Im15VE5CIEFQSSIsImF1ZCI6Im15VE5CIEFQSSBBdWRpZW5jZSJ9.Sy_xahwMgt2izUgztYq_BQeGECGsahP9oSNHeB1kwB0Ij8Grpg3kQZPCa_b_bbiyngzpjKy38_DFU12wToQAiA";
#endif

        public struct SharePreferenceKey
        {
            public const string GetEligibilityTimeStamp = "GetEligibilityTimeStamp";
            public const string GetEligibilityData = "GetEligibilityData";
            public const string AccessToken = "AccessToken";
        }

        public struct PushNotificationTypes
        {
            public const string DBR_Owner = "DBROWNER";
            public const string APPLICATIONSTATUS = "APPLICATIONSTATUS";
        }

        public struct OSType
        {
            public const string Android = "1";
            public const string iOS = "2";
            public const string Huawei = "3";
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