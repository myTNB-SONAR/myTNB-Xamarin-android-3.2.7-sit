﻿namespace myTNB_Android.Src.SiteCore
{
    public static class SiteCoreConfig
    {
        public static string OS = "droid";
        public static string DEFAULT_LANGUAGE = "en";

        //Local machine
        //public static string SITECORE_URL = "http://10.215.229.127/";

        //Local machine
        //public static string SITECORE_URL = "http://10.215.229.127/";

        //Sitecore client
        //public static string SITECORE_URL = "http://10.215.70.246/";
        //public static string SITECORE_USERNAME = "admin";
        //public static string SITECORE_PASSWORD = "b";

        //Production
#if DEBUG || DEVELOP || STUB
        //public static string SITECORE_URL = "http://tnbcsdevapp.tnb.my/";
        public static string SITECORE_URL = "https://sitecore.tnb.com.my/";
#elif SIT
        public static string SITECORE_URL = "http://tnbcsstgapp.tnb.my/";
#else
        public static string SITECORE_URL = "https://sitecore.tnb.com.my/";
#endif
        public static string SITECORE_USERNAME = "api_user";
        public static string SITECORE_PASSWORD = "mytnbapiuser!3$@2";
    }
}