using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB.Registration.CustomerAccounts
{
    public class AccountRecordDataSource : UITableViewSource
    {
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
            if (accountList != null && accountList.d != null)
            {
                _accountList = accountList;
            }

            if (DataManager.DataManager.SharedInstance.AccountsToBeAddedList != null
                && DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d != null)
            {
                foreach (var account in DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d)
                {
                    if (account.isLocal == false)
                    {
                        _linkedAccounts.Add(account);
                    }
                    else
                    {
                        _localAccounts.Add(account);
                    }
                }

                if (_linkedAccounts.Count > 0 && _localAccounts.Count > 0)
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
            _recordCount = _linkedAccounts != null
                     ? _linkedAccounts.Count : 0;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return _sectionCount;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (_linkedAccounts.Count == 0 && _localAccounts.Count == 0)
            {
                if (section == 0)
                {
                    return _linkedAccounts != null ? _linkedAccounts.Count : 0;
                }
            }
            else if (_linkedAccounts.Count > 0 && _localAccounts.Count > 0)
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
            else if (_linkedAccounts.Count == 0 && _localAccounts.Count > 0)
            {
                if (section == 0)
                {
                    return _localAccounts != null ? _localAccounts.Count : 0;
                }
            }
            else if (_linkedAccounts.Count > 0 && _localAccounts.Count == 0)
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
                if ((_linkedAccounts != null && _linkedAccounts.Count > 0 && _localAccounts.Count >= 0) || (_linkedAccounts != null && _linkedAccounts.Count == 0 && _localAccounts.Count == 0))
                {
                    return 92;
                }

                if (_localAccounts != null && _localAccounts.Count > 0 && _linkedAccounts.Count == 0)
                {
                    return 30;
                }
            }

            if (section == 1)
            {
                if (_localAccounts != null && _localAccounts.Count > 0)
                {
                    return 30;
                }
            }
            return 30;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 175;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            UIView view = new UIView(new CGRect(0, 0, tableView.Frame.Width, 100));
            view.BackgroundColor = myTNBColor.SectionGrey();

            var lblSectionTitle = new UILabel(new CGRect(18, 0, tableView.Frame.Width - 36, 40));

            if (section == 0)
            {
                if ((_linkedAccounts != null && _linkedAccounts.Count > 0 && _localAccounts.Count >= 0)
                    || (_linkedAccounts != null && _linkedAccounts.Count == 0 && _localAccounts.Count == 0))
                {
                    lblSectionTitle = new UILabel(new CGRect(18, 0, tableView.Frame.Width - 36, 40));

                    lblSectionTitle.Text = _recordCount > 0
                        ? _recordCount.ToString() + " electricity supply account(s) found!"
                        : "No records found. Please add an account.";

                    var txtViewSubDetails = new UITextView(new CGRect(14, 36, tableView.Frame.Width - 30, 60));
                    txtViewSubDetails.Font = myTNBFont.MuseoSans14();
                    txtViewSubDetails.TextColor = myTNBColor.TunaGrey();
                    txtViewSubDetails.UserInteractionEnabled = false;
                    txtViewSubDetails.BackgroundColor = UIColor.Clear;
                    txtViewSubDetails.Text = "Give your accounts nicknames for your easy reference";

                    view.AddSubview(txtViewSubDetails);

                }

                if (_localAccounts != null && _localAccounts.Count > 0 && _linkedAccounts.Count == 0)
                {
                    lblSectionTitle = new UILabel(new CGRect(18, 0, tableView.Frame.Width - 36, 40));
                    lblSectionTitle.Text = "Additonal account(s)";
                }
            }

            if (section == 1)
            {
                if (_localAccounts != null && _localAccounts.Count > 0)
                {
                    lblSectionTitle = new UILabel(new CGRect(18, 0, tableView.Frame.Width - 36, 40));
                    lblSectionTitle.Text = "Additonal account(s)";
                }
            }

            lblSectionTitle.TextColor = myTNBColor.PowerBlue();
            lblSectionTitle.Font = myTNBFont.MuseoSans16();
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
            cell.SeparatorView.BackgroundColor = myTNBColor.SectionGrey();


            if (indexPath.Section == 0)
            {
                if (_linkedAccounts != null && _linkedAccounts.Count > 0 && _localAccounts.Count >= 0)
                {
                    linkedAcount = _linkedAccounts[indexPath.Row];
                    cell.AccountNumber = linkedAcount.accNum != null ? linkedAcount.accNum : "";
                    cell.Address = linkedAcount.accountStAddress != null ? linkedAcount.accountStAddress : "";
                    cell.NicknameTextField.Text = linkedAcount.accDesc != null ? linkedAcount.accDesc : "";
                    cell.DeleteButton.RemoveTarget(null, null, UIControlEvent.TouchUpInside);

                    acount = linkedAcount;
                    cell.Tag = 0;
                }
                if (_localAccounts != null && _localAccounts.Count > 0 && _linkedAccounts.Count == 0)
                {
                    localAcount = _localAccounts[indexPath.Row];
                    cell.AccountNumber = localAcount.accNum != null ? localAcount.accNum : "";
                    cell.Address = localAcount.accountStAddress != null ? localAcount.accountStAddress : "";
                    cell.NicknameTextField.Text = localAcount.accountNickName != null ? localAcount.accountNickName : "";
                    cell.DeleteButton.RemoveTarget(null, null, UIControlEvent.TouchUpInside);

                    acount = localAcount;
                    cell.Tag = 1;
                }
            }

            if (indexPath.Section == 1)
            {
                if (_localAccounts != null && _localAccounts.Count > 0)
                {
                    localAcount = _localAccounts[indexPath.Row];
                    cell.AccountNumber = localAcount.accNum != null ? localAcount.accNum : "";
                    cell.Address = localAcount.accountStAddress != null ? localAcount.accountStAddress : "";
                    cell.NicknameTextField.Text = localAcount.accDesc != null ? localAcount.accDesc : "";
                    cell.DeleteButton.RemoveTarget(null, null, UIControlEvent.TouchUpInside);
                    acount = localAcount;
                }
                cell.Tag = 1;
            }

            cell.DeleteButton.TouchUpInside += (object sender, EventArgs e) =>
            {
                var nickName = acount.accDesc != null ? acount.accDesc : "";
                var accountNumber = acount.accNum != null ? acount.accNum : "";
                var alertMessage = "You are about to remove " + nickName + ", " + "account no. " + accountNumber;

                var okCancelAlertController = UIAlertController.Create("Remove Account", alertMessage, UIAlertControllerStyle.Alert);
                okCancelAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, alert => RemoveAccount(acount)));
                okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));
                _controller.PresentViewController(okCancelAlertController, true, null);

            };

            cell.NickNameTitle = "ACCOUNT NICKNAME";
            cell.NicknameError = "Invalid characters. Use letters or numbers only.";

            UITextField txtFieldNickname = cell.NicknameTextField;
            UILabel title = cell.NickNameTitleLabel;
            UILabel error = cell.NickNameErrorLabel;
            UIView line = cell.LineView;
            title.Hidden = string.IsNullOrEmpty(txtFieldNickname.Text)
                            && string.IsNullOrWhiteSpace(txtFieldNickname.Text);
            error.Hidden = true;
            line.BackgroundColor = myTNBColor.PlatinumGrey();
            txtFieldNickname.TextColor = myTNBColor.TunaGrey();
            error.Text = "e.g. My House, Parent's House";
            error.TextColor = myTNBColor.SilverChalice();
            SetTextField(txtFieldNickname, title, error, line, cell);

            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            return cell;
        }

        internal void RemoveAccount(CustomerAccountRecordModel theAccount)
        {
            DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d.Remove(theAccount);
            _controller.SetAccountTable();
        }

        internal void SetTextField(UITextField textField, UILabel title, UILabel error
                                   , UIView line, AccountRecordsTableViewCell cell)
        {
            _txtFieldHelper.CreateTextFieldLeftView(textField, "Name");
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            textField.EditingChanged += (sender, e) =>
            {
                title.Hidden = textField.Text.Length == 0;
                error.Hidden = false;
                int index = DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d.FindIndex(x => x.accNum == cell.AccountNumber);
                if (index > -1)
                {
                    DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d[index].accountNickName = textField.Text;
                    DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d[index].accDesc = textField.Text;
                }
                if (cell.Tag == 1)
                {
                    int localIndex = _localAccounts.FindIndex(x => x.accNum == cell.AccountNumber);
                    if (localIndex > -1)
                    {
                        _localAccounts[localIndex].accDesc = textField.Text;
                    }
                }
                if (cell.Tag == 0)
                {
                    int linkedIndex = _linkedAccounts.FindIndex(x => x.accNum == cell.AccountNumber);
                    if (linkedIndex > -1)
                    {
                        _linkedAccounts[linkedIndex].accDesc = textField.Text;
                    }
                }
            };
            textField.ShouldEndEditing = (sender) =>
            {
                bool isValid = _txtFieldHelper.ValidateTextField(textField.Text, NAME_PATTERN);
                error.Hidden = isValid || textField.Text.Length == 0;
                error.Text = isValid || textField.Text.Length == 0
                    ? "e.g. My House, Parent's House"
                    : "Invalid characters. Use letters or numbers only.";
                error.TextColor = isValid || textField.Text.Length == 0
                    ? myTNBColor.TunaGrey()
                    : myTNBColor.Tomato();
                line.BackgroundColor = isValid || textField.Text.Length == 0 ? myTNBColor.PlatinumGrey() : myTNBColor.Tomato();
                textField.TextColor = isValid || textField.Text.Length == 0 ? myTNBColor.TunaGrey() : myTNBColor.Tomato();
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
            };
        }
    }
}