using Android.Graphics;
using myTNB.SitecoreCMS.Model;

namespace myTNB.Android.Src.Utils
{
    public class AppLaunchUtils
    {
        private static AppLaunchModel cacheAppLaunch = null;

        public static AppLaunchModel GetAppLaunch()
        {
            return cacheAppLaunch;
        }

        public static void SetAppLaunchBitmap(AppLaunchModel item)
        {
            cacheAppLaunch = item;
        }
    }
}
