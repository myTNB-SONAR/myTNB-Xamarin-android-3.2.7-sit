using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class LocationNameViewCell : UITableViewCell
    {
        public UILabel lblName;
        public UIView viewLine;
        public LocationNameViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = Frame.Height;

            lblName = new UILabel(new CGRect(18, 16, cellWidth - 36, 40));
            lblName.TextColor = MyTNBColor.PowerBlue;
            lblName.Font = MyTNBFont.MuseoSans16;
            lblName.Lines = 0;
            lblName.LineBreakMode = UILineBreakMode.WordWrap;

            viewLine = new UIView(new CGRect(0, cellHeight - 1, cellWidth, 1));
            viewLine.BackgroundColor = MyTNBColor.PlatinumGrey;
            viewLine.Hidden = false;

            AddSubviews(new UIView[] { lblName, viewLine });
        }
    }
}