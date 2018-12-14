using System;
using UIKit;

namespace myTNB
{
    public partial class FAQViewCell : UITableViewCell
    {
        public UITextView txtViewAnswer;
        public FAQViewCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;

            txtViewAnswer = new UITextView();
            txtViewAnswer.Editable = false;
            txtViewAnswer.ScrollEnabled = false;
            txtViewAnswer.TextAlignment = UITextAlignment.Justified;

            AddSubview(txtViewAnswer);
        }
    }
}