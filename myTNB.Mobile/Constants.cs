namespace myTNB.Mobile
{
    internal static class Constants
    {
        internal const int APITimeOut = 60000;
        internal const int SitecoreTimeOut = 5000;
        internal const string SitecoreDomain = "sitecore";
        internal const string SitecoreUsername = "api_user";
        internal const string SitecorePassword = "mytnbapiuser!3$@2";
        internal const string ApiUrlPath = "v6/mytnbappws.asmx";

#if DEBUG
        private const string DEV1 = "http://10.215.128.191:88";
        private const string DEV2 = "http://10.215.128.191:89";
        private const string SIT = "https://mobiletestingws.tnb.com.my";
        private const string PROD = "https://mytnbapp.tnb.com.my";
        private const string DEVUNIFIED = "http://dev.mytnb.com.my:8322";

        internal static string ApiDomain = DEV2;
        internal const string PaymentURL = "http://10.215.128.191:89/v5/PayRedirect.aspx";
        internal const string ApiKeyId = "9515F2FA-C267-42C9-8087-FABA77CB84DF";
        internal const string SitecoreURL = "https://sitecore.tnb.com.my/";// "http://10.215.70.246/";    //"http://tnbcsdevapp.tnb.my/";
#elif MASTER
        internal static string ApiDomain = "https://mobiletestingws.tnb.com.my";
        internal const string PaymentURL = "https://mobiletestingws.tnb.com.my/v5/PayRedirect.aspx";
        internal const string ApiKeyId = "9515F2FA-C267-42C9-8087-FABA77CB84DF";
        internal const string SitecoreURL = "http://10.215.70.248/";    //"http://tnbcsstgapp.tnb.my/";
#else
        internal static string ApiDomain = "https://mytnbapp.tnb.com.my";
        internal const string PaymentURL = "https://mytnbapp.tnb.com.my/v5/PayRedirect.aspx";
        internal const string ApiKeyId = "E6148656-205B-494C-BC95-CC241423E72F";
        internal const string SitecoreURL = "https://sitecore.tnb.com.my/";
#endif
    }
}