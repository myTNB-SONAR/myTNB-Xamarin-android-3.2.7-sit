using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public partial class BillTableViewCell : UITableViewCell
    {
        public UILabel lblDate;
        public UILabel lblTitle;
        public UILabel lblDetails;
        public UILabel lblAmount;
        public UIImageView imgArrow;
        public UIView viewLine;

        public BillTableViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = Frame.Height;

            lblDate = new UILabel(new CGRect(DeviceHelper.GetScaledSizeByWidth(5.6F), 17, 50, 16));
            lblDate.TextAlignment = UITextAlignment.Left;
            lblDate.Font = myTNBFont.MuseoSans14_500();
            lblDate.TextColor = myTNBColor.TunaGrey();

            lblTitle = new UILabel(new CGRect(DeviceHelper.GetScaledSizeByWidth(25F), 17, 115, 16));
            lblTitle.TextAlignment = UITextAlignment.Left;
            lblTitle.Font = myTNBFont.MuseoSans14_500();
            lblTitle.TextColor = myTNBColor.TunaGrey();

            lblDetails = new UILabel(new CGRect(DeviceHelper.GetScaledSizeByWidth(25F), 33, 115, 14));
            lblDetails.TextAlignment = UITextAlignment.Left;
            lblDetails.Font = myTNBFont.MuseoSans11_300();
            lblDetails.TextColor = myTNBColor.SilverChalice();

            lblAmount = new UILabel(new CGRect(DeviceHelper.GetScaledSizeByWidth(62F), 17, 100, 16));
            lblAmount.TextAlignment = UITextAlignment.Left;
            lblAmount.Font = myTNBFont.MuseoSans14_500();
            lblAmount.TextColor = myTNBColor.TunaGrey();

            imgArrow = new UIImageView(new CGRect(DeviceHelper.GetScaledSizeByWidth(92F), 23, 16, 16));
            imgArrow.Image = UIImage.FromBundle("Arrow-Right-Black");
            viewLine = new UIView(new CGRect(0, cellHeight - 1, cellWidth, 1));
            viewLine.BackgroundColor = myTNBColor.PlatinumGrey();

            AddSubviews(new UIView[]{lblDate, lblTitle, lblDetails
                , lblAmount, imgArrow, viewLine});
        }

        public void AdjustYlocation(bool isBill)
        {
            nfloat y = isBill ? 23 : 17;
            lblDate.Frame = new CGRect(DeviceHelper.GetScaledSizeByWidth(5.6F), y, 50, 16);
            lblTitle.Frame = new CGRect(DeviceHelper.GetScaledSizeByWidth(25F), y, 115, 16);
            lblAmount.Frame = new CGRect(DeviceHelper.GetScaledSizeByWidth(62F), y, 100, 16);
        }
    }
}