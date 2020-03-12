using System;
using System.Collections.Generic;
using System.Diagnostics;
using Force.DeepCloner;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class RearrangeDataSource : UITableViewSource
    {
        private List<CustomerAccountRecordModel> _accountList = new List<CustomerAccountRecordModel>();
        RearrangeAccountViewController _controller;

        public RearrangeDataSource(RearrangeAccountViewController controller)
        {
            _controller = controller;
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null)
            {
                _accountList = DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.DeepClone() ?? new List<CustomerAccountRecordModel>();
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

        public override nfloat EstimatedHeight(UITableView tableView, NSIndexPath indexPath)
        {
            return GetHeightForRow(tableView, indexPath);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return ScaleUtility.GetScaledHeight(61F);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            RearrangeCell cell = tableView.DequeueReusableCell(DashboardHomeConstants.Cell_RearrangeAccount) as RearrangeCell;
            CustomerAccountRecordModel item = _accountList[indexPath.Row];

            cell.lblAccountName.Text = item.accDesc;
            cell.ImageIcon = GetIcon(item);
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

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return UITableViewCellEditingStyle.None;
        }

        public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {
            var item = _accountList[sourceIndexPath.Row];
            var deleteAt = sourceIndexPath.Row;
            var insertAt = destinationIndexPath.Row;

            if (destinationIndexPath.Row == sourceIndexPath.Row) { return; }

            if (destinationIndexPath.Row < sourceIndexPath.Row)
            {
                deleteAt += 1;
            }
            else
            {
                insertAt += 1;
            }
            _accountList.Insert(insertAt, item);
            _accountList.RemoveAt(deleteAt);
            if (_controller != null)
            {
                _controller.RearrangeAction(_accountList);
            }
        }
    }
}
