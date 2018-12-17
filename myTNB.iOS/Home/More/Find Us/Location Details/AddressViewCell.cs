using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class AddressViewCell : UITableViewCell
    {
        public UILabel lblTitle;
        public UILabel lblValue;
        public UIView viewDirections;
        public UIView viewLine;
        public AddressViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = Frame.Height;

            lblTitle = new UILabel(new CGRect(18, 16, cellWidth - 36, 14));
            lblTitle.TextColor = myTNBColor.SilverChalice();
            lblTitle.Font = myTNBFont.MuseoSans9();

            lblValue = new UILabel(new CGRect(18, 30, cellWidth - 78, 60));
            lblValue.TextColor = myTNBColor.TunaGrey();
            lblValue.Font = myTNBFont.MuseoSans12();
            lblValue.Lines = 0;
            lblValue.LineBreakMode = UILineBreakMode.WordWrap;

            viewDirections = new UIView(new CGRect(cellWidth - 42, 35, 24, 24));
            UIImageView imgDirections = new UIImageView(new CGRect(0, 0, 24, 24));
            imgDirections.Image = UIImage.FromBundle("Direction");

            viewDirections.AddSubview(imgDirections);
            viewLine = new UIView(new CGRect(0, cellHeight - 1, cellWidth, 1));
            viewLine.BackgroundColor = myTNBColor.PlatinumGrey();
            viewLine.Hidden = false;

            AddSubviews(new UIView[] { lblTitle, lblValue, viewDirections, viewLine });
        }
    }
}