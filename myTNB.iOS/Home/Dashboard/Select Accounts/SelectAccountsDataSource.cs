using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB.Dashboard.SelectAccounts
{
    public class SelectAccountsDataSource : UITableViewSource
    {
        private List<CustomerAccountRecordModel> _accountList = new List<CustomerAccountRecordModel>();
        private SelectAccountTableViewController _controller;

        public SelectAccountsDataSource(SelectAccountTableViewController controller)
        {
            _controller = controller;
            if (_controller.IsFromSSMR)
            {
                _accountList = SSMRAccounts.GetEligibleAccountList();
            }
            else
            {
                if (DataManager.DataManager.SharedInstance.AccountRecordsList != null && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null)
                {
                    _accountList = DataManager.DataManager.SharedInstance.AccountRecordsList?.d ?? new List<CustomerAccountRecordModel>();
                }
            }
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _accountList != null ? _accountList.Count : 0;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("AccountsViewCell", indexPath) as AccountsViewCell;
            CustomerAccountRecordModel item = _accountList[indexPath.Row];

            cell.lblAccountName.Text = item.accDesc;
            cell.ImageIcon = GetIcon(item);

            bool isSameRow = _controller.IsFromSSMR ? indexPath.Row == _controller.CurrentSelectedIndex
                : indexPath.Row == DataManager.DataManager.SharedInstance.CurrentSelectedAccountIndex;

            if (isSameRow && item.accNum == _accountList[_controller.IsFromSSMR
                ? _controller.CurrentSelectedIndex : DataManager.DataManager.SharedInstance.CurrentSelectedAccountIndex].accNum)
            {
                nfloat iconWidth = ScaleUtility.GetScaledWidth(24);
                cell.Accessory = UITableViewCellAccessory.None;
                cell.AccessoryView = new UIView(new CGRect(0, 0, iconWidth, iconWidth));
                UIImageView imgViewTick = new UIImageView(new CGRect(0, 0, iconWidth, iconWidth))
                {
                    Image = UIImage.FromBundle("Table-Tick")
                };
                cell.AccessoryView.AddSubview(imgViewTick);
            }
            else
            {
                if (cell != null && cell.AccessoryView != null && cell.AccessoryView.Subviews != null)
                {
                    foreach (var subView in cell.AccessoryView.Subviews)
                    {
                        subView.RemoveFromSuperview();
                    }
                }
            }
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }

        private string GetIcon(CustomerAccountRecordModel account)
        {
            string iconName = string.Empty;
            if (account.IsREAccount)
            {
                iconName = DashboardHomeConstants.Img_RELeaf;
            }
            return iconName;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (_controller.IsFromSSMR)
            {
                if (_controller.OnSelect != null)
                {
                    _controller?.OnSelect(indexPath.Row);
                }
                _controller.NavigationController.PopViewController(true);
                return;
            }

            if (_controller.IsFromHome)
            {
                if (indexPath.Row < _accountList.Count)
                {
                    var selected = _accountList[indexPath.Row];
                    DataManager.DataManager.SharedInstance.IsSameAccount = DataManager.DataManager.SharedInstance.GetAccountsCount() > 1
                        && string.Compare(selected.accNum, DataManager.DataManager.SharedInstance.SelectedAccount?.accNum) == 0;
                    DataManager.DataManager.SharedInstance.SelectAccount(selected.accNum);
                    _controller.ShowBillScreen(indexPath.Row);
                }
                return;
            }

            if (indexPath.Row < _accountList.Count)
            {
                var selected = _accountList[indexPath.Row];
                DataManager.DataManager.SharedInstance.IsSameAccount = DataManager.DataManager.SharedInstance.GetAccountsCount() > 1
                    && string.Compare(selected.accNum, DataManager.DataManager.SharedInstance.SelectedAccount?.accNum) == 0;
                DataManager.DataManager.SharedInstance.SelectAccount(selected.accNum);
                if (_controller.OnSelect != null)
                {
                    _controller?.OnSelect(indexPath.Row);
                }
                if (_controller.IsFromUsage)
                {
                    _controller.DismissViewController(true, null);
                    return;
                }
                _controller.DismissViewController(true, null);
            }
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return ScaleUtility.GetScaledHeight(61);
        }
    }
}