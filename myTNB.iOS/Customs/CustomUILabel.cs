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
            if (label != null)
            {
                if (label.Text != null)
                {
                    return label.Text.StringSize(label.Font, new SizeF((float)width, (float)height));
                }
                return label.Frame.Size;
            }
            return new CGSize();
        }

        public static nfloat GetLabelHeight(this UILabel label, nfloat maxHeight)
        {
            if (label != null)
            {
                if (label.Text != null)
                {
                    CGSize size = label.Text.StringSize(label.Font, new SizeF((float)label.Frame.Width, (float)maxHeight));
                    return size.Height;
                }
                return label.Frame.Height;
            }
            return 0;
        }

        public static nfloat GetLabelWidth(this UILabel label, nfloat maxWidth)
        {
            if (label != null)
            {
                if (label.Text != null)
                {
                    CGSize size = label.Text.StringSize(label.Font, new SizeF((float)maxWidth, (float)label.Frame.Height));
                    return size.Width;
                }
                return label.Frame.Width;
            }
            return 0;
        }
    }
}