using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class RemoveViewCell : UITableViewCell
    {
        public UIButton btnRemove;
        public RemoveViewCell (IntPtr handle) : base (handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = 88;
            Frame = new CGRect(0, 0, cellWidth, cellHeight);

            btnRemove = new UIButton(UIButtonType.Custom);
            btnRemove.Frame = new CGRect(18, 16, cellWidth - 36, 48);
            btnRemove.Layer.CornerRadius = 4;
            btnRemove.Layer.BorderColor = myTNBColor.FreshGreen().CGColor;
            btnRemove.BackgroundColor = UIColor.White;
            btnRemove.Layer.BorderWidth = 1;
            btnRemove.SetTitle("Remove this account", UIControlState.Normal);
            btnRemove.Font = myTNBFont.MuseoSans16();
            btnRemove.SetTitleColor(myTNBColor.FreshGreen(), UIControlState.Normal);
            AddSubview(btnRemove);
        }
    }
}