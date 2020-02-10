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
            if (label != null && label.Text.IsValid())
            {
                return label.Text.StringSize(label.Font, new SizeF((float)width, (float)height));
            }
            else if (label != null)
            {
                return label.Frame.Size;
            }
            else
            {
                return new CGSize();
            }
        }

        public static nfloat GetLabelHeight(this UILabel label, nfloat maxHeight)
        {
            if (label != null && label.Text.IsValid())
            {
                CGSize size = label.Text.StringSize(label.Font, new SizeF((float)label.Frame.Width, (float)maxHeight));
                return size.Height;
            }
            else if (label != null)
            {
                return label.Frame.Height;
            }
            else
            {
                return 0;
            }
        }

        public static nfloat GetLabelWidth(this UILabel label, nfloat maxWidth)
        {
            if (label != null && label.Text.IsValid())
            {
                CGSize size = label.Text.StringSize(label.Font, new SizeF((float)maxWidth, (float)label.Frame.Height));
                return size.Width;
            }
            else if (label != null)
            {
                return label.Frame.Width;
            }
            else
            {
                return 0;
            }
        }
    }
}