using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class UpdateViewCell : UITableViewCell
    {
        public UILabel lblTitle;
        public UILabel lblDetail;
        public UIView viewCTA;
        public UILabel lblCTA;
        public UIView viewLine;
        public UpdateViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = 62;
            Frame = new CGRect(0, 0, cellWidth, cellHeight);

            lblTitle = new UILabel(new CGRect(18, 16, cellWidth - 36, 12))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans9_300
            };

            lblDetail = new UILabel(new CGRect(18, 28, cellWidth - 96, 18))
            {
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans14_300
            };

            viewCTA = new UIView(new CGRect(cellWidth - 78, 23, 60, 16));
            lblCTA = new UILabel(new CGRect(0, 0, 60, 16))
            {
                TextColor = MyTNBColor.PowerBlue,
                Font = MyTNBFont.MuseoSans12_500,
                TextAlignment = UITextAlignment.Right
            };

            viewCTA.AddSubview(lblCTA);
            viewLine = new UIView(new CGRect(0, cellHeight - 1, cellWidth, 1))
            {
                BackgroundColor = MyTNBColor.PlatinumGrey,
                Hidden = false
            };

            AddSubviews(new UIView[] { lblTitle, lblDetail, viewCTA, viewLine });
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }
    }
}