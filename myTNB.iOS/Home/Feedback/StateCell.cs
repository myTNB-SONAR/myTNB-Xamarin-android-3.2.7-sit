using Foundation;
using System;
using UIKit;

namespace myTNB
{
    public partial class StateCell : UITableViewCell
    {
        public StateCell (IntPtr handle) : base (handle)
        {
        }

        public UILabel StateForFeedbackLabel
        {
            get
            {
                lblState.Font = myTNBFont.MuseoSans16_300();
                lblState.TextColor = myTNBColor.TunaGrey();
                return lblState;
            }
        }
    }
}