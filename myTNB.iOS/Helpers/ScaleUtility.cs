using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public static class ScaleUtility
    {
        private static nfloat WidthBase = 320;
        private static nfloat HeightBase = 568;
        private static nfloat ARBase = HeightBase / WidthBase;
        private static nfloat ARDevice = UIScreen.MainScreen.Bounds.Height / UIScreen.MainScreen.Bounds.Width;
        private static nfloat ARDelta = ARDevice / ARBase;
        private static bool IsNotched = DeviceHelper.IsNotched;

        public static nfloat GetScaledWidth(nfloat value)
        {
            if (IsNotched) { return value * ARDelta; }
            nfloat percentage = value / WidthBase;
            return UIScreen.MainScreen.Bounds.Width * percentage;
        }

        public static nfloat GetWidthByScreenSize(nfloat value)
        {
            nfloat percentage = value / WidthBase;
            return UIScreen.MainScreen.Bounds.Width * percentage;
        }

        public static nfloat GetScaledHeight(nfloat value)
        {
            if (IsNotched) { return value * ARDelta; }
            nfloat percentage = value / HeightBase;
            return UIScreen.MainScreen.Bounds.Height * percentage;
        }

        public static nfloat GetHeightByScreenSize(nfloat value)
        {
            nfloat percentage = value / HeightBase;
            return UIScreen.MainScreen.Bounds.Height * percentage;
        }

        public static nfloat GetPercentageScaledWidth(nfloat value)
        {
            if (IsNotched) { return value * ARDelta; }
            nfloat percentage = value / WidthBase;
            return UIScreen.MainScreen.Bounds.Width * percentage;
        }

        public static void GetYLocationFromFrame(CGRect frame, ref nfloat yValue)
        {
            nfloat maxY = frame.GetMaxY();
            yValue = maxY + GetScaledHeight(yValue);
        }

        public static nfloat GetYLocationFromFrame(CGRect frame, nfloat yValue)
        {
            nfloat maxY = frame.GetMaxY();
            return maxY + GetScaledHeight(yValue);
        }

        public static nfloat GetYLocationFromFrameScreenSize(CGRect frame, nfloat yValue)
        {
            nfloat maxY = frame.GetMaxY();
            return maxY + GetHeightByScreenSize(yValue);
        }

        public static nfloat GetYLocationFromFrame(this nfloat yValue, CGRect frame)
        {
            nfloat maxY = frame.GetMaxY();
            yValue = maxY + GetScaledHeight(yValue);
            return yValue;
        }

        public static nfloat GetHeightValueByPercentage(CGRect refFrame, nfloat percentage)
        {
            return refFrame.Height * (percentage / 100);
        }

        public static nfloat BaseMarginWidth8
        {
            get { return GetScaledWidth(8F); }
        }

        public static nfloat BaseMarginWidth12
        {
            get { return GetScaledWidth(12F); }
        }

        public static nfloat BaseMarginWidth16
        {
            get { return GetScaledWidth(16F); }
        }

        public static nfloat BaseMarginHeight16
        {
            get { return GetScaledHeight(16F); }
        }

        public static void GetValuesFromAspectRatio(ref nfloat width, ref nfloat height)
        {
            nfloat aspectRatio = width / height;
            nfloat percentage = width / WidthBase;
            width = UIScreen.MainScreen.Bounds.Width * percentage;
            height = width * aspectRatio;
        }
        public static nfloat GetYLocationToCenterObject(nfloat objHeight, UIView parentView = null)
        {
            nfloat height = (parentView != null) ? parentView.Frame.Height : UIScreen.MainScreen.Bounds.Height;
            return height / 2 - (objHeight / 2);
        }

        public static nfloat GetXLocationToCenterObject(nfloat objWidth, UIView parentView = null)
        {
            nfloat width = (parentView != null) ? parentView.Frame.Width : UIScreen.MainScreen.Bounds.Width;
            return width / 2 - (objWidth / 2);
        }

        public static nfloat GetPercentHeightValue(CGRect frame, nfloat percentage)
        {
            return frame.Height * (percentage / 100);
        }
    }
}