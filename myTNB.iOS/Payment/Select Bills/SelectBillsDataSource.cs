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
        private SelectBillsViewController _controller;
        private List<PaymentRecordModel> _accounts = new List<PaymentRecordModel>();
        private TextFieldHelper _textFieldHelper = new TextFieldHelper();
        private Dictionary<string, bool> _amountStatus = new Dictionary<string, bool>();

        public SelectBillsDataSource(SelectBillsViewController controller, List<PaymentRecordModel> accounts)
        {
            _controller = controller;
            _accounts = accounts;
            if (_accounts != null)
            {
                foreach (PaymentRecordModel obj in _accounts)
                {
                    _amountStatus.Add(obj.accNum, true);

                    if (AccountChargesCache.HasMandatory(obj.accNum))
                    {
                        obj.MandatoryCharges = AccountChargesCache.GetMandatoryCharges(obj.accNum);
                    }
                }
            }
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            string acctNumber = _accounts[indexPath.Row].accNum;
            SelectBillsTableViewCell cell = tableView.DequeueReusableCell(PaymentConstants.Cell_SelectBills, indexPath) as SelectBillsTableViewCell;
            string inlineError = string.IsNullOrEmpty(cell._lblAmountError.Text) || string.IsNullOrWhiteSpace(cell._lblAmountError.Text)
                ? string.Empty : _accounts[indexPath.Row].InlineValidationMessage;
            cell._lblAmountError.Text = inlineError;
            cell._lblName.Text = _accounts[indexPath.Row].accountNickName;
            cell._lblAccountNo.Text = acctNumber;
            cell._txtViewAddress.Text = _accounts[indexPath.Row].accountStAddress;
            cell._imgViewCheckBox.Image = UIImage.FromBundle(_accounts[indexPath.Row].IsAccountSelected
                ? PaymentConstants.IMG_CheckboxActive : PaymentConstants.IMG_CheckboxInactive);
            cell._txtFieldAmount.Placeholder = _controller.GetI18NValue(PaymentConstants.I18N_EnterAmount);
            cell._txtFieldAmount.Text = _accounts[indexPath.Row].Amount > 0
                ? _accounts[indexPath.Row].Amount.ToString("N2", CultureInfo.InvariantCulture) : string.Empty;

            cell.AmountTitle = _controller.GetI18NValue(PaymentConstants.I18N_IAmPaying);
            cell.AmountError = _controller.GetErrorI18NValue(Constants.Error_MinimumPayAmount);

            cell.UserInteractionEnabled = true;
            cell.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (_controller.IsFromBillDetails)
                {
                    if (indexPath.Row > 0)
                    {
                        if (_accounts[indexPath.Row].HasMandatory && !_accounts[indexPath.Row].IsAccountSelected && _accounts[indexPath.Row].IsValidAmount)
                        {
                            _controller.OnShowItemisedTooltip(_accounts[indexPath.Row].accNum);
                        }
                    }
                }
                else
                {
                    if (_accounts[indexPath.Row].HasMandatory && !_accounts[indexPath.Row].IsAccountSelected && _accounts[indexPath.Row].IsValidAmount)
                    {
                        _controller.OnShowItemisedTooltip(_accounts[indexPath.Row].accNum);
                    }
                }
                if (_accounts[indexPath.Row].Amount >= _accounts[indexPath.Row].MinimumAmount || _accounts[indexPath.Row].IsAccountSelected)
                {
                    UpdateCheckBox(cell);
                    UpdateUIForInputError(false, cell);
                }
                else
                {
                    cell._lblAmountError.Text = _accounts[indexPath.Row].InlineValidationMessage;
                    cell._lblAmountError.Hidden = false;
                    UpdateUIForInputError(true, cell);
                }
            }));
            bool isValidAmount = NewMethod(acctNumber);
            cell._txtFieldAmount.TextColor = isValidAmount ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;
            cell._lblAmountError.Hidden = isValidAmount;
            cell._viewLineAmount.BackgroundColor = isValidAmount ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
            SetTextField(cell._txtFieldAmount, cell._lblAmountError, cell);
            return cell;
        }

        private bool NewMethod(string acctNumber)
        {
            return _amountStatus.ContainsKey(acctNumber) ? _amountStatus[acctNumber] : true;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _accounts.Count;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 187;
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
                    ? PaymentConstants.IMG_CheckboxActive : PaymentConstants.IMG_CheckboxInactive);
                _controller.UpDateTotalAmount();
            }
        }
        #endregion

        private void UnSelectBill(SelectBillsTableViewCell cell, int index)
        {
            if (_accounts[index].IsAccountSelected && _accounts[index].Amount < _accounts[index].MinimumAmount)
            {
                _accounts[index].IsAccountSelected = false;
                cell._imgViewCheckBox.Image = UIImage.FromBundle(PaymentConstants.IMG_CheckboxInactive);
            }
        }

        #region SetTextField
        private void SetTextField(UITextField textField, UILabel error, SelectBillsTableViewCell cell)
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
                    double.TryParse(cell._txtFieldAmount.Text, out double parsedAmount);
                    _accounts[index].Amount = parsedAmount;
                    UnSelectBill(cell, index);
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
                UnSelectBill(cell, index);
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
                if (!string.IsNullOrEmpty(cell._txtFieldAmount.Text) && !string.IsNullOrWhiteSpace(cell._txtFieldAmount.Text))
                {
                    double.TryParse(cell._txtFieldAmount.Text, out double parsedAmount);
                    cell._txtFieldAmount.Text = parsedAmount.ToString("N2", CultureInfo.InvariantCulture);
                }
                ShowErrorMessage(error, index, cell, true);
            };
        }
        #endregion
        #region ShowErrorMessage 
        private bool ShowErrorMessage(UILabel lblError, int index, SelectBillsTableViewCell cell, bool endEditing = false)
        {
            bool isValid = false;
            if (index < 0 || index >= _accounts.Count)
            {
                return isValid;
            }
            if (_accounts[index].Amount >= _accounts[index].MinimumAmount)
            {
                isValid = true;
                lblError.Hidden = isValid;
                UpdateUIForInputError(false, cell, endEditing);
            }
            else
            {
                string inputVal = cell._txtFieldAmount.Text;
                if (string.IsNullOrEmpty(inputVal) || string.IsNullOrWhiteSpace(inputVal))
                {
                    lblError.Hidden = true;
                    cell._viewLineAmount.BackgroundColor = MyTNBColor.PlatinumGrey;
                    return false;
                }
                lblError.Hidden = false;
                lblError.Text = _accounts[index].InlineValidationMessage;
                UpdateUIForInputError(true, cell, endEditing);
            }
            return isValid;
        }
        #endregion
        #region UpdateUIForInputError
        /// <summary>
        /// Updates the UI based on user input validity.
        /// </summary>
        /// <param name="isError">If set to <c>true</c> is error.</param>
        /// <param name="cell">Cell.</param>
        private void UpdateUIForInputError(bool isError, SelectBillsTableViewCell cell, bool endEditing = false)
        {
            string acctNumber = cell._lblAccountNo.Text;
            if (!string.IsNullOrEmpty(acctNumber))
            {
                if (_amountStatus.ContainsKey(acctNumber))
                {
                    _amountStatus[acctNumber] = !isError;
                }
            }

            if (isError)
            {
                cell._txtFieldAmount.TextColor = MyTNBColor.Tomato;
                cell._viewLineAmount.BackgroundColor = MyTNBColor.Tomato;
            }
            else
            {
                cell._txtFieldAmount.TextColor = MyTNBColor.TunaGrey();
                cell._viewLineAmount.BackgroundColor = (endEditing) ? MyTNBColor.PlatinumGrey : MyTNBColor.PowerBlue;
            }
        }
        #endregion
    }
}