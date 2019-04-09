using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class ManageCardViewCell : UITableViewCell
    {
        public UIImageView imgViewCC;
        public UILabel lblCardNo;
        UIImageView imgViewX;

        public ManageCardViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = 58;
            Frame = new CGRect(0, 0, cellWidth, cellHeight);

            imgViewCC = new UIImageView(new CGRect(34, 16, 24, 24));

            lblCardNo = new UILabel(new CGRect(65, 19, cellWidth - 130, 18));
            lblCardNo.TextColor = myTNBColor.TunaGrey();
            lblCardNo.Font = myTNBFont.MuseoSans14_300();

            imgViewX = new UIImageView(new CGRect(cellWidth - 58, 16, 24, 24));
            imgViewX.Image = UIImage.FromBundle("IC-Action-Delete");

            AddSubviews(new UIView[] { imgViewCC, lblCardNo, imgViewX });

        }
    }
}