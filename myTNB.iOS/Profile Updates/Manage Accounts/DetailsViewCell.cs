using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class DetailsViewCell : UITableViewCell
    {
        public UILabel lblAccountNumber;
        public UILabel lblAddress;
        public UIView viewLine;

        public DetailsViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = 82;
            Frame = new CGRect(0, 0, cellWidth, cellHeight);

            lblAccountNumber = new UILabel(new CGRect(18, 16, cellWidth - 36, 18))
            {
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans14_500
            };

            lblAddress = new UILabel(new CGRect(18, 34, cellWidth - 36, 40))
            {
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans12_300,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            viewLine = new UIView(new CGRect(0, cellHeight - 1, cellWidth, 1))
            {
                BackgroundColor = MyTNBColor.PlatinumGrey,
                Hidden = false
            };

            AddSubviews(new UIView[] { lblAccountNumber, lblAddress, viewLine });
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }
    }
}