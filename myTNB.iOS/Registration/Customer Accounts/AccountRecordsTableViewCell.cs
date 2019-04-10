using System;
using UIKit;

namespace myTNB.Registration.CustomerAccounts
{
    public partial class AccountRecordsTableViewCell : UITableViewCell
    {
        public AccountRecordsTableViewCell(IntPtr handle) : base(handle)
        {
        }

        public UIButton DeleteButton
        {
            get
            {
                return btnDelete;
            }
        }

        public string NickNameTitle
        {
            set
            {
                lblNickNameTitle.Text = value;
            }
            get
            {
                return lblNickNameTitle.Text;
            }
        }

        public UILabel NickNameTitleLabel
        {
            get
            {
                return lblNickNameTitle;
            }
        }

        public string NicknameError
        {
            set
            {
                lblNicknameError.Text = value;
            }
            get
            {
                return lblNicknameError.Text;
            }
        }
        public UILabel NickNameErrorLabel
        {
            get
            {
                return lblNicknameError;
            }
        }

        public string AccountNumber
        {
            set
            {
                lblAccountNo.Text = value;
            }
            get
            {
                return lblAccountNo.Text;
            }
        }

        public string Address
        {
            set
            {
                lblAddress.Text = value;
            }
            get
            {
                return lblAddress.Text;
            }
        }

        public UITextField NicknameTextField
        {
            get
            {
                return txtFieldNickname;
            }
        }

        public UIView LineView
        {
            get
            {
                return lineView;
            }
        }

        public UIView SeparatorView
        {
            get
            {
                return viewSeparator;
            }
        }
    }
}