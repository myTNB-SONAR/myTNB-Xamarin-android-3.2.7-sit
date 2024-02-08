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

        internal const int APITimeOut = 60000;
        internal const int SitecoreTimeOut = 5000;
        internal const string SitecoreDomain = "sitecore";
        internal const string ApiUrlPath = "v7/mytnbws.asmx";
        internal const int MaxAccountList = 100000;

        internal const string SaltKey = "Salt-IT9LJQ3LJEAK5G2R35L5V6A5FUDO7A5B";
        internal const string PassPhrase = "PW-myTNB-PROD";
        internal const string SignKey = "134CED76E57C41C8B97E76D174B58B40";

        public const string SitecoreUsername = "api_user";
        public const string SitecorePassword = "mytnbapiuser!3$@2";
        public const string ApiDomain = "https://mytnbapp.tnb.com.my";
        public const string ApiKeyId = "E6148656-205B-494C-BC95-CC241423E72F";
        public const string SitecoreURL = "https://sitecore.tnb.com.my/";
        public const string AWSApiDomain = "https://api.mytnb.com.my/core/api";
        public const string OvisWebviewBaseUrl = "https://mytnbappwv.ovis.tnb.com.my";
        public const string SaltKeyPDF = "MYTNB@)@$PRD";

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

        //Unified API Key
        internal const string APIKey = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJDaGFubmVsIjoibXlUTkJfQVBJX01vYmlsZSIsIkNoYW5uZWxLZXkiOiIwRTBDRDFFOS04MDZDLTREMkEtQUM0NC1BQ0FGRjZCNDdCMDgiLCJuYmYiOjE2MTIzNTI0MjIsImV4cCI6MTYxMjM1NjAyMiwiaWF0IjoxNjEyMzUyNDIyLCJpc3MiOiJteVROQiBBUEkiLCJhdWQiOiJteVROQiBBUEkgQXVkaWVuY2UifQ.eWIvm3kznjBFt84Q79wlylYUTCnCt4L1sjTCI2QjbJMaS_EfSQ96F1ilbYamSmMLYdcNCFz2NCyfWLZJ4ThJyg";

        public struct SharePreferenceKey
        {
            public const string GetEligibilityTimeStamp = "GetEligibilityTimeStamp";
            public const string GetEligibilityData = "GetEligibilityData";
            public const string AccessToken = "AccessToken";
            public const string UserServiceAccessToken = "UserServiceAccessToken";
        }

        public struct PushNotificationTypes
        {
            public const string DBR_Owner = "DBROWNER";
            public const string DBR_NonOwner = "DBRNONOWNER";
            public const string APPLICATIONSTATUS = "APPLICATIONSTATUS";
            public const string APP_UPDATE = "APPUPDATE";
            public const string ACCOUNT_STATEMENT = "ACCOUNTSTATEMENT";
            public const string NEW_BILL_DESIGN = "NEWBILLDESIGN";
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