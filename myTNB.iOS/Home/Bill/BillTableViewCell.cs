using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public partial class BillTableViewCell : UITableViewCell
    {
        public UILabel lblDate, lblTitle, lblDetails, lblAmount;
        public UIImageView imgArrow;
        public UIView viewLine;

        public BillTableViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = Frame.Height;

            lblDate = new UILabel(new CGRect(DeviceHelper.GetScaledSizeByWidth(5.6F), 17, 50, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.TunaGrey()
            };

            lblTitle = new UILabel(new CGRect(DeviceHelper.GetScaledSizeByWidth(25F), 17, 115, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.TunaGrey()
            };

            lblDetails = new UILabel(new CGRect(DeviceHelper.GetScaledSizeByWidth(25F), 33, 115, 14))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans11_300,
                TextColor = MyTNBColor.SilverChalice
            };

            lblAmount = new UILabel(new CGRect(DeviceHelper.GetScaledSizeByWidth(62F), 17, 100, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.TunaGrey()
            };

            imgArrow = new UIImageView(new CGRect(DeviceHelper.GetScaledSizeByWidth(92F), 23, 16, 16))
            {
                Image = UIImage.FromBundle("Arrow-Right-Black")
            };
            viewLine = GenericLine.GetLine(new CGRect(0, cellHeight - 1, cellWidth, 1));

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