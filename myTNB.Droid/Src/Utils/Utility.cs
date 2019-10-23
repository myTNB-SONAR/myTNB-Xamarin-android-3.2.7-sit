using Android.Content.PM;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using myTNB;
using System;
using System.Text.RegularExpressions;

namespace myTNB_Android.Src.Utils
{
    public class Utility
    {
        public Utility()
        {
        }


        public static bool AccountNumberValidation(int length)
        {
            return (length == 12 || length == 14);
        }

        public static bool AddAccountNumberValidation(int length)
        {
            return (length == 12);
        }

        public static bool isAlphaNumeric(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z0-9\s,]*$");
            return rg.IsMatch(strToCheck);
        }

        public static bool isAlpha(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z\s,]*$");
            return rg.IsMatch(strToCheck);
        }


        public static bool IsValidMobileNumber(string mobileNumber)
        {
            if (!string.IsNullOrEmpty(mobileNumber))
            {
                if (mobileNumber.StartsWith("+60"))
                {
                    if (mobileNumber.Substring(3).Length == 9 || mobileNumber.Substring(3).Length == 10)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public static void LoggingNonFatalError(Exception e)
        {
            Crashlytics.Crashlytics.LogException(new Java.Lang.Throwable(e.ToString()));
        }


        public static bool IsPermissionHasCount(Permission[] grantResults)
        {
            return (grantResults != null && grantResults.Length > 0);
        }

        public static SpannableString GetFormattedURLString(ClickableSpan clickableSpan, Java.Lang.ICharSequence charSequence)
        {
            SpannableString s = new SpannableString(charSequence);
            Java.Lang.Object[] urlSpans = s.GetSpans(0, s.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));
            if (urlSpans.Length != 0)
            {
                int startFAQLink = s.GetSpanStart(urlSpans[0]);
                int endFAQLink = s.GetSpanEnd(urlSpans[0]);
                s.RemoveSpan(urlSpans[0]);
                s.SetSpan(clickableSpan, startFAQLink, endFAQLink, SpanTypes.ExclusiveExclusive);
            }
            return s;
        }

        /// <summary>
        /// Gets the label based on selected language.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetLocalizedLabel(string pageId, string key)
        {
            string label = "";
            try
            {
                label = LanguageManager.Instance.GetValuesByPage(pageId)[key];
            }
            catch (Exception e)
            {
                Log.Debug("DEBUG Error: ", e.Message);
            }
            return label;
        }
    }
}
