﻿using System;

namespace myTNB.AndroidApp.Src.SiteCore
{
    internal static class SiteCoreConfig
    {
        internal static string OS = "droid";
        internal static string DEFAULT_LANGUAGE = "en";

        internal static readonly TimeSpan FiveSecondTimeSpan = TimeSpan.FromMilliseconds(5000);
        internal static readonly TimeSpan TenSecondTimeSpan = TimeSpan.FromMilliseconds(10000);

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
        //public static string SITECORE_URL = "http://tnbcsdevapp.tnb.my/";  //dev
        // public static string SITECORE_URL ="http://tnbcsstgapp.tnb.my/";   // "http:10.215.215.70.248"   // SIT 
        public static string SITECORE_URL = "https://sitecore.tnb.com.my/";  //prod
#elif SIT
        public static string SITECORE_URL = "https://sitecore.tnb.com.my/";  //"http://10.215.70.248/";
        //"http://tnbcsstgapp.tnb.my/"; //"http://tnbcsdevapp.tnb.my/"; 
#else
        public static string SITECORE_URL = "https://sitecore.tnb.com.my/";
#endif
        public static string SITECORE_USERNAME = "api_user";
        public static string SITECORE_PASSWORD = "mytnbapiuser!3$@2";
        
        // internal static string SITECORE_URL = myTNB.Mobile.MobileConstants.SitecoreURL;
        // internal static string SITECORE_USERNAME = myTNB.Mobile.MobileConstants.SitecoreUsername;
        // internal static string SITECORE_PASSWORD = myTNB.Mobile.MobileConstants.SitecorePassword;        
    }
}