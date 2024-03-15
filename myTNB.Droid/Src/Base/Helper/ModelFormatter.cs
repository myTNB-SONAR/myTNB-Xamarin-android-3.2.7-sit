using System;
using Java.Text;
using Java.Util;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.Base.Helper
{
    public class ModelFormatter
    {
        public static string GetFormattedDate(string stringDate, string originalFormat, string resultFormat)
        {
            Date dateInstance;
            SimpleDateFormat dateParser = new SimpleDateFormat(originalFormat, LocaleUtils.GetDefaultLocale());
            SimpleDateFormat dateFormatter = new SimpleDateFormat(resultFormat, LocaleUtils.GetCurrentLocale());
            dateInstance = dateParser.Parse(stringDate);
            return dateFormatter.Format(dateInstance);
        }
    }
}
