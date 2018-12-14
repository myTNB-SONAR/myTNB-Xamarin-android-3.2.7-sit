using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class ServicesViewCell : UITableViewCell
    {
        public UILabel lblTitle;
        public UILabel lblValue;
        public ServicesViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = Frame.Height;

            lblTitle = new UILabel(new CGRect(18, 16, cellWidth - 36, 14));
            lblTitle.TextColor = myTNBColor.SilverChalice();
            lblTitle.Font = myTNBFont.MuseoSans9();

            lblValue = new UILabel(new CGRect(18, 30, cellWidth - 36, 200));
            lblValue.TextColor = myTNBColor.TunaGrey();
            lblValue.Font = myTNBFont.MuseoSans12();
            lblValue.Lines = 12;
            lblValue.LineBreakMode = UILineBreakMode.WordWrap;

            AddSubviews(new UIView[] { lblTitle, lblValue });
        }
    }
}