using System;

namespace myTNB_Android.Src.Utils
{
    public static class StringUtils
    {
        public static string ReverseString(this string data)
        {
            char[] charArray = data.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static bool IsValid(this string str)
        {
            return !string.IsNullOrEmpty(str)
                && !string.IsNullOrWhiteSpace(str);
        }
    }
}