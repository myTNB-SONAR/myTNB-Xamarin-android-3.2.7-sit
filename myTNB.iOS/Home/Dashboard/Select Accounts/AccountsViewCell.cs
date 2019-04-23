using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class AccountsViewCell : UITableViewCell
    {
        public UILabel lblAccountName;
        public UIImageView imgLeaf;
        public UIView viewLine;
        public AccountsViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = Frame.Height;

            lblAccountName = new UILabel(new CGRect(18, 16, 100, 24));
            lblAccountName.LineBreakMode = UILineBreakMode.TailTruncation;
            lblAccountName.Font = MyTNBFont.MuseoSans16;
            lblAccountName.TextColor = MyTNBColor.TunaGrey();

            imgLeaf = new UIImageView(new CGRect(150, 16, 24, 24));
            imgLeaf.Image = UIImage.FromBundle("IC-RE-Leaf-Green");

            viewLine = new UIView(new CGRect(0, cellHeight - 1, cellWidth, 1));
            viewLine.BackgroundColor = MyTNBColor.PlatinumGrey;
            viewLine.Hidden = false;

            AddSubviews(new UIView[] { lblAccountName, imgLeaf, viewLine });
        }
    }
}