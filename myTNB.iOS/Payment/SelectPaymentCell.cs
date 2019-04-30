using System;
using UIKit;

namespace myTNB
{
    public partial class SelectPaymentCell : UITableViewCell
    {
        public UIImageView imgView;
        public UILabel ccNumber;

        public SelectPaymentCell (IntPtr handle) : base (handle)
        {
        }
    }
}