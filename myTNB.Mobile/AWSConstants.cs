namespace myTNB.Mobile
{
    public static class AWSConstants
    {
        //AWS DEV
        private const string SaltKey_DEV = "Salt-5123BEB842C046609AD5FB67A0A2D5D0";
        private const string Passphrase_DEV = "PW-myTNBDbrSso";
        private const string DBROriginURL_DEV = "mytnbapp://action=backToApp";
        private const string StartDBRRedirectURL_DEV = "http://ecsdbr-1386369932.ap-southeast-1.elb.amazonaws.com/DigitalBill/Start";
        private const string PaperBillOptInRedirectURL_DEV = "http://ecsdbr-1386369932.ap-southeast-1.elb.amazonaws.com/PaperBill/OptIn";
        private const string DBRSSOURL_DEV = "http://ecsdbr-1386369932.ap-southeast-1.elb.amazonaws.com/Sso?s={0}";
        //AWS PROD
        private const string SaltKey_PROD = "Salt-5123BEB842C046609AD5FB67A0A2D5D0";
        private const string Passphrase_PROD = "PW-myTNBDbrSso";
        private const string DBROriginURL_PROD = "mytnbapp://action=backToApp";
        private const string StartDBRRedirectURL_PROD = "http://ecsdbr-1386369932.ap-southeast-1.elb.amazonaws.com/DigitalBill/Start";
        private const string PaperBillOptInRedirectURL_PROD = "http://ecsdbr-1386369932.ap-southeast-1.elb.amazonaws.com/PaperBill/OptIn";
        private const string DBRSSOURL_PROD = "http://ecsdbr-1386369932.ap-southeast-1.elb.amazonaws.com/Sso?s={0}";

        internal const int TimeOut = 10000;

        internal const string Channel = "myTNB_API_Mobile";
        internal static int RoleID = 16;

        private const string SIT = "SIT";
        private const string PROD = "Prod";
        private const string XAPIKey_SIT = "8BVhUBJIJO9RAFwrxmx0o76n7tfRsPyd4DSChI5r";
        private const string XAPIKey_PROD = "8BVhUBJIJO9RAFwrxmx0o76n7tfRsPyd4DSChI5r";

        //Domains
        internal struct Domains
        {
            internal const string GenerateAccessToken = "https://ru5ofma1zd.execute-api.ap-southeast-1.amazonaws.com";
            internal const string GetBillRendering = "https://q7zjjtvkmf.execute-api.ap-southeast-1.amazonaws.com";
            internal const string GetDBREligibility = "https://pjpd3gxnd8.execute-api.ap-southeast-1.amazonaws.com";
        }

        public struct URLs
        {
#if DEBUG
            public const string DBROriginURL = DBROriginURL_DEV;
            public const string StartDBRRedirectURL = StartDBRRedirectURL_DEV;
            public const string PaperBillOptInDBRRedirectURL = PaperBillOptInRedirectURL_DEV;
            public const string DBRSSOURL = DBRSSOURL_DEV;
#elif MASTER || SIT
        
            public const string DBROriginURL = DBROriginURL_DEV;
            public const string StartDBRRedirectURL = StartDBRRedirectURL_DEV;
            public const string PaperBillOptInDBRRedirectURL = PaperBillOptInRedirectURL_DEV;
            public const string DBRSSOURL = DBRSSOURL_DEV;
#else
            public const string DBROriginURL = DBROriginURL_PROD;
            public const string StartDBRRedirectURL = StartDBRRedirectURL_PROD;
            public const string PaperBillOptInDBRRedirectURL = PaperBillOptInRedirectURL_PROD;
            public const string DBRSSOURL = DBRSSOURL_PROD;
#endif
        }

#if DEBUG
        internal const string Environment = PROD;
        internal const string XAPIKey = XAPIKey_SIT;
        public const string SaltKey = SaltKey_DEV;
        public const string PassPhrase = Passphrase_DEV;
#elif MASTER || SIT
        internal const string Environment = PROD;
        internal const string XAPIKey = XAPIKey_SIT;
        public const string SaltKey = SaltKey_DEV;
        public const string PassPhrase = Passphrase_DEV;
#else
        internal const string Environment = PROD;
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
            internal const string GenerateAccessToken = "GenerateAccessToken";
            internal const string GetBillRendering = "GetBillRendering";

            public const string GetEligibility = "GetEligibility";
        }

    }
}