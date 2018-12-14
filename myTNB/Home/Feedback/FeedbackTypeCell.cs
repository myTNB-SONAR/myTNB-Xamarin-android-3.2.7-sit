using Foundation;
using System;
using UIKit;

namespace myTNB
{
    public partial class FeedbackTypeCell : UITableViewCell
    {
        public FeedbackTypeCell (IntPtr handle) : base (handle)
        {
        }

        public UILabel FeedbackTypeLabel
        {
            get
            {
                return lblFeedbackType;
            }
        }
    }
}