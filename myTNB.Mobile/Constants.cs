using System;
namespace myTNB.Mobile
{
    public static class Constants
    {
#if DEBUG || MASTER
        public static string ApiDomain = "https://mobiletestingws.tnb.com.my";

        public const string ApiUrlPath = "v5/my_billingssp.asmx";

        public static string ApiKeyId = "9515F2FA-C267-42C9-8087-FABA77CB84DF";

#else
        public static string ApiDomain = "https://mytnbapp.tnb.com.my";

        public const string ApiUrlPath = "v5/my_BillingSSP.asmx";

        public static string ApiKeyId = "E6148656-205B-494C-BC95-CC241423E72F";
#endif

    }
}
