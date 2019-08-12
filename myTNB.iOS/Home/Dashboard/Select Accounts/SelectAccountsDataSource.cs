using System;
using System.Collections.Generic;
using System.Drawing;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB.Dashboard.SelectAccounts
{
    public class SelectAccountsDataSource : UITableViewSource
    {
        List<CustomerAccountRecordModel> _accountList = new List<CustomerAccountRecordModel>();
        SelectAccountTableViewController _controller;
        DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();
        public SelectAccountsDataSource(SelectAccountTableViewController controller)
        {
            _controller = controller;
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null)
            {
                _accountList = DataManager.DataManager.SharedInstance.AccountRecordsList?.d ?? new List<CustomerAccountRecordModel>();
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
            CustomerAccountRecordModel item = new CustomerAccountRecordModel();
            item = _accountList[indexPath.Row];
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            var cell = tableView.DequeueReusableCell("AccountsViewCell", indexPath) as AccountsViewCell;

            cell.lblAccountName.Text = item.accDesc;
            CGSize newLabelSize = GetLabelSize(cell.lblAccountName, cellWidth - 96, 24);
            cell.lblAccountName.Frame = new CGRect(18, 16, newLabelSize.Width, 24);

            cell.imgLeaf.Frame = new CGRect(18 + cell.lblAccountName.Frame.Width + 6, 16, 24, 24);
            bool isREAccount = item.accountCategoryId != null && item.accountCategoryId.Equals("2");
            cell.imgLeaf.Hidden = !isREAccount;

            if (indexPath.Row == DataManager.DataManager.SharedInstance.CurrentSelectedAccountIndex
               && item.accNum == _accountList[DataManager.DataManager.SharedInstance.CurrentSelectedAccountIndex].accNum)
            {
                cell.Accessory = UITableViewCellAccessory.None;
                cell.AccessoryView = new UIView(new CGRect(0, 0, 24, 24));
                UIImageView imgViewTick = new UIImageView(new CGRect(0, 0, 24, 24))
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
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var selected = _accountList[indexPath.Row];
            DataManager.DataManager.SharedInstance.IsSameAccount = DataManager.DataManager.SharedInstance.GetAccountsCount() > 1
                && string.Compare(selected.accNum, DataManager.DataManager.SharedInstance.SelectedAccount?.accNum) == 0;
            DataManager.DataManager.SharedInstance.SelectAccount(selected.accNum);
            _controller.DismissViewController(true, null);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 57F;
        }

        CGSize GetLabelSize(UILabel label, nfloat width, nfloat height)
        {
            return label.Text.StringSize(label.Font, new SizeF((float)width, (float)height));
        }
    }
}