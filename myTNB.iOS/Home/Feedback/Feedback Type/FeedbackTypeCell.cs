using System;
using UIKit;

namespace myTNB
{
    public partial class FeedbackTypeCell : UITableViewCell
    {
        public FeedbackTypeCell(IntPtr handle) : base(handle)
        {
        }
        /// <summary>
        /// Updates the style.
        /// </summary>
        public void UpdateStyle()
        {
            lblFeedbackType.Font = MyTNBFont.MuseoSans16_300;
            lblFeedbackType.TextColor = MyTNBColor.TunaGrey();
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