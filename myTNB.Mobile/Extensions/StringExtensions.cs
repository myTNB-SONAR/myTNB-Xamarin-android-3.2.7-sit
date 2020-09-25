namespace myTNB.Mobile.Extensions
{
    internal static class StringExtensions
    {
        internal static bool IsValid(this string key)
        {
            return !string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key);
        }
    }
}