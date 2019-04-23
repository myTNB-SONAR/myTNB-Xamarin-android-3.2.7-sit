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
                lblAccountNumber.Font = MyTNBFont.MuseoSans16_300;
                lblAccountNumber.TextColor = MyTNBColor.TunaGrey();
                return lblAccountNumber;
            }
        }
    }
}