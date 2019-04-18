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
            UIColor fgroundColor = myTNBColor.SilverChalice();
            if (type == AttributedStringType.Hint)
            {
                fgroundColor = myTNBColor.TunaGrey();
            }
            else if (type == AttributedStringType.Error)
            {
                fgroundColor = myTNBColor.Tomato();
            }
            string txt = type == AttributedStringType.Title ? str.Translate().ToUpper() : str.Translate();
            UIFont font = type == AttributedStringType.Value ? myTNBFont.MuseoSans18_300() : myTNBFont.MuseoSans11_300();
            return new NSAttributedString(txt
                , font: font
                , foregroundColor: fgroundColor
                , strokeWidth: 0);
        }
    }
}
