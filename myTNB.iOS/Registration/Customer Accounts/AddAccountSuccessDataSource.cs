using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using UIKit;
namespace myTNB.Registration
{
    public class AddAccountSuccessDataSource : UITableViewSource
    {
        private CustomerAccountRecordListModel _GetStartedList = new CustomerAccountRecordListModel();

        public AddAccountSuccessDataSource(CustomerAccountRecordListModel getStartedList)
        {
            if (getStartedList != null)
            {
                _GetStartedList = getStartedList;
            }
            else
            {
                _GetStartedList.d = new List<CustomerAccountRecordModel>();
            }
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            AddCASuccessCell cell = tableView.DequeueReusableCell("AddCASuccessCell", indexPath) as AddCASuccessCell;
            if (_GetStartedList != null && _GetStartedList.d != null && _GetStartedList.d.Count > 0)
            {
                CustomerAccountRecordModel account = indexPath.Row < _GetStartedList?.d?.Count
                     ? _GetStartedList?.d[indexPath.Row] : new CustomerAccountRecordModel();

                cell.Name = account.accountNickName != null ? account.accountNickName : string.Empty;
                cell.CANumber = account.accNum != null ? account.accNum : string.Empty;
                cell.Address = account.accountStAddress != null ? account.accountStAddress : string.Empty;
            }
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _GetStartedList?.d?.Count ?? 0;
        }
    }
}