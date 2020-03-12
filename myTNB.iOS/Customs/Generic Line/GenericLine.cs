using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public static class GenericLine
    {
        public static UIView GetLine(CGRect frame)
        {
            return new UIView(frame)
            {
                BackgroundColor = MyTNBColor.PlatinumGrey
            };
        }
    }
}