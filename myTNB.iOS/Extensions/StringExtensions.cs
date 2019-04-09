using Foundation;

namespace myTNB
{
    public static class StringExtensions
    {
        /// <summary>
        /// Get localized string with the specified key.
        /// </summary>
        /// <returns>The translate.</returns>
        /// <param name="key">Key.</param>
        public static string Translate(this string key)
        {
            return NSBundle.MainBundle.GetLocalizedString(key, "", "");
        }
    }
}
