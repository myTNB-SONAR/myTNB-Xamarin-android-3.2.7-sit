using System;
using System.Globalization;
using myTNB.Mobile;

namespace myTNB_Android.Src.Utils
{
    public static class NumberUtils
    {
        public static string ToAmountDisplayString(this double number, bool hasCurrency = false)
        {
            if (hasCurrency)
            {
                return AppendCurrency(number.ToString("N2", CultureInfo.InvariantCulture));
            }
            return number.ToString("N2", CultureInfo.InvariantCulture);
        }

        private static string AppendCurrency(string amount)
        {
            string format = "{0} {1}";
            return string.Format(format
                , "RM"
                , amount);
        }
    }
}

