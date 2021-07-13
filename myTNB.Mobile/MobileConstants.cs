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
        internal const string SitecoreUsername = "api_user";
        internal const string SitecorePassword = "mytnbapiuser!3$@2";
        internal const string ApiUrlPath = "v6/mytnbappws.asmx";

        private const string DEV1 = "http://10.215.128.191:88";
        private const string DEV2 = "http://10.215.128.191:89";
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
        //AWS DEV
        private const string SaltKeyDEV = "Salt-5123BEB842C046609AD5FB67A0A2D5D0";
        private const string PassphraseDEV = "PW-myTNBDbrSso";
        private const string DBROriginURLDEV = "https://test.mytnb.com.my";
        private const string DBRRedirectURLDEV = "http://EC2Co-EcsEl-11MUE9B1S2T04-1563452123.ap-southeast-1.elb.amazonaws.com/DigitalBill/Start";
        //AWS PROD
        private const string SaltKeyPROD = "Salt-5123BEB842C046609AD5FB67A0A2D5D0";
        private const string PassphrasePROD = "PW-myTNBDbrSso";
        private const string DBROriginURLPROD = "https://test.mytnb.com.my";
        private const string DBRRedirectURLPROD = "http://EC2Co-EcsEl-11MUE9B1S2T04-1563452123.ap-southeast-1.elb.amazonaws.com/DigitalBill/Start";

#if DEBUG
        internal static string ApiDomain = SIT;
        internal const string ApiKeyId = ApiKeyIdDEV;
        internal const string SitecoreURL = SitecorePROD;
        internal const string SaltKey = SaltKeyDEV;
        internal const string PassPhrase = PassphraseDEV;
        internal const string DBROriginURL = DBROriginURLDEV;
        internal const string DBRRedirectURL = DBRRedirectURLDEV;
#elif MASTER || SIT
        internal static string ApiDomain = SIT;
        internal const string ApiKeyId = ApiKeyIdDEV;
        internal const string SitecoreURL = SitecorePROD;
        internal const string SaltKey = SaltKeyDEV;
        internal const string PassPhrase = PassphraseDEV;
        internal const string DBROriginURL = DBROriginURLDEV;
        internal const string DBRRedirectURL = DBRRedirectURLDEV;
#else
        internal static string ApiDomain = PROD;
        internal const string ApiKeyId = ApiKeyIdPROD;
        internal const string SitecoreURL = SitecorePROD;
        internal const string SaltKey = SaltKeyPROD;
        internal const string PassPhrase = PassphrasePROD;
        internal const string DBROriginURL = DBROriginURLPROD;
        internal const string DBRRedirectURL = DBRRedirectURLPROD;
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
    }
}