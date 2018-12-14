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

        /// <summary>
        /// Gets the build version.
        /// </summary>
        /// <returns>The build version.</returns>
        public static string GetBuildVersion()
        {
            return NSBundle.MainBundle.InfoDictionary["CFBundleVersion"].ToString();
        }
    }
}
