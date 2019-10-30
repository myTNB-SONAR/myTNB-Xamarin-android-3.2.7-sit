using System;
using System.Collections.Generic;
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
            SuccessfullyAddedAccountCell cell = tableView.DequeueReusableCell("SuccessfullyAddedAccountCell", indexPath) as SuccessfullyAddedAccountCell;
            if (_GetStartedList?.d?.Count > 0)
            {
                CustomerAccountRecordModel account = indexPath.Row < _GetStartedList?.d?.Count
                    ? _GetStartedList?.d[indexPath.Row] : new CustomerAccountRecordModel();
                cell.NickNameLabel.TextColor = MyTNBColor.TunaGrey();
                cell.NickNameLabel.Font = TNBFont.MuseoSans_14_500;
                cell.AccountNumberLabel.TextColor = MyTNBColor.TunaGrey();
                cell.AccountNumberLabel.Font = TNBFont.MuseoSans_12_300;
                cell.AddressTextView.TextColor = MyTNBColor.TunaGrey();
                cell.AddressTextView.Font = TNBFont.MuseoSans_12_300;
                cell.NickNameLabel.Text = account.accountNickName != null ? account.accountNickName : string.Empty;
                cell.AccountNumberLabel.Text = account.accNum != null ? account.accNum : string.Empty;
                cell.AddressTextView.Text = account.accountStAddress != null ? account.accountStAddress : string.Empty;
            }
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _GetStartedList?.d?.Count ?? 0;
        }
    }
}