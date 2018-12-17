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
        /// <summary>
        /// Updates the style.
        /// </summary>
        public void UpdateStyle()
        {
            lblFeedbackType.Font = myTNBFont.MuseoSans14_300();
            lblFeedbackType.TextColor = myTNBColor.TunaGrey();
            lblDetails.Font = myTNBFont.MuseoSans9_300();
            lblDetails.TextColor = myTNBColor.SilverChalice();
            lblDate.Font = myTNBFont.MuseoSans9_300();
            lblDate.TextColor = myTNBColor.SilverChalice();
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