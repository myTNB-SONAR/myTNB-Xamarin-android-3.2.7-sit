using System;
using System.Collections.Generic;
using Foundation;
using myTNB.Model;
using UIKit;
namespace myTNB.Registration
{
    public class AddAccountSuccessDataSource : UITableViewSource
    {
        CustomerAccountRecordListModel _GetStartedList = new CustomerAccountRecordListModel();

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
            var cell = tableView.DequeueReusableCell("SuccessfullyAddedAccountCell", indexPath) as SuccessfullyAddedAccountCell;
            if (_GetStartedList?.d?.Count > 0)
            {
                CustomerAccountRecordModel account = indexPath.Row < _GetStartedList?.d?.Count
                                                              ? _GetStartedList?.d[indexPath.Row]
                                                              : new CustomerAccountRecordModel();
                cell.NickNameLabel.TextColor = myTNBColor.TunaGrey();
                cell.NickNameLabel.Font = myTNBFont.MuseoSans14_500();
                cell.AccountNumberLabel.TextColor = myTNBColor.TunaGrey();
                cell.AccountNumberLabel.Font = myTNBFont.MuseoSans12_300();
                cell.AddressTextView.TextColor = myTNBColor.TunaGrey();
                cell.AddressTextView.Font = myTNBFont.MuseoSans12_300();
                cell.NickNameLabel.Text = account.accountNickName != null ? account.accountNickName : "";
                cell.AccountNumberLabel.Text = account.accNum != null ? account.accNum : "";
                cell.AddressTextView.Text = account.accountStAddress != null ? account.accountStAddress : "";
            }
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _GetStartedList?.d?.Count ?? 0;
        }
    }
}