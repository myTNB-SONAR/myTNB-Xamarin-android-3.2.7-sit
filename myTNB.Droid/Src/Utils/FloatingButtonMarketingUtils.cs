using System;
using myTNB.AndroidApp.Src.SitecoreCMS.Model;

namespace myTNB.AndroidApp.Src.Utils
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

