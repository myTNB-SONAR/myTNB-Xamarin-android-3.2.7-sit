using System;
using System.Collections.Generic;
using System.Globalization;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB.Payment.SelectBills
{
    public class SelectBillsDataSource : UITableViewSource
    {
        SelectBillsViewController _controller;
        List<CustomerAccountRecordModel> _accounts = new List<CustomerAccountRecordModel>();

        public SelectBillsDataSource(SelectBillsViewController controller, List<CustomerAccountRecordModel> accounts)
        {
            _controller = controller;
            _accounts = accounts;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            const string CELLIDENTIFIER = "SelectBillsTableViewCell";
            SelectBillsTableViewCell cell = tableView.DequeueReusableCell(CELLIDENTIFIER, indexPath) as SelectBillsTableViewCell;

            cell._lblName.Text = _accounts[indexPath.Row].accountNickName;
            cell._lblAccountNo.Text = _accounts[indexPath.Row].accNum;
            cell._txtViewAddress.Text = _accounts[indexPath.Row].accountStAddress;
            cell._imgViewCheckBox.Image = UIImage.FromBundle(_accounts[indexPath.Row].IsAccountSelected
                                                             ? "Payment-Checkbox-Active"
                                                             : "Payment-Checkbox-Inactive");
            cell._txtFieldAmount.Text = _accounts[indexPath.Row].Amount.ToString("N2", CultureInfo.InvariantCulture);

            cell._viewCheckBox.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (_accounts[indexPath.Row].Amount > 0)
                {
                    UpdateCheckBox(cell);
                }
                else
                {
                    cell._lblAmountError.Text = "Please enter valid amount";
                    cell._lblAmountError.Hidden = false;
                }
            }));


            SetTextField(cell._txtFieldAmount, cell._lblAmountError, cell);

            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _accounts.Count;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 205;
        }

        void UpdateCheckBox(SelectBillsTableViewCell cell)
        {
            int index = _accounts.FindIndex(x => x.accNum.Equals(cell._lblAccountNo.Text));
            if (index > -1)
            {
                bool isAccountSelected = _accounts[index].IsAccountSelected;
                if (!isAccountSelected)
                {
                    int selectedCount = _accounts.FindAll(x => x.IsAccountSelected == true).Count;
                    if (selectedCount >= 5)
                    {
                        return;
                    }
                }
                _accounts[index].IsAccountSelected = !isAccountSelected;
                cell._imgViewCheckBox.Image = UIImage.FromBundle(_accounts[index].IsAccountSelected
                                                             ? "Payment-Checkbox-Active"
                                                             : "Payment-Checkbox-Inactive");
                _controller.UpDateTotalAmount();
            }
        }

        void SetTextField(UITextField textField, UILabel error, SelectBillsTableViewCell cell)
        {
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            textField.EditingChanged += (sender, e) =>
            {
                int index = _accounts.FindIndex(x => x.accNum.Equals(cell._lblAccountNo.Text));
                if (index > -1)
                {
                    double parsedAmount = 0;
                    if (double.TryParse(cell._txtFieldAmount.Text, out parsedAmount))
                    {
                        _accounts[index].Amount = parsedAmount;
                    }
                    else
                    {
                        _accounts[index].Amount = 0.00;
                    }
                    _controller.UpDateTotalAmount();
                }
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                int index = _accounts.FindIndex(x => x.accNum.Equals(cell._lblAccountNo.Text));
                ShowErrorMessage(error, index);
            };
            textField.ShouldEndEditing = (sender) =>
            {
                int index = _accounts.FindIndex(x => x.accNum.Equals(cell._lblAccountNo.Text));
                ShowErrorMessage(error, index);
                _controller.UpDateTotalAmount();
                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
        }

        bool ShowErrorMessage(UILabel lblError, int index)
        {
            bool isValid = false;
            if (index < 0 || index >= _accounts.Count)
            {
                return isValid;
            }
            if (_accounts[index].Amount > 0)
            {
                lblError.Text = "Amount can be equal to or more than your due amount.";
                //FOR UAT TESTING
                //isValid = _accounts[index].Amount >= _accounts[index].AmountDue;
                isValid = true;
                lblError.Hidden = isValid;
            }
            else
            {
                lblError.Hidden = false;
                lblError.Text = "Please enter valid amount";
            }
            return isValid;
        }
    }
}