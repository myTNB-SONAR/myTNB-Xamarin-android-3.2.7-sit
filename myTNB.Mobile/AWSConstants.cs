namespace myTNB.Mobile
{
    public static class AWSConstants
    {
        //AWS DEV
        private const string SaltKeyDEV = "Salt-5123BEB842C046609AD5FB67A0A2D5D0";
        private const string PassphraseDEV = "PW-myTNBDbrSso";
        private const string DBROriginURLDEV = "https://test.mytnb.com.my";
        private const string DBRRedirectURLDEV = "http://EC2Co-EcsEl-11MUE9B1S2T04-1563452123.ap-southeast-1.elb.amazonaws.com/DigitalBill/Start";
        private const string DBRSSOURLDEV = "http://ec2co-ecsel-11mue9b1s2t04-1563452123.ap-southeast-1.elb.amazonaws.com/Sso?s={0}";
        //AWS PROD
        private const string SaltKeyPROD = "Salt-5123BEB842C046609AD5FB67A0A2D5D0";
        private const string PassphrasePROD = "PW-myTNBDbrSso";
        private const string DBROriginURLPROD = "https://test.mytnb.com.my";
        private const string DBRRedirectURLPROD = "http://EC2Co-EcsEl-11MUE9B1S2T04-1563452123.ap-southeast-1.elb.amazonaws.com/DigitalBill/Start";
        private const string DBRSSOURLPROD = "http://ec2co-ecsel-11mue9b1s2t04-1563452123.ap-southeast-1.elb.amazonaws.com/Sso?s={0}";

        internal const int TimeOut = 10000;

        internal const string Channel = "myTNB_API_Mobile";
        internal static int RoleID = 16;

        private const string SIT = "SIT";
        private const string PROD = "Prod";
        private const string XAPIKeySIT = "8BVhUBJIJO9RAFwrxmx0o76n7tfRsPyd4DSChI5r";
        private const string XAPIKeyProd = "8BVhUBJIJO9RAFwrxmx0o76n7tfRsPyd4DSChI5r";

        //Domains
        internal struct Domains
        {
            internal const string GenerateAccessToken = "https://st11n070y9.execute-api.ap-southeast-1.amazonaws.com";
            internal const string GetBillRendering = "https://q7zjjtvkmf.execute-api.ap-southeast-1.amazonaws.com";
            internal const string GetDBREligibility = "https://pjpd3gxnd8.execute-api.ap-southeast-1.amazonaws.com";
        }

#if DEBUG
        internal const string Environment = PROD;
        internal const string XAPIKey = XAPIKeySIT;
        public const string SaltKey = SaltKeyDEV;
        public const string PassPhrase = PassphraseDEV;
        public const string DBROriginURL = DBROriginURLDEV;
        public const string DBRRedirectURL = DBRRedirectURLDEV;
        public const string DBRSSOURL = DBRSSOURLDEV;
#elif MASTER || SIT
        internal const string Environment = PROD;
        internal const string XAPIKey = XAPIKeySIT;
        public const string SaltKey = SaltKeyDEV;
        public const string PassPhrase = PassphraseDEV;
        public const string DBROriginURL = DBROriginURLDEV;
        public const string DBRRedirectURL = DBRRedirectURLDEV;
        public const string DBRSSOURL = DBRSSOURLDEV;
#else
        internal const string Environment = PROD;
        internal const string XAPIKey = XAPIKeyProd;
        public const string SaltKey = SaltKeyPROD;
        public const string PassPhrase = PassphrasePROD;
        public const string DBROriginURL = DBROriginURLPROD;
        public const string DBRRedirectURL = DBRRedirectURLPROD;
        public const string DBRSSOURL = DBRSSOURLPROD;
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
