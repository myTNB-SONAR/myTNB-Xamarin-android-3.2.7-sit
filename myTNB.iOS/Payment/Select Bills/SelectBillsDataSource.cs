using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB.Payment.SelectBills
{
    public class SelectBillsDataSource : UITableViewSource
    {
        SelectBillsViewController _controller;
        List<CustomerAccountRecordModel> _accounts = new List<CustomerAccountRecordModel>();
        MultiAccountDueAmountResponseModel _multiAccountDueAmount = new MultiAccountDueAmountResponseModel();
        TextFieldHelper _textFieldHelper = new TextFieldHelper();
        Dictionary<string, bool> amountStatus = new Dictionary<string, bool>();

        public SelectBillsDataSource(SelectBillsViewController controller, List<CustomerAccountRecordModel> accounts
            , MultiAccountDueAmountResponseModel multiAccountDueAmount)
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
            _multiAccountDueAmount = multiAccountDueAmount;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            const string CELLIDENTIFIER = "SelectBillsTableViewCell";
            SelectBillsTableViewCell cell = tableView.DequeueReusableCell(CELLIDENTIFIER, indexPath) as SelectBillsTableViewCell;
            string acctNumber = _accounts[indexPath.Row].accNum;

            cell._lblName.Text = _accounts[indexPath.Row].accountNickName;
            cell._lblAccountNo.Text = acctNumber;
            cell._txtViewAddress.Text = _accounts[indexPath.Row].accountStAddress;
            cell._imgViewCheckBox.Image = UIImage.FromBundle(_accounts[indexPath.Row].IsAccountSelected
                ? "Payment-Checkbox-Active" : "Payment-Checkbox-Inactive");
            cell._txtFieldAmount.Text = _accounts[indexPath.Row].Amount.ToString("N2", CultureInfo.InvariantCulture);
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

            var isValidAmount = amountStatus.ContainsKey(acctNumber) ? amountStatus[acctNumber] : true;
            cell._txtFieldAmount.TextColor = isValidAmount ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;
            cell._lblAmountError.Hidden = isValidAmount;
            cell._viewLineAmount.BackgroundColor = isValidAmount ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
            SetTextField(cell._txtFieldAmount, cell._lblAmountError, cell);

            int acctIndex = _multiAccountDueAmount.d.data.FindIndex(x => x.accNum == acctNumber);
            if (acctIndex > -1 && _multiAccountDueAmount.d.data[acctIndex].IsItemisedBilling)
            {
                cell.AddMandatoryPayment(_multiAccountDueAmount.d.data[acctIndex].OpenChargesTotal);
            }
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _accounts.Count;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            string acctNumber = _accounts[indexPath.Row].accNum;
            int acctIndex = _multiAccountDueAmount.d.data.FindIndex(x => x.accNum == acctNumber);
            nfloat val = acctIndex > -1 && _multiAccountDueAmount.d.data[acctIndex].IsItemisedBilling ? 257 : 205;
            return val;
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
                cell._imgViewCheckBox.Image = UIImage.FromBundle(!isAccountSelected
                    ? "Payment-Checkbox-Active" : "Payment-Checkbox-Inactive");
                _controller.UpDateTotalAmount();
                if (!isAccountSelected)
                {
                    _controller.OnShowItemisedTooltip(_accounts[index].accNum);
                }
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
                error.Hidden = true;
                UpdateUIForInputError(false, cell);
                int index = _accounts.FindIndex(x => x.accNum.Equals(cell._lblAccountNo.Text));
                if (index > -1)
                {
                    double parsedAmount = TextHelper.ParseStringToDouble(cell._txtFieldAmount.Text);
                    _accounts[index].Amount = parsedAmount;
                    _controller.UpDateTotalAmount();
                }
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                cell._viewLineAmount.BackgroundColor = MyTNBColor.PowerBlue;
                int index = _accounts.FindIndex(x => x.accNum.Equals(cell._lblAccountNo.Text));
                ShowErrorMessage(error, index, cell);
            };
            textField.ShouldEndEditing = (sender) =>
            {
                int index = _accounts.FindIndex(x => x.accNum.Equals(cell._lblAccountNo.Text));
                ShowErrorMessage(error, index, cell);
                _controller.UpDateTotalAmount();
                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            textField.ShouldChangeCharacters += (txtField, range, replacement) =>
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
                                string[] str = textField.Text.Split('.');
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
            textField.EditingDidEnd += (sender, e) =>
            {
                int index = _accounts.FindIndex(x => x.accNum.Equals(cell._lblAccountNo.Text));
                ShowErrorMessage(error, index, cell, true);
            };
        }

        bool ShowErrorMessage(UILabel lblError, int index, SelectBillsTableViewCell cell, bool endEditing = false)
        {
            bool isValid = false;
            if (index < 0 || index >= _accounts.Count)
            {
                return isValid;
            }
            if (_accounts[index].Amount >= TNBGlobal.PaymentMinAmnt)
            {
                //lblError.Text = "Amount can be equal to or more than your due amount.";
                //FOR UAT TESTING
                //isValid = _accounts[index].Amount >= _accounts[index].AmountDue;
                isValid = true;
                lblError.Hidden = isValid;
                UpdateUIForInputError(false, cell, endEditing);
            }
            else
            {
                lblError.Hidden = false;
                lblError.Text = "Invalid_PayAmount".Translate();
                UpdateUIForInputError(true, cell, endEditing);
            }
            return isValid;
        }
        /// <summary>
        /// Updates the UI based on user input validity.
        /// </summary>
        /// <param name="isError">If set to <c>true</c> is error.</param>
        /// <param name="cell">Cell.</param>
        void UpdateUIForInputError(bool isError, SelectBillsTableViewCell cell, bool endEditing = false)
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
    }
}