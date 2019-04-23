using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class RemoveViewCell : UITableViewCell
    {
        public UIButton btnRemove;
        public RemoveViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = DeviceHelper.GetScaledHeight(88);
            Frame = new CGRect(0, 0, cellWidth, cellHeight);

            btnRemove = new UIButton(UIButtonType.Custom);
            btnRemove.Frame = new CGRect(18, 0, cellWidth - 36, DeviceHelper.GetScaledHeight(48));
            btnRemove.Layer.CornerRadius = 4;
            btnRemove.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            btnRemove.BackgroundColor = UIColor.White;
            btnRemove.Layer.BorderWidth = 1;
            btnRemove.SetTitle("Manage_RemoveAccount".Translate(), UIControlState.Normal);
            btnRemove.Font = MyTNBFont.MuseoSans16_500;
            btnRemove.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            AddSubview(btnRemove);
        }
    }
}