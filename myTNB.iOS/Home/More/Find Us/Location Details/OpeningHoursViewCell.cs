using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class OpeningHoursViewCell : UITableViewCell
    {
        public UILabel lblTitle;
        public UILabel lblDay;
        public UILabel lblTime;
        public UILabel lbl7EOperation;
        public UIView viewLine;
        public OpeningHoursViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = Frame.Height;

            lblTitle = new UILabel(new CGRect(18, 16, cellWidth - 36, 14))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans9
            };

            lblDay = new UILabel(new CGRect(18, 30, 150, 48))
            {
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans12,
                Lines = 3
            };

            lblTime = new UILabel(new CGRect(168, 30, cellWidth - 186, 36))
            {
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans12,
                TextAlignment = UITextAlignment.Right,
                Lines = 2
            };

            lbl7EOperation = new UILabel(new CGRect(18, 30, cellWidth - 36, 16))
            {
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans12
            };

            viewLine = new UIView(new CGRect(0, cellHeight - 1, cellWidth, 1))
            {
                BackgroundColor = MyTNBColor.PlatinumGrey,
                Hidden = false
            };

            AddSubviews(new UIView[] { lblTitle, lblDay, lblTime, lbl7EOperation, viewLine });
        }
    }
}