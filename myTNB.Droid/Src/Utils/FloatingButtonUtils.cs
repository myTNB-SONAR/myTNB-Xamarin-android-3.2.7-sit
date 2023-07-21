using System;
using myTNB_Android.Src.SitecoreCMS.Model;

namespace myTNB_Android.Src.Utils
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

