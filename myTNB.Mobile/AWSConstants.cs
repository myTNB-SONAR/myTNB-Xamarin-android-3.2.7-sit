namespace myTNB.Mobile
{
    public static class AWSConstants
    {
        internal const int TimeOut = 10000;
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

        //Domains
        public struct Domains
        {
#if DEBUG
            internal const string GenerateAccessToken = "https://5y8p2rm83k.execute-api.ap-southeast-1.amazonaws.com";
            internal const string GetDBREligibility = "https://j8kgh1w7y3.execute-api.ap-southeast-1.amazonaws.com";
            internal const string GetBillRendering = "https://udv358acc6.execute-api.ap-southeast-1.amazonaws.com";
            internal const string GetMultiBillRendering = "https://udv358acc6.execute-api.ap-southeast-1.amazonaws.com";
            internal const string GetInstallationDetails = "https://rq9iqdunf6.execute-api.ap-southeast-1.amazonaws.com";// Not yet segregated
            internal const string GetMultiInstallationDetails = "https://jqwg9vxr6d.execute-api.ap-southeast-1.amazonaws.com";// Not yet segregated
            internal const string StartDigitalBill = "http://ecsdbr-1386369932.ap-southeast-1.elb.amazonaws.com:8011/DigitalBill/Start";
            internal const string OptInToPaperBill = "http://ecsdbr-1386369932.ap-southeast-1.elb.amazonaws.com:8011/PaperBill/OptIn";
            public const string SSO = "http://ecsdbr-1386369932.ap-southeast-1.elb.amazonaws.com:8011/Sso?s={0}";
#elif MASTER || SIT
            internal const string GenerateAccessToken = "https://5c6jgu44tf.execute-api.ap-southeast-1.amazonaws.com";
            internal const string GetDBREligibility = "https://12pq772dmj.execute-api.ap-southeast-1.amazonaws.com";
            internal const string GetBillRendering = "https://k6mjxscgb1.execute-api.ap-southeast-1.amazonaws.com";
            internal const string GetMultiBillRendering = "https://k6mjxscgb1.execute-api.ap-southeast-1.amazonaws.com";
            internal const string GetInstallationDetails = "https://rq9iqdunf6.execute-api.ap-southeast-1.amazonaws.com";// Not yet segregated
            internal const string GetMultiInstallationDetails = "https://jqwg9vxr6d.execute-api.ap-southeast-1.amazonaws.com";// Not yet segregated
            internal const string StartDigitalBill = "http://ecsdbr-1386369932.ap-southeast-1.elb.amazonaws.com:8021/DigitalBill/Start";
            internal const string OptInToPaperBill = "http://ecsdbr-1386369932.ap-southeast-1.elb.amazonaws.com:8021/PaperBill/OptIn";
            public const string SSO = "http://ecsdbr-1386369932.ap-southeast-1.elb.amazonaws.com:8021/Sso?s={0}";
#else
            internal const string GenerateAccessToken = "https://u87s7tl12m.execute-api.ap-southeast-1.amazonaws.com";
            internal const string GetDBREligibility = "https://ej1yrq9zc6.execute-api.ap-southeast-1.amazonaws.com";
            internal const string GetBillRendering = "https://yzixerxax2.execute-api.ap-southeast-1.amazonaws.com";
            internal const string GetMultiBillRendering = "https://yzixerxax2.execute-api.ap-southeast-1.amazonaws.com";
            internal const string GetInstallationDetails = "https://rq9iqdunf6.execute-api.ap-southeast-1.amazonaws.com";// Not yet segregated
            internal const string GetMultiInstallationDetails = "https://jqwg9vxr6d.execute-api.ap-southeast-1.amazonaws.com";// Not yet segregated
            internal const string StartDigitalBill = "http://ecsdbr-1386369932.ap-southeast-1.elb.amazonaws.com/DigitalBill/Start";
            internal const string OptInToPaperBill = "http://ecsdbr-1386369932.ap-southeast-1.elb.amazonaws.com/PaperBill/OptIn";
            public const string SSO = "http://ecsdbr-1386369932.ap-southeast-1.elb.amazonaws.com/Sso?s={0}";
#endif
        }

#if DEBUGs
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
        public const string PassPhrase = PassphrasePROD;
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
            internal const string GenerateAccessToken = "GenerateAccessToken";
            internal const string GetBillRendering = "BillRendering";
            internal const string PostMultiBillRendering = "MultiBillRendering";
            internal const string PostInstallationDetails = "InstallationDetails";
            internal const string PostMultiInstallationDetails = "MultiInstallationDetails";
            public const string GetEligibility = "Eligibility";
        }
    }
}