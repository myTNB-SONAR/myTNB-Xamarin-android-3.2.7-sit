using System;
using UIKit;

namespace myTNB
{
    public partial class StateCell : UITableViewCell
    {
        public StateCell(IntPtr handle) : base(handle)
        {
        }

        public UILabel StateForFeedbackLabel
        {
            get
            {
                lblState.Font = MyTNBFont.MuseoSans16_300;
                lblState.TextColor = MyTNBColor.TunaGrey();
                return lblState;
            }
        }
    }
}