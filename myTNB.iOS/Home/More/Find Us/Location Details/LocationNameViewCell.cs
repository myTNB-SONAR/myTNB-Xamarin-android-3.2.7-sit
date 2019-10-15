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

            lblName = new UILabel(new CGRect(18, 16, cellWidth - 36, 40))
            {
                TextColor = MyTNBColor.PowerBlue,
                Font = MyTNBFont.MuseoSans16,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            viewLine = new UIView(new CGRect(0, cellHeight - 1, cellWidth, 1))
            {
                BackgroundColor = MyTNBColor.PlatinumGrey,
                Hidden = false
            };

            AddSubviews(new UIView[] { lblName, viewLine });
        }
    }
}