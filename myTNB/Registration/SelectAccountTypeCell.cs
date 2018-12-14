using Foundation;
using System;
using UIKit;

namespace myTNB
{
    public partial class SelectAccountTypeCell : UITableViewCell
    {
        public SelectAccountTypeCell (IntPtr handle) : base (handle)
        {
        }

        public UILabel AccountTypeLabel
        {
            get
            {
                return lblAccountType;
            }
        }

    }
}