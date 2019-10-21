using System;
using System.Collections.Generic;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class AccountListDataSource : UITableViewSource
    {
        private readonly List<DueAmountDataModel> _accountList = new List<DueAmountDataModel>();
        public Func<string, string> GetI18NValue;
        private readonly Action<DueAmountDataModel> OnRowSelected;
        private readonly Action OnAddAccountAction;
        private readonly bool _isLoading;
        private readonly bool _hasEmptyAcct;

        public AccountListDataSource(List<DueAmountDataModel> accountList,
            Func<string, string> getI18NValue,
            Action<DueAmountDataModel> onRowSelected,
            Action onAddAccountAction,
            bool isLoading = false,
            bool hasEmptyAcct = false)
        {
            _accountList = accountList;
            GetI18NValue = getI18NValue;
            OnRowSelected = onRowSelected;
            OnAddAccountAction = onAddAccountAction;
            _isLoading = isLoading;
            _hasEmptyAcct = hasEmptyAcct;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _hasEmptyAcct ? 1 : _accountList.Count;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return _isLoading ? DashboardHomeConstants.AccountCellHeight : _hasEmptyAcct ? DashboardHomeConstants.EmptyAcctHeight : DashboardHomeConstants.AccountCellHeight;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (_isLoading)
            {
                AccountListCell cell = tableView.DequeueReusableCell(DashboardHomeConstants.Cell_AccountList) as AccountListCell;
                var index = indexPath.Row;
                if (index > -1 && index < _accountList.Count)
                {
                    var acctCached = DataManager.DataManager.SharedInstance.GetDue(_accountList[index].accNum);
                    if (acctCached == null)
                    {
                        cell.SetShimmerCell();
                    }
                    else
                    {
                        cell.GetI18NValue = GetI18NValue;
                        cell.SetAccountCell(_accountList[index]);
                    }
                }
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                return cell;
            }
            else if (_hasEmptyAcct)
            {
                AccountListEmptyCell cell = tableView.DequeueReusableCell(DashboardHomeConstants.Cell_AccountListEmpty) as AccountListEmptyCell;
                cell.GetI18NValue = GetI18NValue;
                cell.SetEmptyCell();
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                return cell;
            }
            else
            {
                AccountListCell cell = tableView.DequeueReusableCell(DashboardHomeConstants.Cell_AccountList) as AccountListCell;
                var index = indexPath.Row;
                if (index > -1 && index < _accountList.Count)
                {
                    cell.GetI18NValue = GetI18NValue;
                    cell.SetAccountCell(_accountList[index]);
                }
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                return cell;
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (_hasEmptyAcct)
            {
                OnAddAccountAction?.Invoke();
            }
            else if (!_isLoading)
            {
                var index = indexPath.Row;
                if (index > -1 && index < _accountList.Count)
                {
                    var acct = _accountList[index];
                    OnRowSelected?.Invoke(acct);
                }
            }
        }
    }
}
