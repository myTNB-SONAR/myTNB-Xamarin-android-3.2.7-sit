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
                return lblState;
            }
        }
    }
}