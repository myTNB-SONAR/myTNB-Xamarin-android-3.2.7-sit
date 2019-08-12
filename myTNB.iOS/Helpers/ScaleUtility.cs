using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public static class ScaleUtility
    {
        private static nfloat WidthBase = 320;
        private static nfloat HeightBase = 568;

        public static nfloat GetScaledWidth(nfloat value)
        {
            nfloat percentage = value / WidthBase;
            return UIScreen.MainScreen.Bounds.Width * percentage;
        }

        public static nfloat GetScaledHeight(nfloat value)
        {
            nfloat percentage = value / HeightBase;
            return UIScreen.MainScreen.Bounds.Height * percentage;
        }

        public static void GetYLocationFromFrame(CGRect frame, ref nfloat yValue)
        {
            nfloat maxY = frame.GetMaxY();
            yValue = maxY + GetScaledHeight(yValue);
        }

        public static nfloat GetYLocationFromFrame(CGRect frame, nfloat yValue)
        {
            nfloat maxY = frame.GetMaxY();
            yValue = maxY + GetScaledHeight(yValue);
            return yValue;
        }

        public static nfloat GetHeightValue(CGRect frame, nfloat percentage)
        {
            return frame.Height * (percentage / 100);
        }
    }
}