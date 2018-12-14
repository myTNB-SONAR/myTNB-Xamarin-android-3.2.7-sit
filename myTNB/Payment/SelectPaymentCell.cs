using Foundation;
using System;
using UIKit;
using CoreGraphics;

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