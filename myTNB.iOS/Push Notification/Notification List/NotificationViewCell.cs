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

        private nfloat _unreadWidth;

        public NotificationViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = ScaleUtility.GetScaledHeight(70);
            nfloat icnWidth = ScaleUtility.GetWidthByScreenSize(24);
            nfloat chkWidth = ScaleUtility.GetWidthByScreenSize(20);
            _unreadWidth = ScaleUtility.GetWidthByScreenSize(16);

            UIView _view = new UIView(new CGRect(0, 0, cellWidth, cellHeight))
            {
                BackgroundColor = UIColor.Clear
            };

            viewCheckBox = new UIView(new CGRect(cellWidth - chkWidth - ScaleUtility.GetScaledWidth(16)
                , (cellHeight - chkWidth) / 2, chkWidth, chkWidth));
            imgCheckbox = new UIImageView(new CGRect(0, 0, chkWidth, chkWidth))
            {
                Image = UIImage.FromBundle("Payment-Checkbox-Inactive")
            };
            viewCheckBox.AddSubview(imgCheckbox);

            imgIcon = new UIImageView(new CGRect(ScaleUtility.GetScaledWidth(16)
                , (cellHeight - icnWidth) / 2, icnWidth, icnWidth));

            lblTitle = new UILabel(new CGRect(imgIcon.Frame.GetMaxX() + ScaleUtility.GetScaledWidth(8), ScaleUtility.GetScaledHeight(16)
                , ScaleUtility.GetPercentWidthValue(50F), ScaleUtility.GetScaledHeight(20)))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_14_500
            };

            lblDate = new UILabel(new CGRect(cellWidth - ScaleUtility.GetPercentWidthValue(20F) - ScaleUtility.GetScaledWidth(36)
                , (cellHeight - ScaleUtility.GetScaledHeight(14)) / 2
                , ScaleUtility.GetPercentWidthValue(20F), ScaleUtility.GetScaledHeight(14)))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = TNBFont.MuseoSans_10_300,
                TextAlignment = UITextAlignment.Right
            };

            imgUnread = new UIImageView(new CGRect(cellWidth - _unreadWidth - ScaleUtility.GetWidthByScreenSize(16)
                , (cellHeight - _unreadWidth) / 2, _unreadWidth, _unreadWidth))
            {
                Image = UIImage.FromBundle("Notification-Unread"),
                Hidden = true
            };

            lblDetails = new UILabel(new CGRect(imgIcon.Frame.GetMaxX() + ScaleUtility.GetScaledWidth(8)
                , ScaleUtility.GetYLocationFromFrame(lblTitle.Frame, 4), ScaleUtility.GetPercentWidthValue(50F), ScaleUtility.GetScaledHeight(14)))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = TNBFont.MuseoSans_10_300,
                Lines = 1,
                LineBreakMode = UILineBreakMode.TailTruncation
            };

            UIView viewLine = new UIView(new CGRect(0, cellHeight - ScaleUtility.GetScaledHeight(1), cellWidth, ScaleUtility.GetScaledHeight(1)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };

            _view.AddSubviews(new UIView[] { viewCheckBox, imgIcon, lblTitle, lblDetails, lblDate, imgUnread, viewLine });
            AddSubviews(_view);
            if (_view != null)
            {
                _view.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
                _view.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
                _view.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
                _view.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            }
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public NotificationViewCell(string cellId) : base(UITableViewCellStyle.Default, cellId)
        {

        }

        public void UpdateCell(bool isSelectionMode, bool isRead)
        {
            UpdateStyle(isSelectionMode, isRead);
        }

        private void UpdateStyle(bool isSelectionMode, bool isRead)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat margin = isRead ? ScaleUtility.GetScaledWidth(16) : ScaleUtility.GetScaledWidth(36);
            nfloat xDate = cellWidth - ScaleUtility.GetPercentWidthValue(20) - (isSelectionMode ? ScaleUtility.GetScaledWidth(64) : margin);
            nfloat xRead = cellWidth - _unreadWidth - (isSelectionMode ? ScaleUtility.GetScaledWidth(44) : ScaleUtility.GetScaledWidth(16));

            imgCheckbox.Hidden = !isSelectionMode;

            CGRect dateFrame = lblDate.Frame;
            dateFrame.X = xDate;
            lblDate.Frame = dateFrame;

            CGRect readFrame = imgUnread.Frame;
            readFrame.X = xRead;
            imgUnread.Frame = readFrame;

            if (isSelectionMode)
            {
                lblDetails.Frame = new CGRect(lblDetails.Frame.X, lblDetails.Frame.Y
                    , cellWidth - (48 + lblDate.Frame.Width + 34 + 12 + 40), lblDetails.Frame.Height);
            }
            else
            {
                lblDetails.Frame = new CGRect(lblDetails.Frame.X, lblDetails.Frame.Y
                    , cellWidth - (48 + lblDate.Frame.Width + 34 + 12), lblDetails.Frame.Height);
            }
        }

        public bool IsRead
        {
            set
            {
                imgUnread.Hidden = value;
            }
        }
    }
}