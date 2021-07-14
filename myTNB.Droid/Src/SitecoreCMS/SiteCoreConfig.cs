using System;

namespace myTNB_Android.Src.SiteCore
{
    internal static class SiteCoreConfig
    {
        internal static string OS = "droid";
        internal static string DEFAULT_LANGUAGE = "en";

        internal static readonly TimeSpan FiveSecondTimeSpan = TimeSpan.FromMilliseconds(5000);
        internal static readonly TimeSpan TenSecondTimeSpan = TimeSpan.FromMilliseconds(10000);

        internal static string SITECORE_URL = myTNB.Mobile.MobileConstants.SitecoreURL;
        internal static string SITECORE_USERNAME = myTNB.Mobile.MobileConstants.SitecoreUsername;
        internal static string SITECORE_PASSWORD = myTNB.Mobile.MobileConstants.SitecorePassword;        
    }
}