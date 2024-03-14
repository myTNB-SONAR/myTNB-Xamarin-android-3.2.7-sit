using System;
using myTNB.Android.Src.SitecoreCMS.Model;

namespace myTNB.Android.Src.Utils
{
	public class FloatingButtonUtils
	{
        private static FloatingButtonModel cacheFloatingButton = null;

        public static FloatingButtonModel GetFloatingButton()
        {
            return cacheFloatingButton;
        }

        public static void SetFloatingButtonBitmap(FloatingButtonModel item)
        {
            cacheFloatingButton = item;
        }
    }
}

