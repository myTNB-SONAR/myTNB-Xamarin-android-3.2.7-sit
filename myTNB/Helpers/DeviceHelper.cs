using UIKit;

namespace myTNB
{
    public static class DeviceHelper
    {
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
    }
}