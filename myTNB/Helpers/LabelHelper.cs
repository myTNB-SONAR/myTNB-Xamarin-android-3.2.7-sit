using System;
using System.Drawing;
using System.Text.RegularExpressions;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public static class LabelHelper
    {
        /// <summary>
        /// Validates the label.
        /// </summary>
        /// <returns><c>true</c>, if label was validated, <c>false</c> otherwise.</returns>
        /// <param name="text">Text.</param>
        /// <param name="regexPattern">Regex pattern.</param>
        public static bool ValidateLabel(string text, string regexPattern)
        {
            Regex regex = new Regex(regexPattern);
            Match match = regex.Match(text);
            return match.Success;
        }

        /// <summary>
        /// Formats the number to decimal with 2 decimal places as default
        /// </summary>
        /// <returns>The number to decimal.</returns>
        /// <param name="amount">Amount.</param>
        public static string FormatNumberToDecimal(string amount, double decimalPlaces = 2)
        {
            decimal value = !string.IsNullOrEmpty(amount.Trim()) ? decimal.Parse(amount) : 0;
            decimal integralValue = Math.Truncate(value);

            decimal fraction = value - integralValue;

            decimal factor = (decimal)Math.Pow(10, decimalPlaces);

            decimal truncatedFraction = Math.Truncate(fraction * factor) / factor;

            decimal result = integralValue + truncatedFraction;

            return string.Format("{0:0.00}", result);
        }

        /// <summary>
        /// Gets the size of the label.
        /// </summary>
        /// <returns>The label size.</returns>
        /// <param name="label">Label.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public static CGSize GetLabelSize(UILabel label, nfloat width, nfloat height)
        {
            return label.Text.StringSize(label.Font, new SizeF((float)width, (float)height));
        }
    }
}