using UIKit;

namespace myTNB
{
    public static class myTNBFont
    {
        const string FONTNAME = "Museo Sans";

        internal static UIFont GetFont(float size)
        {
            return UIFont.FromName(FONTNAME, size);
        }

        static UIFont GetFont300(float size)
        {
            return UIFont.FromName("MuseoSans-300", size);
        }
        /// <summary>
        /// Museos 500 with font size of 9.
        /// </summary>
        public static UIFont MuseoSans9()
        {
            return GetFont(9f);
        }
        /// <summary>
        /// Museos 500 with font size of 10.
        /// </summary>
        public static UIFont MuseoSans10()
        {
            return GetFont(10f);
        }
        /// <summary>
        /// Museos 500 with font size of 12.
        /// </summary>
        public static UIFont MuseoSans12()
        {
            return GetFont(12f);
        }
        /// <summary>
        /// Museos 500 with font size of 14.
        /// </summary>
        public static UIFont MuseoSans14()
        {
            return GetFont(14f);
        }
        /// <summary>
        /// Museos 500 with font size of 16.
        /// </summary>
        public static UIFont MuseoSans16()
        {
            return GetFont(16f);
        }
        /// <summary>
        /// Museos 500 with font size of 24.
        /// </summary>
        public static UIFont MuseoSans24()
        {
            return GetFont(24f);
        }
        /// <summary>
        /// Museos 300 with font size of 9.
        /// </summary>
        public static UIFont MuseoSans9_300()
        {
            return GetFont300(9f);
        }
        /// <summary>
        /// Museos 300 with font size of 10.
        /// </summary>
        public static UIFont MuseoSans10_300()
        {
            return GetFont300(10f);
        }
        /// <summary>
        /// Museos 300 with font size of 12.
        /// </summary>
        public static UIFont MuseoSans12_300()
        {
            return GetFont300(12f);
        }
        /// <summary>
        /// Museos 300 with font size of 14.
        /// </summary>
        public static UIFont MuseoSans14_300()
        {
            return GetFont300(14f);
        }
        /// <summary>
        /// Museos 300 with font size of 16.
        /// </summary>
        public static UIFont MuseoSans16_300()
        {
            return GetFont300(16f);
        }
        /// <summary>
        /// Museos 300 with font size of 24.
        /// </summary>
        public static UIFont MuseoSans24_300()
        {
            return GetFont300(24f);
        }
    }
}