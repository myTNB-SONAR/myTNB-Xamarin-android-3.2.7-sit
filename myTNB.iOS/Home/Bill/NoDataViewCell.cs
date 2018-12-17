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
            nfloat cellHeight = Frame.Height;

            imgViewState = new UIImageView(new CGRect((cellWidth / 2) - 75, 16, 150, 150));

            lblDescription = new UILabel(new CGRect(20, 182, cellWidth - 40, 32));
            lblDescription.Lines = 0;
            lblDescription.LineBreakMode = UILineBreakMode.WordWrap;
            lblDescription.Font = myTNBFont.MuseoSans12();
            lblDescription.TextColor = myTNBColor.SilverChalice();
            lblDescription.TextAlignment = UITextAlignment.Center;

            this.SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);
            this.LayoutMargins = new UIEdgeInsets(0, 0, 0, 0);
            this.SelectionStyle = UITableViewCellSelectionStyle.None;

            AddSubviews(new UIView[] { imgViewState, lblDescription });
        }
    }
}