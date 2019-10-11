using System;
using System.Collections.Generic;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class AccountListDataSource : UITableViewSource
    {
        private List<DueAmountDataModel> _accountList;
        public Func<string, string> GetI18NValue;
        private Action<DueAmountDataModel> OnRowSelected;
        private Action OnAddAccountAction;
        private bool _hasEmptyAcct;

        public AccountListDataSource(List<DueAmountDataModel> accountList,
            Func<string, string> getI18NValue,
            Action<DueAmountDataModel> onRowSelected,
            Action onAddAccountAction,
            bool hasEmptyAcct = false)
        {
            _accountList = accountList;
            GetI18NValue = getI18NValue;
            OnRowSelected = onRowSelected;
            OnAddAccountAction = onAddAccountAction;
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
            return _hasEmptyAcct ? ScaleUtility.GetScaledHeight(101F) : ScaleUtility.GetScaledHeight(61F);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (_hasEmptyAcct)
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
                    cell.SetAccountCell(_accountList[indexPath.Row]);
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
            else
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
