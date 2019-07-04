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
        public UIView viewCheckBox;
        public UIImageView imgCheckbox;

        public NotificationViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = 66;
            Frame = new CGRect(0, 0, cellWidth, cellHeight);

            viewCheckBox = new UIView(new CGRect(cellWidth - 40, 20, 24, 24));
            imgCheckbox = new UIImageView(new CGRect(0, 0, 24, 24))
            {
                Image = UIImage.FromBundle("Payment-Checkbox-Inactive")
            };
            viewCheckBox.AddSubview(imgCheckbox);

            imgIcon = new UIImageView(new CGRect(18, 17, 24, 24));

            lblTitle = new UILabel(new CGRect(48, 16, cellWidth - 96 - 60, 20));
            lblTitle.TextColor = MyTNBColor.TunaGrey();
            lblTitle.Font = MyTNBFont.MuseoSans14_500;

            lblDate = new UILabel(new CGRect(cellWidth - 94, (cellHeight - 14) / 2, 60, 14))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans10_300,
                TextAlignment = UITextAlignment.Right
            };

            imgUnread = new UIImageView(new CGRect(cellWidth - 34, (cellHeight - 16) / 2, 16, 16))
            {
                Image = UIImage.FromBundle("Notification-Unread"),
                Hidden = true
            };

            lblDetails = new UILabel(new CGRect(48, 40, cellWidth - (48 + lblDate.Frame.Width + 34 + 12), 14))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans10_300,
                Lines = 1,
                LineBreakMode = UILineBreakMode.TailTruncation
            };

            UIView viewLine = new UIView(new CGRect(0, cellHeight - 1, cellWidth, 1))
            {
                BackgroundColor = MyTNBColor.PlatinumGrey
            };

            AddSubviews(new UIView[] { viewCheckBox, imgIcon, lblTitle, lblDetails, lblDate, imgUnread, viewLine });
        }

        public NotificationViewCell(string cellId) : base(UITableViewCellStyle.Default, cellId)
        {

        }

        public void UpdateCell(bool isSelectionMode)
        {
            UpdateStyle(isSelectionMode);
        }

        private void UpdateStyle(bool isSelectionMode)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat xDate = isSelectionMode ? cellWidth - 134 : cellWidth - 94;
            nfloat xRead = isSelectionMode ? cellWidth - 64 : cellWidth - 34;

            imgCheckbox.Hidden = !isSelectionMode;

            CGRect dateFrame = lblDate.Frame;
            dateFrame.X = xDate;
            lblDate.Frame = dateFrame;

            CGRect readFrame = imgUnread.Frame;
            readFrame.X = xRead;
            imgUnread.Frame = readFrame;

            if (isSelectionMode) {
                lblDetails.Frame = new CGRect(lblDetails.Frame.X, lblDetails.Frame.Y
                    , cellWidth - (48 + lblDate.Frame.Width + 34 + 12 + 40), lblDetails.Frame.Height);   
            }
            else
            {
                lblDetails.Frame = new CGRect(lblDetails.Frame.X, lblDetails.Frame.Y
                    , cellWidth - (48 + lblDate.Frame.Width + 34 + 12), lblDetails.Frame.Height);
            }
        }
    }
}