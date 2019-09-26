using System;
using UIKit;

namespace myTNB
{
    public static class TNBFont
    {
        readonly static string FONTNAME_300 = "MuseoSans-300";
        readonly static string FONTNAME_500 = "MuseoSans-500";

        private static nfloat WidthBase = 320;
        private static nfloat HeightBase = 568;
        private static nfloat ScreenHeight = UIScreen.MainScreen.Bounds.Height;
        private static nfloat ARBase = HeightBase / WidthBase;
        private static nfloat ARDevice = UIScreen.MainScreen.Bounds.Height / UIScreen.MainScreen.Bounds.Width;
        private static nfloat ARDelta = DeviceHelper.IsNotched ? ARDevice / ARBase : ScreenHeight / HeightBase;

        readonly static UIFont _m300_9 = UIFont.FromName(FONTNAME_300, 9F * ARDelta);
        readonly static UIFont _m300_10 = UIFont.FromName(FONTNAME_300, 10F * ARDelta);
        readonly static UIFont _m300_11 = UIFont.FromName(FONTNAME_300, 11F * ARDelta);
        readonly static UIFont _m300_12 = UIFont.FromName(FONTNAME_300, 12F * ARDelta);
        readonly static UIFont _m300_13 = UIFont.FromName(FONTNAME_300, 13F * ARDelta);
        readonly static UIFont _m300_14 = UIFont.FromName(FONTNAME_300, 14F * ARDelta);
        readonly static UIFont _m300_16 = UIFont.FromName(FONTNAME_300, 16F * ARDelta);
        readonly static UIFont _m300_18 = UIFont.FromName(FONTNAME_300, 18F * ARDelta);
        readonly static UIFont _m300_20 = UIFont.FromName(FONTNAME_300, 20F * ARDelta);
        readonly static UIFont _m300_22 = UIFont.FromName(FONTNAME_300, 22F * ARDelta);
        readonly static UIFont _m300_24 = UIFont.FromName(FONTNAME_300, 24F * ARDelta);
        readonly static UIFont _m300_26 = UIFont.FromName(FONTNAME_300, 26F * ARDelta);
        readonly static UIFont _m300_36 = UIFont.FromName(FONTNAME_300, 36F * ARDelta);

        readonly static UIFont _m500_9 = UIFont.FromName(FONTNAME_500, 9F * ARDelta);
        readonly static UIFont _m500_10 = UIFont.FromName(FONTNAME_500, 10F * ARDelta);
        readonly static UIFont _m500_11 = UIFont.FromName(FONTNAME_500, 11F * ARDelta);
        readonly static UIFont _m500_12 = UIFont.FromName(FONTNAME_500, 12F * ARDelta);
        readonly static UIFont _m500_13 = UIFont.FromName(FONTNAME_500, 13F * ARDelta);
        readonly static UIFont _m500_14 = UIFont.FromName(FONTNAME_500, 14F * ARDelta);
        readonly static UIFont _m500_16 = UIFont.FromName(FONTNAME_500, 16F * ARDelta);
        readonly static UIFont _m500_18 = UIFont.FromName(FONTNAME_500, 18F * ARDelta);
        readonly static UIFont _m500_20 = UIFont.FromName(FONTNAME_500, 20F * ARDelta);
        readonly static UIFont _m500_22 = UIFont.FromName(FONTNAME_500, 22F * ARDelta);
        readonly static UIFont _m500_24 = UIFont.FromName(FONTNAME_500, 24F * ARDelta);
        readonly static UIFont _m500_26 = UIFont.FromName(FONTNAME_500, 26F * ARDelta);

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
        public static UIFont MuseoSans_36_300 { get { return _m300_36; } }
    }
}