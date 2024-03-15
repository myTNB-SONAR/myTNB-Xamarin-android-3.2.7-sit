using System;
using myTNB.AndroidApp.Src.SitecoreCMS.Model;

namespace myTNB.AndroidApp.Src.Utils
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

