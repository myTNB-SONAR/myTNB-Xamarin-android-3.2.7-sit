using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class AccountDetailsViewCell : UITableViewCell
    {
        public UILabel lblTitle;
        public UILabel lblDetail;
        public UIView viewCTA;
        public UILabel lblCTA;
        public UIView viewLine;
        public AccountDetailsViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = 64;
            Frame = new CGRect(0, 0, cellWidth, cellHeight);

            lblTitle = new UILabel(new CGRect(18, 16, cellWidth - 36, 12));
            lblTitle.TextColor = MyTNBColor.SilverChalice;
            lblTitle.Font = MyTNBFont.MuseoSans9_300;

            lblDetail = new UILabel(new CGRect(18, 28, cellWidth - 36, 18));
            lblDetail.TextColor = MyTNBColor.SilverChalice;
            lblDetail.Font = MyTNBFont.MuseoSans14_300;

            viewCTA = new UIView(new CGRect(cellWidth - 78, 23, 60, 16));
            lblCTA = new UILabel(new CGRect(0, 0, 60, 16));
            lblCTA.TextColor = MyTNBColor.PowerBlue;
            lblCTA.Font = MyTNBFont.MuseoSans12;
            lblCTA.TextAlignment = UITextAlignment.Right;

            viewCTA.AddSubview(lblCTA);
            viewLine = new UIView(new CGRect(0, cellHeight - 1, cellWidth, 1));
            viewLine.BackgroundColor = MyTNBColor.PlatinumGrey;
            viewLine.Hidden = false;

            AddSubviews(new UIView[] { lblTitle, lblDetail, viewCTA, viewLine });
        }
    }
}