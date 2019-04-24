using UIKit;

namespace myTNB
{
    public static class MyTNBFont
    {
        const string FONTNAME = "Museo Sans";
        public const string FONTNAME_300 = "MuseoSans-300";
        public const string FONTNAME_500 = "MuseoSans-500";

        internal static UIFont GetFont(float size)
        {
            return UIFont.FromName(FONTNAME, size);
        }

        static UIFont GetFont300(float size)
        {
            return UIFont.FromName(FONTNAME_300, size);
        }
        static UIFont GetFont500(float size)
        {
            return UIFont.FromName(FONTNAME_500, size);
        }
        /// <summary>
        /// Museos 500 with font size of 9.
        /// </summary>
        public static UIFont MuseoSans9
        {
            get
            {
                return GetFont(9f);
            }
        }
        /// <summary>
        /// Museos 500 with font size of 10.
        /// </summary>
        public static UIFont MuseoSans10
        {
            get
            {
                return GetFont(10f);
            }
        }
        /// <summary>
        /// Museos 500 with font size of 12.
        /// </summary>
        public static UIFont MuseoSans12
        {
            get
            {
                return GetFont(12f);
            }
        }
        /// <summary>
        /// Museos 500 with font size of 14.
        /// </summary>
        public static UIFont MuseoSans14
        {
            get
            {
                return GetFont(14f);
            }
        }
        /// <summary>
        /// Museos 500 with font size of 16.
        /// </summary>
        public static UIFont MuseoSans16
        {
            get
            {
                return GetFont(16f);
            }
        }
        /// <summary>
        /// Museos 500 with font size of 24.
        /// </summary>
        public static UIFont MuseoSans24
        {
            get
            {
                return GetFont(24f);
            }
        }
        /// <summary>
        /// Museos 300 with font size of 9.
        /// </summary>
        public static UIFont MuseoSans9_300
        {
            get
            {
                return GetFont300(9f);
            }
        }
        /// <summary>
        /// Museos 300 with font size of 10.
        /// </summary>
        public static UIFont MuseoSans10_300
        {
            get
            {
                return GetFont300(10f);
            }
        }
        /// <summary>
        /// Museos 300 with font size of 11.
        /// </summary>
        public static UIFont MuseoSans11_300
        {
            get
            {
                return GetFont300(11f);
            }
        }
        /// <summary>
        /// Museos 300 with font size of 12.
        /// </summary>
        public static UIFont MuseoSans12_300
        {
            get
            {
                return GetFont300(12f);
            }
        }
        /// <summary>
        /// Museos 300 with font size of 13.
        /// </summary>
        /// <returns>The sans13 300.</returns>
        public static UIFont MuseoSans13_300
        {
            get
            {
                return GetFont300(13f);
            }
        }
        /// <summary>
        /// Museos 300 with font size of 14.
        /// </summary>
        public static UIFont MuseoSans14_300
        {
            get
            {
                return GetFont300(14f);
            }
        }
        /// <summary>
        /// Museos 300 with font size of 16.
        /// </summary>
        public static UIFont MuseoSans16_300
        {
            get
            {
                return GetFont300(16f);
            }
        }
        /// <summary>
        /// Museos 300 with font size of 18.
        /// </summary>
        public static UIFont MuseoSans18_300
        {
            get
            {
                return GetFont300(18f);
            }
        }
        /// <summary>
        /// Museos 300 with font size of 24.
        /// </summary>
        public static UIFont MuseoSans24_300
        {
            get
            {
                return GetFont300(24f);
            }
        }
        /// <summary>
        /// Museos 500 with font size of 10.
        /// </summary>
        public static UIFont MuseoSans10_500
        {
            get
            {
                return GetFont500(10f);
            }
        }
        /// <summary>
        /// Museos 500 with font size of 12.
        /// </summary>
        public static UIFont MuseoSans12_500
        {
            get
            {
                return GetFont500(12f);
            }
        }
        /// <summary>
        /// Museos 500 with font size of 14.
        /// </summary>
        public static UIFont MuseoSans14_500
        {
            get
            {
                return GetFont500(14f);
            }
        }
        /// <summary>
        /// Museos 500 with font size of 16.
        /// </summary>
        public static UIFont MuseoSans16_500
        {
            get
            {
                return GetFont500(16f);
            }
        }
        /// <summary>
        /// Museos 500 with font size of 18.
        /// </summary>
        public static UIFont MuseoSans18_500
        {
            get
            {
                return GetFont500(18f);
            }
        }
        /// <summary>
        /// Museos 500 with font size of 24.
        /// </summary>
        public static UIFont MuseoSans24_500
        {
            get
            {
                return GetFont500(24f);
            }
        }
        /// <summary>
        /// Museos 500 with font size of 26.
        /// </summary>
        public static UIFont MuseoSans26_500
        {
            get
            {
                return GetFont500(26f);
            }
        }
        /// <summary>
        /// Museos 500 with font size of 20.
        /// </summary>
        public static UIFont MuseoSans20_500
        {
            get
            {
                return GetFont500(20f);
            }
        }
    }
}