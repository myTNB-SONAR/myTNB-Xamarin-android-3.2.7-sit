using Java.Util;

namespace myTNB.Android.Src.Utils
{
	public class LocaleUtils
    {
		public static Locale GetCurrentLocale()
		{
			return (LanguageUtil.GetAppLanguage().ToUpper() == "MS") ? new Locale("ms", "MY") : new Locale("en", "US");
		}

        public static Locale GetDefaultLocale()
        {
            return new Locale("en", "US");
        }
    }
}
