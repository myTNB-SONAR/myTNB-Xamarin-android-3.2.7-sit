using Android.Content;
using Android.Util;
using System;

namespace myTNB_Android.Src.Utils
{
    public class DPUtils
    {

        internal static float ConvertDPToPixel(float dp)
        {
            var scale = global::Android.App.Application.Context.Resources.DisplayMetrics.Density;
            var px = (int)(dp * scale + 0.5);
            return px;
        }


        internal static String GetDeviceDensity(Context context)
        {
            String deviceDensity = "hdpi";
            switch (context.Resources.DisplayMetrics.DensityDpi)
            {
                case DisplayMetrics.DensityLow:
                    deviceDensity = "mdpi";
                    break;
                case DisplayMetrics.DensityMedium:
                    deviceDensity = "mdpi";
                    break;
                case DisplayMetrics.DensityHigh:
                    deviceDensity = "hdpi";
                    break;
                case DisplayMetrics.DensityXhigh:
                    deviceDensity = "xhdpi";
                    break;
                case DisplayMetrics.DensityXxhigh:
                    deviceDensity = "xxhdpi";
                    break;
                case DisplayMetrics.DensityXxxhigh:
                    deviceDensity = "xxxhdpi";
                    break;
            }
            return deviceDensity;
        }
    }
}