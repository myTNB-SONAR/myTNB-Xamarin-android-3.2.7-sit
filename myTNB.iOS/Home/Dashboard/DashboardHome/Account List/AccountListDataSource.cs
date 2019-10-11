using System;
using System.Collections.Generic;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class AccountListDataSource : UITableViewSource
    {
        List<DueAmountDataModel> _accountList;
        public Func<string, string> GetI18NValue;
        public AccountListDataSource(List<DueAmountDataModel> accountList, Func<string, string> getI18NValue)
        {
            _accountList = accountList;
            GetI18NValue = getI18NValue;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _accountList.Count;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return ScaleUtility.GetScaledHeight(61F);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            AccountListCell cell = tableView.DequeueReusableCell(DashboardHomeConstants.Cell_AccountList) as AccountListCell;
            var index = indexPath.Row;
            if (index > -1 && index < _accountList.Count)
            {
                cell.GetI18NValue = GetI18NValue;
                cell.SetModel(_accountList[indexPath.Row]);
            }
            return cell;
        }
    }
}
