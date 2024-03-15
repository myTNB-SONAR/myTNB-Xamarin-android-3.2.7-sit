using System;
namespace myTNB.AndroidApp.Src.Utils.LinkRedirection
{
    public class LinkRedirection
    {
        public class Constants
        {
            internal static readonly string DateFormat = "M/d/yyyy h:m:s tt";
            internal static readonly string Pattern = "\\b{0}.*\\b";
            internal static readonly string ReplaceKey = "{0}=";
            internal static readonly string InAppScreenKey = "inAppScreen";
        }

        public enum ScreenEnum
        {
            NewBillDesignComms,
            None
        }
    }
}
