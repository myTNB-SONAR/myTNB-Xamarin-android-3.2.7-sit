using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class SubmittedFeedbackCell : UITableViewCell
    {
        public UIImageView imgViewIcon;

        public SubmittedFeedbackCell(IntPtr handle) : base(handle)
        {
            imgViewIcon = new UIImageView(new CGRect(16, 20, 24, 24));

            AddSubview(imgViewIcon);
        }

        public void UpdateStyle()
        {
            lblFeedbackType.Font = MyTNBFont.MuseoSans14_300;
            lblFeedbackType.TextColor = MyTNBColor.TunaGrey();
            //lblDetails.Font = MyTNBFont.MuseoSans9_300;
            //lblDetails.TextColor = MyTNBColor.SilverChalice;
            lblDate.Font = MyTNBFont.MuseoSans9_300;
            lblDate.TextColor = MyTNBColor.SilverChalice;
        }

        public UILabel FeedbackTypeLabel
        {
            get
            {
                return lblFeedbackType;
            }
        }

        //public UILabel FeedbackDetailsLabel
        //{
        //    get
        //    {
        //        return lblDetails;
        //    }
        //}

        public UILabel FeedbackDateLabel
        {
            get
            {
                return lblDate;
            }
        }
    }
}