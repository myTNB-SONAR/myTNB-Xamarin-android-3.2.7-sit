namespace myTNB.Mobile.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValid(this string key)
        {
            return !string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key);
        }
    }
}