using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public static class CustomUIButton
    {
        public static UIButton GetUIButton(CGRect frame, string key)
        {
            UIButton customButton = new UIButton(UIButtonType.Custom);
            customButton.Frame = frame;
            customButton.SetTitle(key.Translate(), UIControlState.Normal);
            customButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            customButton.TitleLabel.Font = MyTNBFont.MuseoSans16;
            customButton.BackgroundColor = MyTNBColor.SilverChalice;
            customButton.Layer.CornerRadius = 4.0f;
            return customButton;
        }

        public static void MakeTopCornerRadius(UIButton button)
        {
            if (button == null)
            {
                return;
            }
            button.Layer.CornerRadius = 0.0F;
            var maskPath = UIBezierPath.FromRoundedRect(button.Bounds
                , UIRectCorner.TopLeft | UIRectCorner.TopRight, new CGSize(10.0, 10.0));
            var maskLayer = new CAShapeLayer
            {
                Frame = button.Bounds,
                Path = maskPath.CGPath
            };
            button.Layer.Mask = maskLayer;
        }
    }
}
