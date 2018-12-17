using Foundation;
using System;
using UIKit;

namespace myTNB
{
    public partial class SuccessfullyAddedAccountCell : UITableViewCell
    {
        public SuccessfullyAddedAccountCell (IntPtr handle) : base (handle)
        {
        }

        public UILabel NickNameLabel
        {
            get
            {
                return lblNickName;
            }
        }

        public UILabel AccountNumberLabel
        {
            get
            {
                return lblAccountNumber;
            }
        }

        public UITextView AddressTextView
        {
            get
            {
                return txtViewAddress;
            }
        }

    }

}