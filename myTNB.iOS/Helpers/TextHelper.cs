using System;
using System.Globalization;
using Foundation;
using UIKit;
using System.Security;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace myTNB
{
    public static class TextHelper
    {
        /// <summary>
        /// Converts to html with css.
        /// </summary>
        /// <returns>The to html with css.</returns>
        /// <param name="htmlContent">Html content.</param>
        /// <param name="styleCss">Style css.</param>
        /// <param name="htmlCampaignPeriodError">Html campaign period error.</param>
        public static NSAttributedString ConvertToHtmlWithCss(string htmlContent, string styleCss, ref NSError htmlCampaignPeriodError)
        {
            var htmlStr = string.Format(@"{0}<body>{1}</body>", styleCss, htmlContent);
            NSString nsstr = new NSString(htmlStr, NSStringEncoding.Unicode);
            NSAttributedString attributedString = new NSAttributedString(htmlStr,
                                                                         new NSAttributedStringDocumentAttributes
                                                                         {
                                                                             DocumentType = NSDocumentType.HTML,
                                                                             StringEncoding = NSStringEncoding.UTF8
                                                                         },
                                                                         ref htmlCampaignPeriodError);
            return attributedString;
        }

        /// <summary>
        /// Converts to html with font.
        /// </summary>
        /// <returns>The to html with font.</returns>
        /// <param name="htmlContent">Html content.</param>
        /// <param name="htmlCampaignPeriodError">Html campaign period error.</param>
        /// <param name="fontName">Font name.</param>
        /// <param name="fontSize">Font size.</param>
        public static NSAttributedString ConvertToHtmlWithFont(string htmlContent, ref NSError htmlCampaignPeriodError,
                                                               string fontName, float fontSize)
        {
            try
            {
                var styleCss = string.Format("<style>body{{font-family: '{0}'; font-size:{1}px; }}</style>", fontName, fontSize);
                return ConvertToHtmlWithCss(htmlContent, styleCss, ref htmlCampaignPeriodError);
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("DEBUG - ConvertToHtmlWithFont Error: " + e.Message);
#endif
                return new NSAttributedString(htmlContent ?? string.Empty
                    , new NSAttributedStringDocumentAttributes
                    {
                        DocumentType = NSDocumentType.HTML,
                        StringEncoding = NSStringEncoding.UTF8
                    }
                    , ref htmlCampaignPeriodError);
            }
        }

        /// <summary>
        /// Creates the value pair string.
        /// </summary>
        /// <returns>The value pair string.</returns>
        /// <param name="valueText">Value text.</param>
        /// <param name="pairText">Pair text.</param>
        /// <param name="isValuePositionRight">If set to <c>true</c> is value position right.</param>
        /// <param name="valueFont">Value font.</param>
        /// <param name="valueColor">Value color.</param>
        /// <param name="pairFont">Pair font.</param>
        /// <param name="pairColor">Pair color.</param>
        public static NSMutableAttributedString CreateValuePairString(string valueText, string pairText, bool isValuePositionRight,
                                                                      UIFont valueFont, UIColor valueColor,
                                                                      UIFont pairFont, UIColor pairColor)
        {
            var attrStringValue = new NSMutableAttributedString(valueText ?? "0", font: valueFont,
                                                                foregroundColor: valueColor);
            var attrStringPair = new NSMutableAttributedString(pairText ?? "0", font: pairFont,
                                                               foregroundColor: pairColor);
            if (isValuePositionRight)
            {
                attrStringPair.Append(attrStringValue);
                return attrStringPair;
            }

            attrStringValue.Append(attrStringPair);
            return attrStringValue;
        }

        /// <summary>
        /// Parses the string to double.
        /// </summary>
        /// <returns>The string to double.</returns>
        /// <param name="text">Text.</param>
        public static double ParseStringToDouble(string text)
        {
            double parsedAmount = 0;
            if (!string.IsNullOrWhiteSpace(text))
            {
                if (double.TryParse(text, NumberStyles.Any & (~NumberStyles.AllowCurrencySymbol), CultureInfo.InvariantCulture, out parsedAmount))
                {
                    return parsedAmount;
                }
                else if (double.TryParse(text, out parsedAmount))
                {
                    return parsedAmount;
                }
            }

            return 0;
        }

        /// <summary>
        /// Converts the string to secure string.
        /// </summary>
        /// <returns>The string to secure string.</returns>
        /// <param name="str">String.</param>
        public static SecureString ConvertStringToSecureString(string str)
        {
            SecureString secureString = new SecureString();
            if (!string.IsNullOrWhiteSpace(str))
            {
                foreach (char ch in str)
                {
                    secureString.AppendChar(ch);
                }
            }
            return secureString;
        }

        /// <summary>
        /// Converts the secure string to string.
        /// </summary>
        /// <returns>The secure string to string.</returns>
        /// <param name="secureString">Secure string.</param>
        public static string ConvertSecureStringToString(SecureString secureString)
        {
            string str = string.Empty;
            IntPtr valuePtr = IntPtr.Zero;
            if (secureString != null)
            {
                try
                {
                    valuePtr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                    str = Marshal.PtrToStringUni(valuePtr);
                    return str;
                }
                finally
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
                }
            }
            return str;
        }
    }
}
