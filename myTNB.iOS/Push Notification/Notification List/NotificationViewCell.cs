﻿using System;
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

            viewCheckBox = new UIView(new CGRect(10, 20, 24, 24));
            imgCheckbox = new UIImageView(new CGRect(0, 0, 24, 24));
            imgCheckbox.Image = UIImage.FromBundle("Payment-Checkbox-Inactive");
            viewCheckBox.AddSubview(imgCheckbox);

            imgIcon = new UIImageView(new CGRect(18, 17, 24, 24));

            lblTitle = new UILabel(new CGRect(48, 17, cellWidth - 96 - 60, 18));
            lblTitle.TextColor = MyTNBColor.TunaGrey();
            lblTitle.Font = MyTNBFont.MuseoSans14;

            lblDetails = new UILabel(new CGRect(48, 34, cellWidth - 96, 14));
            lblDetails.TextColor = MyTNBColor.SilverChalice;
            lblDetails.Font = MyTNBFont.MuseoSans9;
            lblDetails.Lines = 1;
            lblDetails.LineBreakMode = UILineBreakMode.TailTruncation;

            lblDate = new UILabel(new CGRect(cellWidth - 94, 17, 60, 14));
            lblDate.TextColor = MyTNBColor.SilverChalice;
            lblDate.Font = MyTNBFont.MuseoSans9;
            lblDate.TextAlignment = UITextAlignment.Right;

            imgUnread = new UIImageView(new CGRect(cellWidth - 34, 16, 16, 16));
            imgUnread.Image = UIImage.FromBundle("Notification-Unread");
            imgUnread.Hidden = true;

            UIView viewLine = new UIView(new CGRect(0, cellHeight - 1, cellWidth, 1));
            viewLine.BackgroundColor = MyTNBColor.PlatinumGrey;

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
            var xValueImg = isSelectionMode ? 41 : 18;
            var xValueTitle = isSelectionMode ? 71 : 48;

            imgCheckbox.Hidden = !isSelectionMode;

            CGRect imgframe = imgIcon.Frame;
            imgframe.X = xValueImg;
            imgIcon.Frame = imgframe;

            CGRect titleframe = lblTitle.Frame;
            titleframe.X = xValueTitle;
            lblTitle.Frame = titleframe;

            CGRect descframe = lblDetails.Frame;
            descframe.X = xValueTitle;
            lblDetails.Frame = descframe;
        }
    }
}