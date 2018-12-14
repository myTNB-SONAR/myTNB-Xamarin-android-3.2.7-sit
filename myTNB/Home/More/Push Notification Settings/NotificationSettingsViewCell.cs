using System;
using CoreGraphics;
using UIKit;

namespace myTNB.Home.More.PushNotificationSettings
{
    public partial class NotificationSettingsViewCell : UITableViewCell
    {
        public UILabel lblTitle;
        public UIView viewCheckBox;
        public UIView viewLine;
        public UISwitch switchToggle;

        public NotificationSettingsViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = 54;
            Frame = new CGRect(0, 0, cellWidth, cellHeight);

            lblTitle = new UILabel(new CGRect(18, 16, cellWidth - 36, 18));
            lblTitle.TextColor = myTNBColor.TunaGrey();
            lblTitle.Font = myTNBFont.MuseoSans14_300();

            switchToggle = new UISwitch(new CGRect(cellWidth - 18 - 38, 10, 38, 23));
            switchToggle.Transform = CGAffineTransform.MakeScale(0.75f, 0.75f);

            viewLine = new UIView(new CGRect(0, cellHeight - 1, cellWidth, 1));
            viewLine.BackgroundColor = myTNBColor.PlatinumGrey();
            viewLine.Hidden = false;

            AddSubviews(new UIView[] { lblTitle, switchToggle, viewLine });
        }
    }
}