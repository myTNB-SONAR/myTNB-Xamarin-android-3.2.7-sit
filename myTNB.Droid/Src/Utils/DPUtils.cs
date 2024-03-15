using Android.Content;
using Android.Util;
using System;

namespace myTNB.AndroidApp.Src.Utils
{
    public class DPUtils
    {

        internal static float ConvertDPToPixel(float dp)
        {
            var scale = global::Android.App.Application.Context.Resources.DisplayMetrics.Density;
            var px = (int)(dp * scale + 0.5);
            return px;
        }

        internal static float ConvertDPToPx(float dp)
        {
            var scale = global::Android.App.Application.Context.Resources.DisplayMetrics.Density;
            var px = dp * scale;
            return px;
        }

        internal static float ConvertPxToDP(float px)
        {
            var scale = Android.App.Application.Context.Resources.DisplayMetrics.Density;
            var dp = px / scale;
            return dp;
        }

        internal static float GetDensity()
        {
            return Android.App.Application.Context.Resources.DisplayMetrics.Density;
        }

        internal static int GetWidth()
        {
            return global::Android.App.Application.Context.Resources.DisplayMetrics.WidthPixels;
        }

        internal static int GetHeight()
        {
            return global::Android.App.Application.Context.Resources.DisplayMetrics.HeightPixels;
        }

        internal static String GetDeviceDensity(Context context)
        {
            String deviceDensity = "hdpi";
            switch (context.Resources.DisplayMetrics.DensityDpi)
            {
                case DisplayMetricsDensity.Low:
                    deviceDensity = "mdpi";
                    break;
                case DisplayMetricsDensity.Medium:
                    deviceDensity = "mdpi";
                    break;
                case DisplayMetricsDensity.High:
                    deviceDensity = "hdpi";
                    break;
                case DisplayMetricsDensity.Xhigh:
                    deviceDensity = "xhdpi";
                    break;
                case DisplayMetricsDensity.Xxhigh:
                    deviceDensity = "xxhdpi";
                    break;
                case DisplayMetricsDensity.Xxxhigh:
                    deviceDensity = "xxxhdpi";
                    break;
            }
            return deviceDensity;
        }
    }
}