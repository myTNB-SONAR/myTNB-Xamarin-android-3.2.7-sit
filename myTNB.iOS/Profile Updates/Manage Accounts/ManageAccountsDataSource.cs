using System;
using CoreGraphics;
using Foundation;
using myTNB.DataManager;
using myTNB.Model;
using myTNB.MyAccount;
using UIKit;

namespace myTNB.Home.More.MyAccount.ManageAccounts
{
    public class ManageAccountsDataSource : UITableViewSource
    {
        ManageAccountsViewController _controller;
        CustomerAccountRecordModel _accountRecord = new CustomerAccountRecordModel();

        public ManageAccountsDataSource(ManageAccountsViewController controller, CustomerAccountRecordModel accountRecord)
        {
            _controller = controller;
            if (accountRecord != null)
            {
                _accountRecord = accountRecord;
            }
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return 3;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row == 0)
            {
                DetailsViewCell cell = tableView.DequeueReusableCell(MyAccountConstants.Cell_Details, indexPath) as DetailsViewCell;
                cell.Frame = new CGRect(cell.Frame.X, cell.Frame.Y, tableView.Frame.Width, 82);
                cell.lblAccountNumber.Text = ServiceCall.ValidateResponseItem(_accountRecord.accNum);
                cell.lblAddress.Text = ServiceCall.ValidateResponseItem(_accountRecord.accountStAddress);
                return cell;
            }
            else if (indexPath.Row == 1)
            {
                UpdateViewCell cell = tableView.DequeueReusableCell(MyAccountConstants.Cell_Update, indexPath) as UpdateViewCell;
                cell.Frame = new CGRect(cell.Frame.X, cell.Frame.Y, tableView.Frame.Width, 62);
                cell.lblTitle.Text = GetCommonI18NValue(Constants.Common_AccountNickname).ToUpper();
                cell.lblDetail.Text = ServiceCall.ValidateResponseItem(_accountRecord.accDesc);
                cell.lblCTA.Text = GetCommonI18NValue(Constants.Common_Update);
                cell.viewCTA.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    _controller.UpdateNickName();
                }));
                return cell;
            }
            else if (indexPath.Row == 2)
            {
                RemoveViewCell cell = tableView.DequeueReusableCell(MyAccountConstants.Cell_Remove, indexPath) as RemoveViewCell;
                cell.Frame = new CGRect(cell.Frame.X, cell.Frame.Y, tableView.Frame.Width, DeviceHelper.GetScaledHeight(88));
                cell.btnRemove.TouchUpInside += (sender, e) =>
                {
                    _controller.OnRemoveAccount();
                };
                cell.btnRemove.SetTitle(GetI18NValue(MyAccountConstants.I18N_RemoveAccount), UIControlState.Normal);
                return cell;
            }
            return new UITableViewCell();
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            nfloat rowHeight = 80;
            if (indexPath.Row == 0)
            {
                rowHeight = 82;
            }
            else if (indexPath.Section == 1)
            {
                rowHeight = 62;
            }
            else if (indexPath.Section == 2)
            {
                rowHeight = DeviceHelper.GetScaledHeight(88);
            }
            return rowHeight;
        }

        private string GetI18NValue(string key)
        {
            return _controller.GetI18NValue(key);
        }

        private string GetCommonI18NValue(string key)
        {
            return _controller.GetCommonI18NValue(key);
        }
    }
}