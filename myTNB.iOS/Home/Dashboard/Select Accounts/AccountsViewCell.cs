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
        private nfloat _imgWidth = ScaleUtility.GetScaledWidth(28);
        public AccountsViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cellHeight = Frame.Height;
            lblAccountName = new UILabel(new CGRect(ScaleUtility.GetScaledWidth(55)
                , ScaleUtility.GetScaledHeight(18), cellWidth - ScaleUtility.GetScaledWidth(106), ScaleUtility.GetScaledHeight(24)))
            {
                LineBreakMode = UILineBreakMode.TailTruncation,
                Font = TNBFont.MuseoSans_16_300,
                TextColor = MyTNBColor.CharcoalGrey
            };

            imgIconView = new UIImageView(new CGRect(ScaleUtility.BaseMarginWidth16, ScaleUtility.BaseMarginWidth16, _imgWidth, _imgWidth));
            viewLine = GenericLine.GetLine(new CGRect(0, ScaleUtility.GetScaledHeight(60), cellWidth, ScaleUtility.GetScaledHeight(1)));
            viewLine.Hidden = false;
            AddSubviews(new UIView[] { lblAccountName, imgIconView, viewLine });
        }

        public string ImageIcon
        {
            set
            {
                if (imgIconView != null && !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
                {
                    imgIconView.Image = UIImage.FromBundle(value);
                }
            }
        }
    }
}