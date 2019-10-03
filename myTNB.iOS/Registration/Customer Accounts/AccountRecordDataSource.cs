using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB.Registration.CustomerAccounts
{
    public class AccountRecordDataSource : UITableViewSource
    {
        public Func<string, string> GetI18NValue;
        public CustomerAccountRecordListModel _accountList = new CustomerAccountRecordListModel();
        TextFieldHelper _txtFieldHelper = new TextFieldHelper();
        const string NAME_PATTERN = @"^[A-Za-z0-9 _]*[A-Za-z0-9][A-Za-z0-9 \-\\_ _]*$";
        int _recordCount;
        int _sectionCount;

        List<CustomerAccountRecordModel> _linkedAccounts = new List<CustomerAccountRecordModel>();
        List<CustomerAccountRecordModel> _localAccounts = new List<CustomerAccountRecordModel>();
        AccountsViewController _controller;

        NSMutableArray _oldAccounts = new NSMutableArray();

        public AccountRecordDataSource(CustomerAccountRecordListModel accountList, AccountsViewController controller)
        {
            _controller = controller;
            if (accountList != null && accountList?.d != null)
            {
                _accountList = accountList;
            }

            if (DataManager.DataManager.SharedInstance.AccountsToBeAddedList != null
                && DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d != null)
            {
                foreach (var account in DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d)
                {
                    if (account.isLocal == false)
                    {
                        _linkedAccounts?.Add(account);
                    }
                    else
                    {
                        _localAccounts?.Add(account);
                    }
                }

                if (_linkedAccounts?.Count > 0 && _localAccounts?.Count > 0)
                {
                    _sectionCount = 2;
                }
                else
                {
                    _sectionCount = 1;
                }
            }
            else
            {
                _sectionCount = 1;
            }
            _recordCount = (int)(_linkedAccounts != null
                     ? _linkedAccounts?.Count : 0);
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return _sectionCount;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (_linkedAccounts?.Count == 0 && _localAccounts?.Count == 0)
            {
                if (section == 0)
                {
                    return _linkedAccounts != null ? _linkedAccounts.Count : 0;
                }
            }
            else if (_linkedAccounts?.Count > 0 && _localAccounts?.Count > 0)
            {
                if (section == 0)
                {
                    return _linkedAccounts != null ? _linkedAccounts.Count : 0;
                }
                else
                {
                    return _localAccounts != null ? _localAccounts.Count : 0;
                }
            }
            else if (_linkedAccounts?.Count == 0 && _localAccounts?.Count > 0)
            {
                if (section == 0)
                {
                    return _localAccounts != null ? _localAccounts.Count : 0;
                }
            }
            else if (_linkedAccounts?.Count > 0 && _localAccounts?.Count == 0)
            {
                if (section == 0)
                {
                    return _linkedAccounts != null ? _linkedAccounts.Count : 0;
                }
            }
            return 0;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            if (section == 0)
            {
                if ((_linkedAccounts != null && _linkedAccounts?.Count > 0 && _localAccounts?.Count >= 0) || (_linkedAccounts != null && _linkedAccounts?.Count == 0 && _localAccounts?.Count == 0))
                {
                    return ScaleUtility.GetScaledHeight(92);
                }

                if (_localAccounts != null && _localAccounts?.Count > 0 && _linkedAccounts?.Count == 0)
                {
                    return ScaleUtility.GetScaledHeight(30);
                }
            }

            if (section == 1)
            {
                if (_localAccounts != null && _localAccounts?.Count > 0)
                {
                    return ScaleUtility.GetScaledHeight(40);
                }
            }
            return ScaleUtility.GetScaledHeight(30);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return ScaleUtility.GetScaledHeight(175);
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            UIView view = new UIView(new CGRect(0, 0, tableView.Frame.Width, ScaleUtility.GetScaledHeight(100)))
            {
                BackgroundColor = MyTNBColor.SectionGrey
            };

            var lblSectionTitle = new UILabel(new CGRect(ScaleUtility.GetScaledWidth(18), 0, tableView.Frame.Width - ScaleUtility.GetScaledWidth(36), ScaleUtility.GetScaledHeight(140)));

            if (section == 0)
            {
                if ((_linkedAccounts != null && _linkedAccounts?.Count > 0 && _localAccounts?.Count >= 0)
                    || (_linkedAccounts != null && _linkedAccounts?.Count == 0 && _localAccounts?.Count == 0))
                {
                    lblSectionTitle = new UILabel(new CGRect(ScaleUtility.GetScaledWidth(18), 0, tableView.Frame.Width - ScaleUtility.GetScaledWidth(36), ScaleUtility.GetScaledHeight(40)))
                    {
                        Text = _recordCount > 0
                        ? string.Format("{0} {1}", _recordCount.ToString(), GetI18NValue(AddAccountConstants.I18N_SupplyAcctCount))
                        : GetI18NValue(AddAccountConstants.I18N_NoAcctsTitle)
                    };

                    var txtViewSubDetails = new UITextView(new CGRect(ScaleUtility.GetScaledWidth(14), ScaleUtility.GetScaledHeight(36), tableView.Frame.Width - ScaleUtility.GetScaledWidth(30), ScaleUtility.GetScaledHeight(60)))
                    {
                        Font = TNBFont.MuseoSans_14_300,
                        TextColor = MyTNBColor.TunaGrey(),
                        UserInteractionEnabled = false,
                        BackgroundColor = UIColor.Clear,
                        Text = GetI18NValue(AddAccountConstants.I18N_NoAcctFoundMsg)
                    };

                    view.AddSubview(txtViewSubDetails);

                }

                if (_localAccounts != null && _localAccounts?.Count > 0 && _linkedAccounts?.Count == 0)
                {
                    lblSectionTitle = new UILabel(new CGRect(ScaleUtility.GetScaledWidth(18), 0, tableView.Frame.Width - ScaleUtility.GetScaledWidth(36), ScaleUtility.GetScaledHeight(40)))
                    {
                        Text = GetI18NValue(AddAccountConstants.I18N_AdditionalAccts)
                    };
                }
            }

            if (section == 1)
            {
                if (_localAccounts != null && _localAccounts?.Count > 0)
                {
                    lblSectionTitle = new UILabel(new CGRect(ScaleUtility.GetScaledWidth(18), 0, tableView.Frame.Width - ScaleUtility.GetScaledWidth(36), ScaleUtility.GetScaledHeight(40)))
                    {
                        Text = GetI18NValue(AddAccountConstants.I18N_AdditionalAccts)
                    };
                }
            }

            lblSectionTitle.TextColor = MyTNBColor.PowerBlue;
            lblSectionTitle.Font = TNBFont.MuseoSans_16_500;
            lblSectionTitle.Lines = 0;
            lblSectionTitle.LineBreakMode = UILineBreakMode.WordWrap;
            view.AddSubview(lblSectionTitle);
            return view;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            CustomerAccountRecordModel linkedAcount = new CustomerAccountRecordModel();
            CustomerAccountRecordModel localAcount = new CustomerAccountRecordModel();
            CustomerAccountRecordModel acount = new CustomerAccountRecordModel();

            var cell = tableView.DequeueReusableCell("accountCell", indexPath) as AccountRecordsTableViewCell;
            cell.SeparatorView.BackgroundColor = MyTNBColor.SectionGrey;


            if (indexPath.Section == 0)
            {
                if (_linkedAccounts != null && _linkedAccounts?.Count > 0 && _localAccounts?.Count >= 0)
                {
                    linkedAcount = _linkedAccounts[indexPath.Row];
                    cell.AccountNumber = linkedAcount?.accNum ?? string.Empty;
                    cell.Address = linkedAcount?.accountStAddress ?? string.Empty;
                    cell.NicknameTextField.Text = linkedAcount?.accDesc ?? string.Empty;
                    cell.DeleteButton.RemoveTarget(null, null, UIControlEvent.TouchUpInside);

                    acount = linkedAcount;
                    cell.Tag = 0;
                }
                if (_localAccounts != null && _localAccounts?.Count > 0 && _linkedAccounts?.Count == 0)
                {
                    localAcount = _localAccounts[indexPath.Row];
                    cell.AccountNumber = localAcount?.accNum ?? string.Empty;
                    cell.Address = localAcount?.accountStAddress ?? string.Empty;
                    cell.NicknameTextField.Text = localAcount?.accountNickName ?? string.Empty;
                    cell.DeleteButton.RemoveTarget(null, null, UIControlEvent.TouchUpInside);

                    acount = localAcount;
                    cell.Tag = 1;
                }
            }

            if (indexPath.Section == 1)
            {
                if (_localAccounts != null && _localAccounts?.Count > 0)
                {
                    localAcount = _localAccounts[indexPath.Row];
                    cell.AccountNumber = localAcount?.accNum ?? string.Empty;
                    cell.Address = localAcount?.accountStAddress ?? string.Empty;
                    cell.NicknameTextField.Text = localAcount?.accDesc ?? string.Empty;
                    cell.DeleteButton.RemoveTarget(null, null, UIControlEvent.TouchUpInside);
                    acount = localAcount;
                }
                cell.Tag = 1;
            }

            cell.DeleteButton.TouchUpInside += (object sender, EventArgs e) =>
            {
                var nickName = acount?.accDesc ?? string.Empty;
                var accountNumber = acount?.accNum ?? string.Empty;
                var okCancelAlertController = UIAlertController.Create(GetI18NValue(AddAccountConstants.I18N_RemoveAcct)
                    , string.Format(GetI18NValue(AddAccountConstants.I18N_RemoveAcctMsg), nickName, accountNumber)
                    , UIAlertControllerStyle.Alert);
                okCancelAlertController.AddAction(UIAlertAction.Create(LanguageUtility.GetCommonI18NValue(AddAccountConstants.I18N_Ok), UIAlertActionStyle.Default, alert => RemoveAccount(acount)));
                okCancelAlertController.AddAction(UIAlertAction.Create(LanguageUtility.GetCommonI18NValue(AddAccountConstants.I18N_Cancel), UIAlertActionStyle.Cancel, alert => Debug.WriteLine("Cancel was clicked")));
                okCancelAlertController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                _controller?.PresentViewController(okCancelAlertController, true, null);
            };

            cell.NickNameTitle = LanguageUtility.GetCommonI18NValue(AddAccountConstants.I18N_AcctNickname).ToUpper();

            UITextField txtFieldNickname = cell.NicknameTextField;
            UILabel title = cell.NickNameTitleLabel;
            UILabel error = cell.NickNameErrorLabel;
            UIView line = cell.LineView;
            title.Hidden = string.IsNullOrEmpty(txtFieldNickname.Text)
                            && string.IsNullOrWhiteSpace(txtFieldNickname.Text);
            error.Hidden = true;
            line.BackgroundColor = MyTNBColor.PlatinumGrey;
            txtFieldNickname.TextColor = MyTNBColor.TunaGrey();
            error.Text = LanguageUtility.GetHintI18NValue(AddAccountConstants.I18N_HintNickname);
            error.TextColor = MyTNBColor.SilverChalice;
            SetTextField(txtFieldNickname, title, error, line, cell);

            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            cell.SetScale();
            return cell;
        }

        internal void RemoveAccount(CustomerAccountRecordModel theAccount)
        {
            if (theAccount != null)
            {
                if (DataManager.DataManager.SharedInstance.AccountsToBeAddedList != null
                && DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d != null
                && DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d?.Count > 0)
                {
                    DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d.Remove(theAccount);
                    _controller?.SetAccountTable();
                }
            }
        }

        internal void SetTextField(UITextField textField, UILabel title, UILabel error
                                   , UIView line, AccountRecordsTableViewCell cell)
        {
            _txtFieldHelper.CreateTextFieldLeftView(textField, LanguageUtility.GetCommonI18NValue(AddAccountConstants.I18N_Name));
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            textField.EditingChanged += (sender, e) =>
            {
                title.Hidden = textField.Text.Length == 0;
                error.Hidden = false;
                if (DataManager.DataManager.SharedInstance.AccountsToBeAddedList != null
                    && DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d != null
                    && DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d?.Count > 0)
                {
                    int index = DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d.FindIndex(x => x.accNum == cell.AccountNumber);
                    if (index > -1)
                    {
                        DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d[index].accountNickName = textField.Text;
                        DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d[index].accDesc = textField.Text;
                    }
                }
                if (cell.Tag == 1)
                {
                    int localIndex = (int)_localAccounts?.FindIndex(x => x.accNum == cell.AccountNumber);
                    if (localIndex > -1)
                    {
                        _localAccounts[localIndex].accDesc = textField.Text;
                    }
                }
                if (cell.Tag == 0)
                {
                    int linkedIndex = (int)_linkedAccounts?.FindIndex(x => x.accNum == cell.AccountNumber);
                    if (linkedIndex > -1)
                    {
                        _linkedAccounts[linkedIndex].accDesc = textField.Text;
                    }
                }
            };
            textField.ShouldEndEditing = (sender) =>
            {
                bool isFormatValid = !string.IsNullOrWhiteSpace(textField.Text) && _txtFieldHelper.ValidateTextField(textField.Text, TNBGlobal.ACCOUNT_NAME_PATTERN);
                bool isUnique = DataManager.DataManager.SharedInstance.IsAccountNicknameUnique(textField.Text, cell.AccountNumber);
                bool isValid = isFormatValid && isUnique;
                error.Hidden = isValid;
                error.Text = isValid ? LanguageUtility.GetHintI18NValue(AddAccountConstants.I18N_HintNickname)
                    : (!isFormatValid ? LanguageUtility.GetErrorI18NValue(AddAccountConstants.I18N_InvalidNickname) : LanguageUtility.GetErrorI18NValue(AddAccountConstants.I18N_DuplicateNickname));
                error.TextColor = isValid ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;
                line.BackgroundColor = isValid ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                textField.TextColor = isValid ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;
                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                title.Hidden = false;
                error.Hidden = false;
                line.BackgroundColor = MyTNBColor.PowerBlue;
            };
            textField.EditingDidEnd += (sender, e) =>
            {
                _controller?.UpdateControlStates();
            };
            textField.ShouldChangeCharacters = (txtField, range, replacementString) =>
            {
                if (!string.IsNullOrEmpty(replacementString))
                {
                    return _txtFieldHelper.ValidateTextField(replacementString, TNBGlobal.ACCOUNT_NAME_PATTERN);
                }
                return true;
            };
        }
    }
}