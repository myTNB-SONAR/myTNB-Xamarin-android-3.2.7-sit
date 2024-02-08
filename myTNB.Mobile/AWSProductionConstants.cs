namespace myTNB.Mobile
{
    public static class AWSConstants
    {
        internal const int TimeOut = 60000;
        internal const int DebugTimeOut = 20000;
        internal const int AccountStatementTimeOut = 5000;
        internal const string Channel = "myTNB_API_Mobile";
        internal const int RoleID = 16;

        //Salt
        public const string MyHome_SaltKey = "Salt-9F586DF42C58-4753-8FCE7113EBFAACCF";

        //Passphrase
        public const string MyHome_Passphrase = "PW-myTNBMyHomeSso";

        //WEB Actions
        internal const string BackToApp = "mytnbapp://action=backToApp";
        internal const string BackToHome = "mytnbapp://action=backToHome";
        public const string BackToHomeCancelURL = "mytnbapp://action=backToHomeWithCancelToast";
        public const string ApplicationStatusLandingCancelURL = "mytnbapp://action=applicationStatusLandingWithCancelToast";
        public const string BackToHomeCancelCOTURL = "mytnbapp://action=backToHomeWithCancelCOTToast";
        public const string ApplicationStatusLandingCancelCOTURL = "mytnbapp://action=applicationStatusLandingWithCancelCOTToast";

        //Domains
        public struct Domains
        {
            internal const string Domain = "https://api.mytnb.com.my";
            internal const string GenerateAccessToken = "https://api.mytnb.com.my/Identity/api/v1";
            internal const string GetEligibility = "https://api.mytnb.com.my/Eligibility/api/v1";
            internal const string GetBillRendering = "https://api.mytnb.com.my/BillRendering/api/v1";
            internal const string GetMultiBillRendering = "https://api.mytnb.com.my/BillRendering/api/v1";
            internal const string StartDigitalBill = "https://dbr.mytnb.com.my/DigitalBill/Start";
            internal const string OptInToPaperBill = "https://dbr.mytnb.com.my/PaperBill/OptIn";
            internal const string DSRedirect = "https://ds.mytnb.com.my/EKYC/StartEKYC";
            internal const string PostBREligibilityIndicators = "https://api.mytnb.com.my/BillRendering/api/v1";
            internal const string GetAutoOptInCa = "https://api.mytnb.com.my/StagedData/api/v1";
            internal const string PatchUpdateAutoOptInCa = "https://api.mytnb.com.my/StagedData/api/v1";
            public struct SSO
            {
                public const string DBR = "https://dbr.mytnb.com.my/Sso?s={0}";
                public const string MyHome = "https://myhome.mytnb.com.my/Sso?s={0}";
                public const string DS = "https://ds.mytnb.com.my/Sso?s={0}";
            }
        }

        internal const string Environment = "Prod";
        internal const string XAPIKey = "gpUS5pe4aO2yMbId7bFa13dGfYYnBWbjn3vqn0d7";
        public const string SaltKey = "Salt-5123BEB842C046609AD5FB67A0A2D5D0";
        public const string PassPhrase = "PW-myTNBDbrSso";

        //Headers
        internal struct Headers
        {
            internal const string XAPIKey = "x-api-key";
            internal const string Authorization = "Authorization";
            internal const string ViewInfo = "ViewInfo";
        }

        //Services
        public struct Services
        {
            public const string GetEligibility = "Eligibility";
            public const string EligibilityByCriteria = "EligibilityByCriteria";

            //Identity
            internal const string GenerateAccessToken = "GenerateAccessToken";
            internal const string GetUserServiceAccessToken = "GetUserServiceAccessToken";
            //DBR
            internal const string GetBillRendering = "BillRendering";
            internal const string PostMultiBillRendering = "MultiBillRendering";
            internal const string PostInstallationDetails = "InstallationDetails";
            internal const string PostMultiInstallationDetails = "MultiInstallationDetails";
            internal const string PostAccountStatement = "AccountStatement";
            internal const string PostAccountStatementNotification = "AccountStatementNotification";
            internal const string PostServices = "GetServices";

            internal const string PostBREligibilityIndicators = "BREligibilityIndicators";
            internal const string GetAutoOptInCa = "GetAutoOptInCa";
            internal const string PatchUpdateAutoOptInCa = "PatchUpdateAutoOptInCa";
            internal const string PostDeleteNCDraft = "DeleteNCDraft";
            internal const string PostGetDraftApplications = "GetDraftApplications";
            internal const string PostDeleteCOTDraft = "DeleteCOTDraft";
            internal const string PostDeleteCOADraft = "DeleteCOADraft";
            internal const string PostGetNCDraftApplications = "GetNCDraftApplications";
            //DS
            internal const string GetEKYCStatus = "EKYCStatus";
            internal const string GetEKYCIdentification = "EKYCIdentification";
        }
    }
}