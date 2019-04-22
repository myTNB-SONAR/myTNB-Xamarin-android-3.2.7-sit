using CoreGraphics;
using System;
using UIKit;

namespace myTNB
{
    public partial class FeedbackDetailsViewImageCell : UITableViewCell
    {
        public UILabel lblTitle;
        public UILabel lblValue;
        public UIScrollView imgScrollView;

        public FeedbackDetailsViewImageCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;

            lblTitle = new UILabel(new CGRect(18, 16, cellWidth - 36, 14))
            {
                Font = myTNBFont.MuseoSans9(),
                TextColor = myTNBColor.SilverChalice()
            };

            lblValue = new UILabel(new CGRect(18, 30, cellWidth - 36, 18))
            {
                Font = myTNBFont.MuseoSans14(),
                TextColor = myTNBColor.TunaGrey(),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            imgScrollView = new UIScrollView(new CGRect(18, 31, cellWidth - 36, 94));

            AddSubviews(new UIView[] { lblTitle, lblValue, imgScrollView });
        }
    }
}