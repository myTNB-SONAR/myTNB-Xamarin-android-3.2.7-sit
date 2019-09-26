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
        public Func<string, string> GetI18NValue;

        private SelectBillsViewController _controller;
        private List<PaymentRecordModel> _accounts = new List<PaymentRecordModel>();
        private TextFieldHelper _textFieldHelper = new TextFieldHelper();
        private Dictionary<string, bool> _mandatoryPopupState = new Dictionary<string, bool>();

        public SelectBillsDataSource(SelectBillsViewController controller, List<PaymentRecordModel> accounts)
        {
            _controller = controller;
            _accounts = accounts;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            string CELLIDENTIFIER = "SelectBillsTableViewCell";
            string acctNumber = _accounts[indexPath.Row].accNum;
            bool hasMandatoryCharges = AccountChargesCache.HasMandatory(acctNumber);
            SelectBillsTableViewCell cell = tableView.DequeueReusableCell(CELLIDENTIFIER, indexPath) as SelectBillsTableViewCell;

            cell._lblName.Text = _accounts[indexPath.Row].accountNickName;
            cell._txtViewAddress.Text = _accounts[indexPath.Row].accountStAddress;
            cell._imgViewCheckBox.Image = UIImage.FromBundle(_accounts[indexPath.Row].IsAccountSelected
                ? "Payment-Checkbox-Active" : "Payment-Checkbox-Inactive");
            cell._txtFieldAmount.Placeholder = PaymentConstants.I18N_EnterAmount;
            cell._txtFieldAmount.Text = _accounts[indexPath.Row].Amount > 0
                ? _accounts[indexPath.Row].Amount.ToString("N2", CultureInfo.InvariantCulture) : string.Empty;

            cell.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                UpdateInlineError(cell, indexPath.Row);
            }));

            SetTextField(cell, indexPath.Row);
            ShowErrorMessage(indexPath.Row, cell, true);
            return cell;
        }

        private void UpdateInlineError(SelectBillsTableViewCell cell, int index)
        {
            if (_accounts[index].Amount >= TNBGlobal.PaymentMinAmnt || _accounts[index].IsAccountSelected)
            {
                if (AccountChargesCache.HasMandatory(_accounts[index].accNum) && !_accounts[index].IsAccountSelected
                    && !_mandatoryPopupState.ContainsKey(_accounts[index].accNum))
                {
                    _controller.OnShowItemisedTooltip(_accounts[index].accNum);
                    if (_mandatoryPopupState.ContainsKey(_accounts[index].accNum))
                    {
                        _mandatoryPopupState[_accounts[index].accNum] = true;
                    }
                    else
                    {
                        _mandatoryPopupState.Add(_accounts[index].accNum, true);
                    }
                }
                UpdateCheckBox(cell, index);
                UpdateUIForInputError(false, cell);
            }
            else
            {
                cell._lblAmountError.Text = "Invalid_PayAmount".Translate();
                if (AccountChargesCache.HasMandatory(_accounts[index].accNum))
                {
                    MandatoryChargesModel mandatoryCharges = AccountChargesCache.GetMandatoryCharges(_accounts[index].accNum);
                    double mandatoryAmount = mandatoryCharges.TotalAmount;
                    double.TryParse(cell._txtFieldAmount.Text, out double enteredAmt);

                    cell._lblAmountError.Text = string.Format(GetI18NValue(PaymentConstants.I18N_MinimumMandatoryPayment)
                        , string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, mandatoryAmount.ToString("N2", CultureInfo.InvariantCulture)));
                }
                cell._lblAmountError.Hidden = false;
                UpdateUIForInputError(true, cell);
            }
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
        private void UpdateCheckBox(SelectBillsTableViewCell cell, int index)
        {
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
        private void SetTextField(SelectBillsTableViewCell cell, int index)
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
                if (index > -1)
                {
                    double parsedAmount = TextHelper.ParseStringToDouble(cell._txtFieldAmount.Text);
                    _accounts[index].Amount = parsedAmount;
                    _controller.UpDateTotalAmount();
                }
            };
            cell._txtFieldAmount.ShouldEndEditing = (sender) =>
            {
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
            /*cell._txtFieldAmount.EditingDidEnd += (sender, e) =>
            {
                int index = _accounts.FindIndex(x => x.accNum.Equals(cell._lblAccountNo.Text));
                ShowErrorMessage(index, cell, true);
            };*/
        }
        #endregion
        #region ShowErrorMessage 
        private void ShowErrorMessage(int index, SelectBillsTableViewCell cell, bool isInitialLoad = false)
        {
            bool isValid = false;
            if (index < 0 || index >= _accounts.Count)
            {
                return;
            }
            if (AccountChargesCache.HasMandatory(_accounts[index].accNum))
            {
                MandatoryChargesModel mandatoryCharges = AccountChargesCache.GetMandatoryCharges(_accounts[index].accNum);
                double mandatoryAmount = mandatoryCharges.TotalAmount;
                double.TryParse(cell._txtFieldAmount.Text, out double enteredAmt);
                isValid = enteredAmt >= mandatoryAmount;
                cell._lblAmountError.Hidden = isValid;
                cell._lblAmountError.Text = string.Format(GetI18NValue(PaymentConstants.I18N_MinimumMandatoryPayment)
                    , string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, mandatoryAmount.ToString("N2", CultureInfo.InvariantCulture)));
            }
            else if (_accounts[index].Amount >= TNBGlobal.PaymentMinAmnt)
            {
                isValid = true;
                cell._lblAmountError.Hidden = isValid;
            }
            else if (isInitialLoad)
            {
                if (cell._lblAmountError.Hidden)
                {
                    cell._lblAmountError.Hidden = true;
                    isValid = true;
                }
                else
                {
                    isValid = false;
                }
            }
            else
            {
                cell._lblAmountError.Hidden = false;
                cell._lblAmountError.Text = GetI18NValue(PaymentConstants.I18N_MinimumPayAmount);
            }
            UpdateUIForInputError(!isValid, cell);
        }
        #endregion
        #region UpdateUIForInputError
        private void UpdateUIForInputError(bool isError, SelectBillsTableViewCell cell, bool endEditing = false)
        {
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