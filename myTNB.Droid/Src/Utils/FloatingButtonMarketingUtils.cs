using System;
using myTNB.Android.Src.SitecoreCMS.Model;

namespace myTNB.Android.Src.Utils
{
	public class FloatingButtonMarketingUtils
	{
        private static FloatingButtonMarketingModel cacheFloatingButtonMarketing = null;

        public static FloatingButtonMarketingModel GetContent()
        {
            return cacheFloatingButtonMarketing;
        }

        public static void SetFloatingButtonMarketingBitmap(FloatingButtonMarketingModel item)
        {
            cacheFloatingButtonMarketing = item;
        }
    }
}

