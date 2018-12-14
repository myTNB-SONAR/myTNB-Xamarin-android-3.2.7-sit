using System;
using System.Collections.Generic;
using Foundation;
using myTNB.Model;
using UIKit;
namespace myTNB.Registration
{
    public class AddAccountSuccessDataSource: UITableViewSource
    {
        CustomerAccountRecordListModel GetStartedList = new CustomerAccountRecordListModel();

        public AddAccountSuccessDataSource()
        {
            Console.WriteLine("test");
            GetStartedList.d = new List<CustomerAccountRecordModel>();
            if(DataManager.DataManager.SharedInstance.AccountRecordsList != null
               && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null
               && DataManager.DataManager.SharedInstance.AccountRecordsList.d.Count > 0
               && DataManager.DataManager.SharedInstance.AccountsToBeAddedList != null
               && DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d != null){
                GetStartedList.d = new List<CustomerAccountRecordModel>();
                foreach(var item in DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d){
                    int index = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindIndex(x => x.accNum == item.accNum);
                    if(index == -1){
                        GetStartedList.d.Add(item);
                    }
                }
            }else{
                if(DataManager.DataManager.SharedInstance.AccountsToBeAddedList != null
                   && DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d != null){
                    GetStartedList = DataManager.DataManager.SharedInstance.AccountsToBeAddedList;
                }
            }
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("SuccessfullyAddedAccountCell", indexPath) as SuccessfullyAddedAccountCell;
            if (GetStartedList != null && GetStartedList.d != null && GetStartedList.d.Count != 0) {
                CustomerAccountRecordModel account = new CustomerAccountRecordModel();
                account = GetStartedList.d[indexPath.Row];
                cell.NickNameLabel.Text = account.accountNickName != null ? account.accountNickName : "";
                cell.AccountNumberLabel.Text = account.accNum != null ? account.accNum : "";
                cell.AddressTextView.Text = account.accountStAddress != null ? account.accountStAddress : "";
            }
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return GetStartedList.d.Count;
        }
    }
}