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
            UIColor fgroundColor = MyTNBColor.SilverChalice;
            if (type == AttributedStringType.Hint)
            {
                fgroundColor = MyTNBColor.TunaGrey();
            }
            else if (type == AttributedStringType.Error)
            {
                fgroundColor = MyTNBColor.Tomato;
            }
            string txt = type == AttributedStringType.Title ? str.Translate().ToUpper() : str.Translate();
            UIFont font = type == AttributedStringType.Value ? MyTNBFont.MuseoSans18_300 : MyTNBFont.MuseoSans11_300;
            return new NSAttributedString(txt
                , font: font
                , foregroundColor: fgroundColor
                , strokeWidth: 0);
        }
    }
}
