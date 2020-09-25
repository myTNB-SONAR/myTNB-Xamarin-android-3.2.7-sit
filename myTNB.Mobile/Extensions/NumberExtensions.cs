using System;
using System.Globalization;

namespace myTNB.Mobile.Extensions
{
    internal static class NumberExtensions
    {
        internal static string ToAmountDisplayString(this double number, bool hasCurrency = false)
        {
            if (hasCurrency)
            {
                return AppendCurrency(number.ToString("N2", CultureInfo.InvariantCulture));
            }
            return number.ToString("N2", CultureInfo.InvariantCulture);
        }

        internal static string ToAmountDisplayString(this int number, bool hasCurrency = false)
        {
            if (hasCurrency)
            {
                return AppendCurrency(number.ToString("N2", CultureInfo.InvariantCulture));
            }
            return number.ToString("N2", CultureInfo.InvariantCulture);
        }

        internal static string ToAmountDisplayString(this float number, bool hasCurrency = false)
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
                , Constants.Constants_Currency
                , amount);
        }
    }
}