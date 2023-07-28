using System;
using myTNB_Android.Src.SitecoreCMS.Model;

namespace myTNB_Android.Src.Utils
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

