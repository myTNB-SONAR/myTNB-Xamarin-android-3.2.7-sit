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
        /// <summary>
        /// Checks if device is iPhone X and higher resolution
        /// </summary>
        /// <returns><c>true</c>, if iPhone X, <c>false</c> otherwise.</returns>
        public static bool IsIphoneXUpResolution()
        {
            return UIScreen.MainScreen.NativeBounds.Height >= 2436;
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
        /// Gets the OS Version.
        /// </summary>
        /// <returns>The OSV ersion.</returns>
        public static string GetOSVersion()
        {
            return UIDevice.CurrentDevice.SystemVersion;
        }
    }
}