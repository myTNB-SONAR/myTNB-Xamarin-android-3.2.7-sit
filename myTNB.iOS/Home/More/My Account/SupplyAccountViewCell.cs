using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class SupplyAccountViewCell : UITableViewCell
    {
        public UILabel lblName;
        public UILabel lblAccountNumber;
        //public UILabel lblUsers;
        public UIView viewCTA;
        public UILabel lblCTA;
        public UIView viewLine;
        public UIImageView imgLeaf;
        public SupplyAccountViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            //nfloat cellHeight = 80;
            //Frame = new CGRect(0, 0, cellWidth, cellHeight);

            lblName = new UILabel(new CGRect(18, 16, cellWidth - 126, 18));
            lblName.TextColor = MyTNBColor.TunaGrey();
            lblName.Font = MyTNBFont.MuseoSans14;

            imgLeaf = new UIImageView(new CGRect(lblName.Frame.X + lblName.Frame.Width, 16, 24, 24));
            imgLeaf.Image = UIImage.FromBundle("IC-RE-Leaf-Green");
            imgLeaf.Hidden = true;

            lblAccountNumber = new UILabel(new CGRect(18, 34, cellWidth - 36, 16));
            lblAccountNumber.TextColor = MyTNBColor.TunaGrey();
            lblAccountNumber.Font = MyTNBFont.MuseoSans12_300;

            /*lblUsers = new UILabel(new CGRect(18, 50, cellWidth - 36, 12));
            lblUsers.TextColor = myTNBColor.SilverChalice;
            lblUsers.Font = myTNBFont.MuseoSans9;
            lblUsers.Hidden = true;
            */

            viewCTA = new UIView(new CGRect(cellWidth - 78, 25, 60, 16));
            lblCTA = new UILabel(new CGRect(0, 0, 60, 16));
            lblCTA.TextColor = MyTNBColor.PowerBlue;
            lblCTA.Font = MyTNBFont.MuseoSans12;
            lblCTA.TextAlignment = UITextAlignment.Right;

            viewCTA.AddSubview(lblCTA);

            viewLine = new UIView(new CGRect(0, Frame.Height - 1, cellWidth, 1));
            viewLine.BackgroundColor = MyTNBColor.PlatinumGrey;
            viewLine.Hidden = false;

            AddSubviews(new UIView[] { lblName, imgLeaf, lblAccountNumber, viewCTA, viewLine });
        }
    }
}