using CoreGraphics;
using System;
using UIKit;

namespace myTNB
{
    public partial class FeedbackDetailsViewCell : UITableViewCell
    {
        public UILabel lblTitle;
        public UILabel lblValue;

        public FeedbackDetailsViewCell(IntPtr handle) : base(handle)
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

            AddSubviews(new UIView[] { lblTitle, lblValue });
        }
    }
}