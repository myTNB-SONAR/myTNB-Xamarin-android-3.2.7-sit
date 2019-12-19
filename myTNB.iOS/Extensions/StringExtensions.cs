using System;
using System.Globalization;
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
            //NSBundle languageBundle = LanguageSettings.LanguageBundle ?? NSBundle.MainBundle;
            return key;//languageBundle.GetLocalizedString(key, "", "");
        }

        public static string FormatAmountString(this string amount, string currency, bool ignoreSign = false)
        {
            bool showNegative = false;
            if (string.IsNullOrEmpty(amount) || string.IsNullOrWhiteSpace(amount)) { amount = "0"; }
            if (string.IsNullOrEmpty(currency) || string.IsNullOrWhiteSpace(currency)) { currency = TNBGlobal.UNIT_CURRENCY; }
            double.TryParse(amount, out double parsedAmount);
            if (ignoreSign)
            {
                parsedAmount = Math.Abs(parsedAmount);
            }
            else
            {
                showNegative = parsedAmount < 0;
                parsedAmount = Math.Abs(parsedAmount);
            }
            string formattedAmount = parsedAmount.ToString("N2", CultureInfo.InvariantCulture);
            string amountFormat = showNegative ? "- {0} {1}" : "{0} {1}";
            return string.Format(amountFormat, currency, formattedAmount);
        }

        public static bool IsValid(this string key)
        {
            return !string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key);
        }
    }
}