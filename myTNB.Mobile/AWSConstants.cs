﻿namespace myTNB.Mobile
{
    public static class AWSConstants
    {
        internal const int TimeOut = 10000;
        internal const int AccountStatementTimeOut = 5000;
        internal const string Channel = "myTNB_API_Mobile";
        internal const int RoleID = 16;

        //XAPI Keys
        private const string XAPIKey_DEV = "KqNPPaCgl913pSLSHBgVT8NjJvTfTdYH6W0R1w78";
        private const string XAPIKey_SIT = "8BVhUBJIJO9RAFwrxmx0o76n7tfRsPyd4DSChI5r";
        private const string XAPIKey_PROD = "gpUS5pe4aO2yMbId7bFa13dGfYYnBWbjn3vqn0d7";

        //Salt
        private const string SaltKey_DEV = "Salt-5123BEB842C046609AD5FB67A0A2D5D0";
        private const string SaltKey_SIT = "Salt-5123BEB842C046609AD5FB67A0A2D5D0";
        private const string SaltKey_PROD = "Salt-5123BEB842C046609AD5FB67A0A2D5D0";

        //Passphrase
        private const string Passphrase_DEV = "PW-myTNBDbrSso";
        private const string Passphrase_SIT = "PW-myTNBDbrSso";
        private const string Passphrase_PROD = "PW-myTNBDbrSso";

        //Environment
        private const string Environment_DEV = "Prod";
        private const string Environment_SIT = "Prod";
        private const string Environment_PROD = "Prod";

        //WEB Actions
        internal const string BackToApp = "mytnbapp://action=backToApp";
        internal const string BackToHome = "mytnbapp://action=backToHome";

        //Domains
        public struct Domains
        {
#if DEBUGx
            internal const string Domain = "https://devapi.mytnb.com.my";
            internal const string GenerateAccessToken = "https://devapi.mytnb.com.my/Identity/api/v1";
            internal const string GetEligibility = "https://devapi.mytnb.com.my/Eligibility/api/v1";
            internal const string GetBillRendering = "https://devapi.mytnb.com.my/BillRendering/api/v1";
            internal const string GetMultiBillRendering = "https://devapi.mytnb.com.my/BillRendering/api/v1";
            internal const string StartDigitalBill = "https://devdbr.mytnb.com.my/DigitalBill/Start";
            internal const string OptInToPaperBill = "https://devdbr.mytnb.com.my/PaperBill/OptIn";
            public const string DBRSSO = "https://devdbr.mytnb.com.my/Sso?s={0}";
#elif MASTER || SIT || DEBUG
            internal const string Domain = "https://stagingapi.mytnb.com.my";
            internal const string GenerateAccessToken = "https://stagingapi.mytnb.com.my/Identity/api/v1";
            internal const string GetEligibility = "https://stagingapi.mytnb.com.my/Eligibility/api/v1";
            internal const string GetBillRendering = "https://stagingapi.mytnb.com.my/BillRendering/api/v1";
            internal const string GetBillRenderingTenant = "https://stagingapi.mytnb.com.my/BillRendering/api/v1";
            internal const string GetAutoOptInCa = "https://stagingapi.mytnb.com.my/StagedData/api/v1/";
            internal const string UpdateAutoOptInCa = "https://stagingapi.mytnb.com.my/StagedData/api/v1/";
            internal const string GetMultiBillRendering = "https://stagingapi.mytnb.com.my/BillRendering/api/v1";
            internal const string StartDigitalBill = "https://stagingdbr.mytnb.com.my/DigitalBill/Start";
            internal const string OptInToPaperBill = "https://stagingdbr.mytnb.com.my/PaperBill/OptIn";
            internal const string DSRedirect = "https://stagingds.mytnb.com.my/EKYC/StartEKYC";
            public const string DBRSSO = "https://stagingdbr.mytnb.com.my/Sso?s={0}";
            public const string DSSSO = "https://stagingds.mytnb.com.my/Sso?s={0}";
#else
            internal const string Domain = "https://api.mytnb.com.my";
            internal const string GenerateAccessToken = "https://api.mytnb.com.my/Identity/api/v1";
            internal const string GetEligibility = "https://api.mytnb.com.my/Eligibility/api/v1";
            internal const string GetBillRendering = "https://api.mytnb.com.my/BillRendering/api/v1";
            internal const string GetBillRenderingTenant = "https://api.mytnb.com.my/BillRendering/api/v1";
            internal const string GetAutoOptInCa = "https://api.mytnb.com.my/StagedData/api/v1/";
            internal const string UpdateAutoOptInCa = "https://api.mytnb.com.my/StagedData/api/v1/";
            internal const string GetMultiBillRendering = "https://api.mytnb.com.my/BillRendering/api/v1";
            internal const string StartDigitalBill = "https://dbr.mytnb.com.my/DigitalBill/Start";
            internal const string OptInToPaperBill = "https://dbr.mytnb.com.my/PaperBill/OptIn";
            internal const string DSRedirect = "https://stagingds.mytnb.com.my/EKYC/StartEKYC";//Staging
            public const string DBRSSO = "https://dbr.mytnb.com.my/Sso?s={0}";
            public const string DSSSO = "https://stagingds.mytnb.com.my/Sso?s={0}";//Staging
#endif
        }

#if DEBUGx
        internal const string Environment = Environment_DEV;
        internal const string XAPIKey = XAPIKey_DEV;
        public const string SaltKey = SaltKey_DEV;
        public const string PassPhrase = Passphrase_DEV;
#elif MASTER || SIT || DEBUG
        internal const string Environment = Environment_SIT;
        internal const string XAPIKey = XAPIKey_SIT;
        public const string SaltKey = SaltKey_SIT;
        public const string PassPhrase = Passphrase_SIT;
#else
        internal const string Environment = Environment_PROD;
        internal const string XAPIKey = XAPIKey_PROD;
        public const string SaltKey = SaltKey_PROD;
        public const string PassPhrase = Passphrase_PROD;
#endif

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
            //Identity
            internal const string GenerateAccessToken = "GenerateAccessToken";
            //DBR
            internal const string GetBillRendering = "BillRendering";
            internal const string GetBillRenderingTenant = "BillRenderingTenant";
            internal const string GetAutoOptInCaDBR = "GetAutoOptIn";
            internal const string UpdateAutoOptInCaDBR = "UpdateAutoOptIn";
            internal const string PostMultiBillRendering = "MultiBillRendering";
            internal const string PostInstallationDetails = "InstallationDetails";
            internal const string PostMultiInstallationDetails = "MultiInstallationDetails";
            internal const string PostAccountStatement = "AccountStatement";
            internal const string PostAccountStatementNotification = "AccountStatementNotification";
            public const string GetEligibility = "Eligibility";
            //DS
            internal const string GetEKYCStatus = "EKYCStatus";
            internal const string GetEKYCIdentification = "EKYCIdentification";
            public const string EligibilityByCriteria = "EligibilityByCriteria";
        }
    }
}