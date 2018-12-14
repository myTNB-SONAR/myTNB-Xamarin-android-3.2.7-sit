using Foundation;
using System;
using UIKit;

namespace myTNB
{
    public partial class FeedbackAccountCell : UITableViewCell
    {
        public FeedbackAccountCell (IntPtr handle) : base (handle)
        {
        }

        public UILabel AccountNumber
        {
            get
            {
                return lblAccountNumber;
            }
        }

    }
}