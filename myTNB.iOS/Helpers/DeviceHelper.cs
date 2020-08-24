using System;
using System.Diagnostics;
using UIKit;

namespace myTNB
{
    public static class DeviceHelper
    {
        /// <summary>
        /// iPhone 5s screen Base values 
        /// </summary>
        const float baseScreenWidth = 320f;
        const float baseScreenHeight = 568f;

        /// <summary>
        /// Gets the size of the image.
        /// </summary>
        /// <returns>The image size.</returns>
        /// <param name="width">Width.</param>
        public static string GetImageSize(int width)
        {
            float height = (float)UIScreen.MainScreen.NativeBounds.Height;
            string size = "main";

            if (height <= 480)
            {
                size = "main";
            }
            else if (height > 480 && height < 2208)
            {
                size = "2x";
            }
            else if (height >= 2208)
            {
                size = "3x";
            }
            return size;
        }

        public static string GetImageSize()
        {
            return GetImageSize(0);
        }

        /// <summary>
        /// Checks if device is iPhone X and higher resolution
        /// </summary>
        /// <returns><c>true</c>, if iPhone X, <c>false</c> otherwise.</returns>
        public static bool IsIphoneXUpResolution()
        {
            // xsmax    =   2688
            // x & xs   =   2436
            // xr       =   1792
            return UIScreen.MainScreen.NativeBounds.Height >= 2436
                || UIScreen.MainScreen.NativeBounds.Height == 1792;
        }

        public static bool IsIphoneXOrXs()
        {
            return UIScreen.MainScreen.NativeBounds.Height == 2436
                && UIScreen.MainScreen.NativeBounds.Width == 1125;
        }

        /// <summary>
        /// Checks if device is iPhone X
        /// </summary>
        /// <returns><c>true</c>, if iPhone X, <c>false</c> otherwise.</returns>
        public static bool IsIphoneX()
        {
            return UIScreen.MainScreen.NativeBounds.Height == 2436;
        }
        /// <summary>
        /// Checks if device is iPhone 6 and higher resolution
        /// </summary>
        /// <returns><c>true</c>, if iPhone 6 and higher resolution, <c>false</c> otherwise.</returns>
        public static bool IsIphone6UpResolution()
        {
            return UIScreen.MainScreen.NativeBounds.Height >= 1334;
        }

        public static bool IsIphone678PlusResolution()
        {
            return UIScreen.MainScreen.NativeBounds.Height > 1334 && !IsIphoneXUpResolution();
        }

        /// <summary>
        /// Checks if device is iPhone 6, 7 or 8
        /// </summary>
        /// <returns></returns>
        public static bool IsIphone678()
        {
            return UIScreen.MainScreen.NativeBounds.Height == 1334;
        }

        /// <summary>
        /// Checks if device is iPhone 5/5s
        /// </summary>
        /// <returns><c>true</c>, if iPhone5/5s, <c>false</c> otherwise.</returns>
        public static bool IsIphone5()
        {
            return UIScreen.MainScreen.NativeBounds.Height == 1136;
        }
        /// <summary>
        /// Checks if device is iPhone 4
        /// </summary>
        /// <returns><c>true</c>, if iPhone4, <c>false</c> otherwise.</returns>
        public static bool IsIphone4()
        {
            return UIScreen.MainScreen.NativeBounds.Height == 960;
        }
        /// <summary>
        /// Gets scaled size.
        /// </summary>
        /// <returns>The scaled size.</returns>
        /// <param name="percentage">Percentage.</param>
        public static float GetScaledSize(float percentage)
        {
            float screenHeight = (float)UIScreen.MainScreen.Bounds.Height;
            return screenHeight * (percentage / 100);
        }
        /// <summary>
        /// Gets scaled size by width.
        /// </summary>
        /// <returns>The scaled size by width.</returns>
        /// <param name="percentage">Percentage.</param>
        public static float GetScaledSizeByWidth(float percentage)
        {
            float screenWidth = (float)UIScreen.MainScreen.Bounds.Width;
            return screenWidth * (percentage / 100);
        }
        /// <summary>
        /// Gets scaled size by height.
        /// </summary>
        /// <returns>The scaled size by height.</returns>
        /// <param name="percentage">Percentage.</param>
        public static float GetScaledSizeByHeight(float percentage)
        {
            float screenHeight = (float)UIScreen.MainScreen.Bounds.Height;
            return screenHeight * (percentage / 100);
        }
        /// <summary>
        /// Gets scaled size by height.
        /// </summary>
        /// <returns>The scaled size by height.</returns>
        /// <param name="percentage">Percentage.</param>
        /// <param name="containerHeight">Container height.</param>
        public static float GetScaledSizeByHeight(float percentage, double containerHeight)
        {
            return (float)containerHeight * (percentage / 100);
        }
        /// <summary>
        /// Gets scaled size by width.
        /// </summary>
        /// <returns>The scaled size by width.</returns>
        /// <param name="percentage">Percentage.</param>
        /// <param name="containerWidth">Container width.</param>
        public static float GetScaledSizeByWidth(float percentage, double containerWidth)
        {
            return (float)containerWidth * (percentage / 100);
        }
        /// <summary>
        /// Gets the scaled width.
        /// </summary>
        /// <returns>The scaled width.</returns>
        /// <param name="width">Width.</param>
        public static float GetScaledWidth(float width)
        {
            float screenWidth = (float)UIScreen.MainScreen.Bounds.Width;
            return width * (screenWidth / baseScreenWidth);
        }
        /// <summary>
        /// Gets the scaled height.
        /// </summary>
        /// <returns>The scaled height.</returns>
        /// <param name="height">Height.</param>
        public static float GetScaledHeight(float height)
        {
            float adj = IsIphoneXUpResolution() ? 0.8f : 1;
            float screenHeight = (float)UIScreen.MainScreen.Bounds.Height;
            return height * (screenHeight / baseScreenHeight) * adj;
        }
        /// <summary>
        /// Gets the scaled height using y-axis.
        /// </summary>
        /// <returns>The scaled height using y-axis.</returns>
        /// <param name="y">The y coordinate.</param>
        public static float GetScaledHeightWithY(float y)
        {
            float adj = IsIphoneXUpResolution() ? 1.3f : 1;
            float screenHeight = (float)UIScreen.MainScreen.Bounds.Height;
            return y * (screenHeight / baseScreenHeight) * adj;
        }
        /// <summary>
        /// Gets the X value to center obj using obj width.
        /// </summary>
        /// <returns>The X value.</returns>
        /// <param name="objWidth">Object width.</param>
        public static float GetCenterXWithObjWidth(float objWidth, UIView parentView = null)
        {
            float screenWidth = (float)((parentView != null) ? parentView.Frame.Width : (float)UIScreen.MainScreen.Bounds.Width);
            return screenWidth / 2 - (objWidth / 2);
        }
        /// <summary>
        /// Gets the Y value to center obj using obj height.
        /// </summary>
        /// <returns>The Y value.</returns>
        /// <param name="objHeight">Object height.</param>
        public static float GetCenterYWithObjHeight(float objHeight, UIView parentView = null)
        {
            float screenHeight = (float)((parentView != null) ? parentView.Frame.Height : (float)UIScreen.MainScreen.Bounds.Height);
            return screenHeight / 2 - (objHeight / 2);
        }
        /// <summary>
        /// Gets the OS Version.
        /// </summary>
        /// <returns>The OSV ersion.</returns>
        public static string GetOSVersion()
        {
            return UIDevice.CurrentDevice.SystemVersion;
        }
        /// <summary>
        /// Gets the Device Status Bar Height
        /// </summary>
        /// <returns></returns>
        public static nfloat GetStatusBarHeight()
        {
            return UIApplication.SharedApplication.StatusBarFrame.Size.Height;
        }

        public static bool IsNotched
        {
            get
            {
                try
                {
                    if (IsIOS10AndBelow) { return false; }

                    return UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Bottom > 0;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("IsNotched Error: " + e.Message);
                    return IsIphoneXUpResolution();
                }
            }
        }

        public static nfloat TopSafeAreaInset
        {
            get
            {
                try
                {
                    if (IsIOS10AndBelow) { return 0; }

                    return UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Top;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("TopSafeAreaInset Error: " + e.Message);
                    return 0;
                }
            }
        }

        public static nfloat BottomSafeAreaInset
        {
            get
            {
                try
                {
                    if (IsIOS10AndBelow) { return 0; }
                    return UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Bottom;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("BottomSafeAreaInset Error: " + e.Message);
                    return 0;
                }
            }
        }

        public static bool IsIOS10AndBelow
        {
            get
            {
                try
                {
                    return !UIDevice.CurrentDevice.CheckSystemVersion(11, 0);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("IsIOS10 Exception: " + e.Message);
                }
                return false;
            }
        }

        public static bool IsIOS13AndUp
        {
            get
            {
                try
                {
                    return UIDevice.CurrentDevice.CheckSystemVersion(13, 0);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("IsIOS13 Exception: " + e.Message);
                }
                return false;
            }
        }
    }
}