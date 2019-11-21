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
            NSBundle languageBundle = LanguageSettings.LanguageBundle ?? NSBundle.MainBundle;
            return languageBundle.GetLocalizedString(key, "", "");
        }

        public static string FormatAmountString(this string amount, string currency)
        {
            if (string.IsNullOrEmpty(amount) || string.IsNullOrWhiteSpace(amount)) { amount = "0"; }
            if (string.IsNullOrEmpty(currency) || string.IsNullOrWhiteSpace(currency)) { currency = TNBGlobal.UNIT_CURRENCY; }
            double.TryParse(amount, out double parsedAmount);
            string formattedAmount = parsedAmount.ToString("N2", CultureInfo.InvariantCulture);
            return string.Format("{0} {1}", currency, formattedAmount);
        }

        public static bool IsValid(this string key)
        {
            return !string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key);
        }
    }
}