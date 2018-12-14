using System;
using CoreGraphics;
using UIKit;

namespace myTNB.PushNotification
{
    public partial class NotificationViewCell : UITableViewCell
    {
        public UIImageView imgIcon;
        public UILabel lblTitle;
        public UILabel lblDetails;
        public UILabel lblDate;
        public UIImageView imgUnread;

        public NotificationViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = 66;
            Frame = new CGRect(0, 0, cellWidth, cellHeight);

            imgIcon = new UIImageView(new CGRect(18, 17, 24, 24));

            lblTitle = new UILabel(new CGRect(48, 17, cellWidth - 96 - 60, 18));
            lblTitle.TextColor = myTNBColor.TunaGrey();
            lblTitle.Font = myTNBFont.MuseoSans14();

            lblDetails = new UILabel(new CGRect(48, 34, cellWidth - 96, 14));
            lblDetails.TextColor = myTNBColor.SilverChalice();
            lblDetails.Font = myTNBFont.MuseoSans9();
            lblDetails.Lines = 1;
            lblDetails.LineBreakMode = UILineBreakMode.TailTruncation;

            lblDate = new UILabel(new CGRect(cellWidth - 94, 17, 60, 14));
            lblDate.TextColor = myTNBColor.SilverChalice();
            lblDate.Font = myTNBFont.MuseoSans9();
            lblDate.TextAlignment = UITextAlignment.Right;

            imgUnread = new UIImageView(new CGRect(cellWidth - 34, 16, 16, 16));
            imgUnread.Image = UIImage.FromBundle("Notification-Unread");
            imgUnread.Hidden = true;

            UIView viewLine = new UIView(new CGRect(0, cellHeight - 1, cellWidth, 1));
            viewLine.BackgroundColor = myTNBColor.PlatinumGrey();

            AddSubviews(new UIView[] { imgIcon, lblTitle, lblDetails, lblDate, imgUnread, viewLine });
        }
    }
}