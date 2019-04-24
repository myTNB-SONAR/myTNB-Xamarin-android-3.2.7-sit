using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class PromotionsViewCell : UITableViewCell
    {
        public UIView viewBanner;
        public UIView viewContainer;
        public UIImageView imgPromotionIcon;
        public UIImageView imgBanner;
        public UIImageView imgUnread;
        public UILabel lblTitle;
        public UILabel lblDetails;
        public UILabel lblDate;

        public PromotionsViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            viewBanner = new UIView(new CGRect(0, 0, cellWidth, cellWidth / 1.777));
            imgBanner = new UIImageView(new CGRect(0, 0, cellWidth, cellWidth / 1.777));
            //imgBanner.BackgroundColor = UIColor.Gray;
            viewBanner.AddSubview(imgBanner);

            viewContainer = new UIView(new CGRect(0, imgBanner.Frame.Height, cellWidth, 64));
            imgPromotionIcon = new UIImageView(new CGRect(18, 16, 24, 24));
            imgPromotionIcon.Image = UIImage.FromBundle("Notification-Promo");

            lblTitle = new UILabel(new CGRect(44, 16, cellWidth - (110 + 16 + 44), 18));
            lblTitle.TextColor = MyTNBColor.TunaGrey();
            lblTitle.Font = MyTNBFont.MuseoSans14;
            lblTitle.TextAlignment = UITextAlignment.Left;

            lblDetails = new UILabel(new CGRect(44, 34, 180, 14));
            lblDetails.TextColor = MyTNBColor.SilverChalice;
            lblDetails.Font = MyTNBFont.MuseoSans9_300;
            lblDetails.TextAlignment = UITextAlignment.Left;

            lblDate = new UILabel(new CGRect(cellWidth - 110, 16, 70, 14));
            lblDate.TextColor = MyTNBColor.SilverChalice;
            lblDate.Font = MyTNBFont.MuseoSans9_300;
            lblDate.TextAlignment = UITextAlignment.Right;

            imgUnread = new UIImageView(new CGRect(cellWidth - 34, 16, 16, 16));
            imgUnread.Image = UIImage.FromBundle("Notification-Unread");

            viewContainer.AddSubviews(new UIView[] { imgPromotionIcon, lblTitle, lblDetails, lblDate, imgUnread });
            AddSubviews(new UIView[] { viewBanner, viewContainer });
        }
    }
}