using System;
using Java.Text;
using Java.Util;

namespace myTNB_Android.Src.Base.Helper
{
    public class ModelFormatter
    {
        public static string GetFormattedDate(string stringDate, string originalFormat, string resultFormat)
        {
            Date dateInstance;
            SimpleDateFormat dateParser = new SimpleDateFormat(originalFormat);
            SimpleDateFormat dateFormatter = new SimpleDateFormat(resultFormat);
            dateInstance = dateParser.Parse(stringDate);
            return dateFormatter.Format(dateInstance);
        }
    }
}
