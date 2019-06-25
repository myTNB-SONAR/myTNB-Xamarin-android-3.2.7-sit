using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class NoDataViewCell : UITableViewCell
    {
        public UIImageView imgViewState;
        public UILabel lblDescription;

        public NoDataViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;

            imgViewState = new UIImageView(new CGRect((cellWidth / 2) - 75, 16, 150, 150));

            lblDescription = new UILabel(new CGRect(20, 182, cellWidth - 40, 32))
            {
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Font = MyTNBFont.MuseoSans12,
                TextColor = MyTNBColor.SilverChalice,
                TextAlignment = UITextAlignment.Center
            };

            this.SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);
            this.LayoutMargins = new UIEdgeInsets(0, 0, 0, 0);
            this.SelectionStyle = UITableViewCellSelectionStyle.None;

            AddSubviews(new UIView[] { imgViewState, lblDescription });
        }
    }
}