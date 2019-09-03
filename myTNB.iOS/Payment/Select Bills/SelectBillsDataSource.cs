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
        List<PaymentRecordModel> _accounts = new List<PaymentRecordModel>();
        TextFieldHelper _textFieldHelper = new TextFieldHelper();
        Dictionary<string, bool> amountStatus = new Dictionary<string, bool>();

        public SelectBillsDataSource(SelectBillsViewController controller, List<PaymentRecordModel> accounts)
        {
            _controller = controller;
            _accounts = accounts;
            if (_accounts != null)
            {
                foreach (var obj in _accounts)
                {
                    amountStatus.Add(obj.accNum, true);
                }
            }
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            string CELLIDENTIFIER = "SelectBillsTableViewCell";
            string acctNumber = _accounts[indexPath.Row].accNum;

            var cell = tableView.DequeueReusableCell(CELLIDENTIFIER, indexPath) as SelectBillsTableViewCell;
            cell._lblName.Text = _accounts[indexPath.Row].accountNickName;
            cell._lblAccountNo.Text = acctNumber;
            cell._txtViewAddress.Text = _accounts[indexPath.Row].accountStAddress;
            cell._imgViewCheckBox.Image = UIImage.FromBundle(_accounts[indexPath.Row].IsAccountSelected
                ? "Payment-Checkbox-Active" : "Payment-Checkbox-Inactive");
            if (_accounts[indexPath.Row].Amount > 0)
            {
                cell._txtFieldAmount.Text = _accounts[indexPath.Row].Amount.ToString("N2", CultureInfo.InvariantCulture);
            }
            cell._txtFieldAmount.Placeholder = "Enter amount";
            cell._viewCheckBox.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (_accounts[indexPath.Row].Amount >= TNBGlobal.PaymentMinAmnt)
                {
                    UpdateCheckBox(cell);
                    UpdateUIForInputError(false, cell);
                }
                else
                {
                    cell._lblAmountError.Text = "Invalid_PayAmount".Translate();
                    cell._lblAmountError.Hidden = false;
                    UpdateUIForInputError(true, cell);
                }
            }));
            SetTextField(cell);
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

        #region UpdateCheckBox
        private void UpdateCheckBox(SelectBillsTableViewCell cell)
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
                cell._imgViewCheckBox.Image = UIImage.FromBundle(!isAccountSelected
                    ? "Payment-Checkbox-Active" : "Payment-Checkbox-Inactive");
                _controller.UpDateTotalAmount();
            }
        }
        #endregion
        #region SetTextField
        private void SetTextField(SelectBillsTableViewCell cell)
        {
            cell._txtFieldAmount.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            cell._txtFieldAmount.EditingChanged += (sender, e) =>
            {
                cell._lblAmountError.Hidden = true;
                UpdateUIForInputError(false, cell);
                int index = _accounts.FindIndex(x => x.accNum.Equals(cell._lblAccountNo.Text));
                if (index > -1)
                {
                    double parsedAmount = TextHelper.ParseStringToDouble(cell._txtFieldAmount.Text);
                    _accounts[index].Amount = parsedAmount;
                    _controller.UpDateTotalAmount();
                }
            };
            cell._txtFieldAmount.ShouldEndEditing = (sender) =>
            {
                int index = _accounts.FindIndex(x => x.accNum.Equals(cell._lblAccountNo.Text));
                ShowErrorMessage(index, cell);
                _controller.UpDateTotalAmount();
                return true;
            };
            cell._txtFieldAmount.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            cell._txtFieldAmount.ShouldChangeCharacters += (txtField, range, replacement) =>
            {
                bool isCharValid = _textFieldHelper.ValidateTextField(replacement, TNBGlobal.AmountPattern);

                if (!isCharValid)
                {
                    return false;
                }

                if (txtField.Text.Contains("."))
                {
                    int indx = txtField.Text.IndexOf(".", StringComparison.InvariantCulture);
                    if (range.Location > indx)
                    {
                        if (replacement == ".")
                        {
                            return false;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(replacement))
                            {
                                string[] str = cell._txtFieldAmount.Text.Split('.');
                                if (str[1] != null)
                                {
                                    if (str[1].Length == 2)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (replacement == ".")
                        {
                            return false;
                        }
                    }
                }

                return true;
            };
            cell._txtFieldAmount.EditingDidEnd += (sender, e) =>
            {
                int index = _accounts.FindIndex(x => x.accNum.Equals(cell._lblAccountNo.Text));
                ShowErrorMessage(index, cell, true);
            };
        }
        #endregion
        #region ShowErrorMessage 
        private bool ShowErrorMessage(int index, SelectBillsTableViewCell cell, bool endEditing = false)
        {
            bool isValid = false;
            if (index < 0 || index >= _accounts.Count)
            {
                return isValid;
            }
            if (_accounts[index].Amount >= TNBGlobal.PaymentMinAmnt)
            {
                isValid = true;
                cell._lblAmountError.Hidden = isValid;
                UpdateUIForInputError(false, cell, endEditing);
            }
            else
            {
                cell._lblAmountError.Hidden = false;
                cell._lblAmountError.Text = "Invalid_PayAmount".Translate();
                UpdateUIForInputError(true, cell, endEditing);
            }
            return isValid;
        }
        #endregion
        #region UpdateUIForInputError
        private void UpdateUIForInputError(bool isError, SelectBillsTableViewCell cell, bool endEditing = false)
        {
            string acctNumber = cell._lblAccountNo.Text;
            if (!string.IsNullOrEmpty(acctNumber))
            {
                if (amountStatus.ContainsKey(acctNumber))
                {
                    amountStatus[acctNumber] = !isError;
                }
            }

            UIView viewLine = cell.ViewWithTag(0).ViewWithTag(1) as UIView;
            if (isError)
            {
                cell._txtFieldAmount.TextColor = MyTNBColor.Tomato;
                viewLine.BackgroundColor = MyTNBColor.Tomato;
            }
            else
            {
                cell._txtFieldAmount.TextColor = MyTNBColor.TunaGrey();
                viewLine.BackgroundColor = (endEditing) ? MyTNBColor.PlatinumGrey : MyTNBColor.PowerBlue;
            }
        }
        #endregion
    }
}