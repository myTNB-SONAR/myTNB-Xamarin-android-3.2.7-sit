using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class SubmittedFeedbackCell : UITableViewCell
    {
        public UIImageView imgViewIcon;

        public SubmittedFeedbackCell (IntPtr handle) : base (handle)
        {
            imgViewIcon = new UIImageView(new CGRect(16, 18, 24, 24));

            AddSubview(imgViewIcon);
        }

        public UILabel FeedbackTypeLabel
        {
            get
            {
                return lblFeedbackType;
            }
        }

        public UILabel FeedbackDetailsLabel
        {
            get
            {
                return lblDetails;
            }
        }

        public UILabel FeedbackDateLabel
        {
            get
            {
                return lblDate;
            }
        }
    }
}