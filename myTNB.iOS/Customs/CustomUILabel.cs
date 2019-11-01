using System;
using System.Drawing;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public static class CustomUILabel
    {
        public static CGSize GetLabelSize(UILabel label, nfloat width, nfloat height)
        {
            return label.Text.StringSize(label.Font, new SizeF((float)width, (float)height));
        }

        public static nfloat GetLabelHeight(this UILabel label, nfloat maxHeight)
        {
            CGSize size = label.Text.StringSize(label.Font, new SizeF((float)label.Frame.Width, (float)maxHeight));
            return size.Height;
        }

        public static nfloat GetLabelWidth(this UILabel label, nfloat maxWidth)
        {
            CGSize size = label.Text.StringSize(label.Font, new SizeF((float)maxWidth, (float)label.Frame.Height));
            return size.Width;
        }
    }
}