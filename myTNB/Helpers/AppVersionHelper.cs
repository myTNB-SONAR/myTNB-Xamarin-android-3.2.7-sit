using System;
using Foundation;

namespace myTNB
{
    public static class AppVersionHelper
    {
        /// <summary>
        /// Gets the app version.
        /// </summary>
        /// <returns>The app version.</returns>
        public static string GetAppShortVersion()
        {
            return NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"].ToString();
        }
    }
}
