using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class AccountsViewCell : UITableViewCell
    {
        public UILabel lblAccountName;
        public UIImageView imgIconView;
        public UIView viewLine;
        public AccountsViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = Frame.Height;

            lblAccountName = new UILabel(new CGRect(51, (cellHeight - 24) / 2, cellWidth - 102, 24))
            {
                LineBreakMode = UILineBreakMode.TailTruncation,
                Font = MyTNBFont.MuseoSans16_300,
                TextColor = MyTNBColor.TunaGrey()
            };

            imgIconView = new UIImageView(new CGRect(16, (cellHeight - 24) / 2, 24, 24))
            {
                Image = UIImage.FromBundle("IC-RE-Leaf-Green")
            };

            viewLine = GenericLine.GetLine(new CGRect(0, cellHeight - 1, cellWidth, 1));
            viewLine.Hidden = false;

            AddSubviews(new UIView[] { lblAccountName, imgIconView, viewLine });
        }

        public string ImageIcon
        {
            set
            {
                if (imgIconView!= null && !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
                {
                    imgIconView.Image = UIImage.FromBundle(value);
                }
            }
        }
    }
}