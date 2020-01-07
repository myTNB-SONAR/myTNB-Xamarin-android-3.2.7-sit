using Foundation;
using UIKit;

namespace myTNB
{
    public static class AttributedStringUtility
    {
        public enum AttributedStringType
        {
            Error,
            Hint,
            Title,
            Value
        }

        public static NSAttributedString GetAttributedString(string str, AttributedStringType type)
        {
            str = str ?? string.Empty;
            UIColor fgroundColor = MyTNBColor.SilverChalice;
            if (type == AttributedStringType.Hint)
            {
                fgroundColor = MyTNBColor.TunaGrey();
            }
            else if (type == AttributedStringType.Error)
            {
                fgroundColor = MyTNBColor.Tomato;
            }
            string txt = type == AttributedStringType.Title ? str.ToUpper() : str;
            UIFont font = type == AttributedStringType.Value ? TNBFont.MuseoSans_16_300 : TNBFont.MuseoSans_9_300;
            return new NSAttributedString(txt
                , font: font
                , foregroundColor: fgroundColor
                , strokeWidth: 0);
        }
    }
}