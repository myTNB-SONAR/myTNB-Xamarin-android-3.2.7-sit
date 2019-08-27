using System;
using System.Collections.Generic;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP
{
    public class ItemisedBillingMenuPresenter
    {
        public ItemisedBillingMenuPresenter()
        {
        }

        public int GetBillImageHeader(AccountData selectedAccount)
        {
            return Resource.Drawable.bill_no_outstanding_banner;
        }

        public bool IsEnableAccountSelection()
        {
            List<CustomerBillingAccount> accountList = CustomerBillingAccount.List();
            return accountList.Count > 0 ? true : false;
        }

        public bool IsREAccount(string accountCategoryId)
        {
            return accountCategoryId.Equals("2");
        }

        public List<ItemisedBillingHistoryModel> GetBillingHistoryList()
        {
            List<ItemisedBillingHistoryModel> modelList = new List<ItemisedBillingHistoryModel>();

            ItemisedBillingHistoryModel model = new ItemisedBillingHistoryModel();
            model.MonthYear = "Feb 2019";

            List<ItemisedBillingHistoryModel.BillingHistoryData> dataList = new List<ItemisedBillingHistoryModel.BillingHistoryData>();
            ItemisedBillingHistoryModel.BillingHistoryData data = new ItemisedBillingHistoryModel.BillingHistoryData();
            data.HistoryType = "BILL";
            data.DateAndHistoryType = "24 Feb - Bill";
            data.Amount = "257.50";
            data.DetailedInfoNumber = "000530477074";
            data.PaidVia = "24 Feb 2019";
            dataList.Add(data);

            data.HistoryType = "PAYMENT";
            data.DateAndHistoryType = "05 Mar - Payment";
            data.Amount = "257.00";
            data.DetailedInfoNumber = "";
            data.PaidVia = "via CIMB BATCH";
            dataList.Add(data);


            model.BillingHistoryDataList = dataList;
            modelList.Add(model);

            model = new ItemisedBillingHistoryModel();
            model.MonthYear = "Mar 2019";

            data.HistoryType = "PAYMENT";
            data.DateAndHistoryType = "03 May - Payment";
            data.Amount = "25.00";
            data.DetailedInfoNumber = "000530477074";
            data.PaidVia = "26 Feb 2019";
            dataList.Add(data);

            data.HistoryType = "BILL";
            data.DateAndHistoryType = "2 Jun - Bill";
            data.Amount = "57.50";
            data.DetailedInfoNumber = "000530477074";
            data.PaidVia = "24 Mar 2019";
            dataList.Add(data);


            model.BillingHistoryDataList = dataList;
            modelList.Add(model);


            return modelList;
        }
    }
}
