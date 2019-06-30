using System;
using System.Drawing;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public static class CustomUILabel
    {
        public static UILabel GetUILabelField(CGRect lblFrame, string key
            , UITextAlignment txtAlignment = UITextAlignment.Left)
        {
            return new UILabel(lblFrame)
            {
                Text = key.Translate(),
                TextAlignment = txtAlignment,
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans12
            };
        }

        public static UILabel GetUILabelField(CGRect lblFrame, string key, UIFont font
            , UIColor textColor, UITextAlignment txtAlignment = UITextAlignment.Left)
        {
            UILabel customLabel = GetUILabelField(lblFrame, key, txtAlignment);
            customLabel.Font = font;
            customLabel.TextColor = textColor;
            return customLabel;
        }

        public static CGSize GetLabelSize(UILabel label, nfloat width, nfloat height)
        {
            return label.Text.StringSize(label.Font, new SizeF((float)width, (float)height));
        }
    }
}