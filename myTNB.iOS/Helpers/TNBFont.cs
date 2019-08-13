using UIKit;

namespace myTNB
{
    public static class TNBFont
    {
        readonly static string FONTNAME_300 = "MuseoSans-300";
        readonly static string FONTNAME_500 = "MuseoSans-500";
        readonly static float _sreenHeight = (float)UIScreen.MainScreen.Bounds.Height;
        readonly static float _baseHeight = 568;

        readonly static UIFont _m300_9 = UIFont.FromName(FONTNAME_300, _sreenHeight * (9F / _baseHeight));
        readonly static UIFont _m300_10 = UIFont.FromName(FONTNAME_300, _sreenHeight * (10F / _baseHeight));
        readonly static UIFont _m300_11 = UIFont.FromName(FONTNAME_300, _sreenHeight * (11F / _baseHeight));
        readonly static UIFont _m300_12 = UIFont.FromName(FONTNAME_300, _sreenHeight * (12F / _baseHeight));
        readonly static UIFont _m300_13 = UIFont.FromName(FONTNAME_300, _sreenHeight * (13F / _baseHeight));
        readonly static UIFont _m300_14 = UIFont.FromName(FONTNAME_300, _sreenHeight * (14F / _baseHeight));
        readonly static UIFont _m300_16 = UIFont.FromName(FONTNAME_300, _sreenHeight * (16F / _baseHeight));
        readonly static UIFont _m300_18 = UIFont.FromName(FONTNAME_300, _sreenHeight * (18F / _baseHeight));
        readonly static UIFont _m300_20 = UIFont.FromName(FONTNAME_300, _sreenHeight * (20F / _baseHeight));
        readonly static UIFont _m300_22 = UIFont.FromName(FONTNAME_300, _sreenHeight * (22F / _baseHeight));
        readonly static UIFont _m300_24 = UIFont.FromName(FONTNAME_300, _sreenHeight * (24F / _baseHeight));
        readonly static UIFont _m300_26 = UIFont.FromName(FONTNAME_300, _sreenHeight * (26F / _baseHeight));

        readonly static UIFont _m500_9 = UIFont.FromName(FONTNAME_500, _sreenHeight * (9F / _baseHeight));
        readonly static UIFont _m500_10 = UIFont.FromName(FONTNAME_500, _sreenHeight * (10F / _baseHeight));
        readonly static UIFont _m500_11 = UIFont.FromName(FONTNAME_500, _sreenHeight * (11F / _baseHeight));
        readonly static UIFont _m500_12 = UIFont.FromName(FONTNAME_500, _sreenHeight * (12F / _baseHeight));
        readonly static UIFont _m500_13 = UIFont.FromName(FONTNAME_500, _sreenHeight * (13F / _baseHeight));
        readonly static UIFont _m500_14 = UIFont.FromName(FONTNAME_500, _sreenHeight * (14F / _baseHeight));
        readonly static UIFont _m500_16 = UIFont.FromName(FONTNAME_500, _sreenHeight * (16F / _baseHeight));
        readonly static UIFont _m500_18 = UIFont.FromName(FONTNAME_500, _sreenHeight * (18F / _baseHeight));
        readonly static UIFont _m500_20 = UIFont.FromName(FONTNAME_500, _sreenHeight * (20F / _baseHeight));
        readonly static UIFont _m500_22 = UIFont.FromName(FONTNAME_500, _sreenHeight * (22F / _baseHeight));
        readonly static UIFont _m500_24 = UIFont.FromName(FONTNAME_500, _sreenHeight * (24F / _baseHeight));
        readonly static UIFont _m500_26 = UIFont.FromName(FONTNAME_500, _sreenHeight * (26F / _baseHeight));

        public static UIFont MuseoSans_9_300 { get { return _m300_9; } }
        public static UIFont MuseoSans_10_300 { get { return _m300_10; } }
        public static UIFont MuseoSans_11_300 { get { return _m300_11; } }
        public static UIFont MuseoSans_12_300 { get { return _m300_12; } }
        public static UIFont MuseoSans_13_300 { get { return _m300_13; } }
        public static UIFont MuseoSans_14_300 { get { return _m300_14; } }
        public static UIFont MuseoSans_16_300 { get { return _m300_16; } }
        public static UIFont MuseoSans_18_300 { get { return _m300_18; } }
        public static UIFont MuseoSans_20_300 { get { return _m300_20; } }
        public static UIFont MuseoSans_22_300 { get { return _m300_22; } }
        public static UIFont MuseoSans_24_300 { get { return _m300_24; } }
        public static UIFont MuseoSans_26_300 { get { return _m300_26; } }

        public static UIFont MuseoSans_9_500 { get { return _m500_9; } }
        public static UIFont MuseoSans_10_500 { get { return _m500_10; } }
        public static UIFont MuseoSans_11_500 { get { return _m500_11; } }
        public static UIFont MuseoSans_12_500 { get { return _m500_12; } }
        public static UIFont MuseoSans_13_500 { get { return _m500_13; } }
        public static UIFont MuseoSans_14_500 { get { return _m500_14; } }
        public static UIFont MuseoSans_16_500 { get { return _m500_16; } }
        public static UIFont MuseoSans_18_500 { get { return _m500_18; } }
        public static UIFont MuseoSans_20_500 { get { return _m500_20; } }
        public static UIFont MuseoSans_22_500 { get { return _m500_22; } }
        public static UIFont MuseoSans_24_500 { get { return _m500_24; } }
        public static UIFont MuseoSans_26_500 { get { return _m500_26; } }
    }
}