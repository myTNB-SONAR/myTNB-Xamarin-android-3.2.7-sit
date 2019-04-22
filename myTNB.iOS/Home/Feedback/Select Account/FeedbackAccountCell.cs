using System;
using UIKit;

namespace myTNB
{
    public partial class FeedbackAccountCell : UITableViewCell
    {
        public FeedbackAccountCell(IntPtr handle) : base(handle)
        {
        }

        public UILabel AccountNumber
        {
            get
            {
                lblAccountNumber.Font = myTNBFont.MuseoSans16_300();
                lblAccountNumber.TextColor = myTNBColor.TunaGrey();
                return lblAccountNumber;
            }
        }
    }
}