using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class ContactUsViewCell : UITableViewCell
    {
        public UILabel lblTitle;
        public UILabel lblValue;
        public UIView viewCall;
        public UIView viewLine;
        public ContactUsViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = Frame.Height;

            lblTitle = new UILabel(new CGRect(18, 16, cellWidth - 36, 14));
            lblTitle.TextColor = MyTNBColor.SilverChalice;
            lblTitle.Font = MyTNBFont.MuseoSans9;

            lblValue = new UILabel(new CGRect(18, 30, cellWidth - 78, 16));
            lblValue.TextColor = MyTNBColor.TunaGrey();
            lblValue.Font = MyTNBFont.MuseoSans12;

            viewCall = new UIView(new CGRect(cellWidth - 42, 19, 24, 24));
            UIImageView imgCall = new UIImageView(new CGRect(0, 0, 24, 24));
            imgCall.Image = UIImage.FromBundle("Call");
            viewCall.AddSubview(imgCall);

            viewLine = new UIView(new CGRect(0, cellHeight - 1, cellWidth, 1));
            viewLine.BackgroundColor = MyTNBColor.PlatinumGrey;
            viewLine.Hidden = false;

            AddSubviews(new UIView[] { lblTitle, lblValue, viewCall, viewLine });
        }
    }
}