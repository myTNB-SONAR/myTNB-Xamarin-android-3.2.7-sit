using System;
using System.Collections.Generic;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.API;
using myTNB_Android.Src.myTNBMenu.Models;
using static myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.API.AccountBillPayHistoryResponse;
using myTNB_Android.Src.Utils;
using System.Threading;
using myTNB_Android.Src.AddAccount.Models;
using static myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.API.AccountChargesResponse;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP
{
    public class ItemisedBillingMenuPresenter
    {
        ItemisedBillingAPIImpl api;
        ItemisedBillingContract.IView mView;
        AccountChargesModel mAccountChargesModel;


        public ItemisedBillingMenuPresenter(ItemisedBillingContract.IView view)
        {
            mView = view;
            api = new ItemisedBillingAPIImpl();
        }

        public async void GetAccountsCharges(string contractAccountValue, bool isOwnedAccountValue)
        {
            List<string> accountList = new List<string>();
            accountList.Add(contractAccountValue);
            AccountsChargesRequest request = new AccountsChargesRequest(
                accountList,
                isOwnedAccountValue
                );
            AccountChargesResponse response = await api.GetAccountsCharges(request);
            if (response.Data != null && response.Data.ErrorCode == "7200")
            {
                List<AccountChargeModel> accountChargeModelList = GetAccountCharges(response.Data.ResponseData.AccountCharges);
                mView.PopulateAccountCharge(accountChargeModelList);
            }
        }

        public async void GetAccountBillPayHistory(string contractAccountValue, bool isOwnedAccountValue, string accountTypeValue)
        {
            AccountBillPayHistoryRequest request = new AccountBillPayHistoryRequest(
                contractAccountValue,
                isOwnedAccountValue,
                accountTypeValue);

            AccountBillPayHistoryResponse response = await api.GetAccountBillPayHistory(request);
            if (response.Data != null && response.Data.ErrorCode == "7200")
            {
                List<ItemisedBillingHistoryModel> billingHistoryList = GetBillingHistoryModelList(response.Data.ResponseData.BillPayHistories);
                mView.PopulateBillingHistoryList(billingHistoryList);
            }
        }

        private List<AccountChargeModel> GetAccountCharges(List<AccountCharge> accountCharges)
        {
            List<AccountChargeModel> accountChargeModelList = new List<AccountChargeModel>();
            accountCharges.ForEach(accountCharge =>
            {
                MandatoryCharge mandatoryCharge = accountCharge.MandatoryCharges;
                List<ChargeModel> chargeModelList = new List<ChargeModel>();
                mandatoryCharge.Charges.ForEach(charge =>
                {
                    ChargeModel chargeModel = new ChargeModel();
                    chargeModel.Key = charge.Key;
                    chargeModel.Title = charge.Title;
                    chargeModel.Amount = charge.Amount;
                    chargeModelList.Add(chargeModel);
                });
                MandatoryChargeModel mandatoryChargeModel = new MandatoryChargeModel();
                mandatoryChargeModel.TotalAmount = mandatoryCharge.TotalAmount;
                mandatoryChargeModel.ChargeModelList = chargeModelList;

                AccountChargeModel accountChargeModel = new AccountChargeModel();
                accountChargeModel.IsCleared = false;
                accountChargeModel.IsNeedPay = false;
                accountChargeModel.IsPaidExtra = false;
                accountChargeModel.ContractAccount = accountCharge.ContractAccount;
                accountChargeModel.CurrentCharges = accountCharge.CurrentCharges;
                accountChargeModel.OutstandingCharges = accountCharge.OutstandingCharges;
                accountChargeModel.AmountDue = accountCharge.AmountDue;
                accountChargeModel.DueDate = accountCharge.DueDate;
                accountChargeModel.BillDate = accountCharge.BillDate;
                accountChargeModel.IncrementREDueDateByDays = accountCharge.IncrementREDueDateByDays;
                accountChargeModel.MandatoryCharges = mandatoryChargeModel;
                EvaluateAmountDue(accountChargeModel);
                accountChargeModelList.Add(accountChargeModel);
            });
            return accountChargeModelList;
        }

        private List<ItemisedBillingHistoryModel> GetBillingHistoryModelList(List<BillPayHistory> billPayHistoryList)
        {
            List<ItemisedBillingHistoryModel> modelList = new List<ItemisedBillingHistoryModel>();
            List<ItemisedBillingHistoryModel.BillingHistoryData> dataList;
            ItemisedBillingHistoryModel.BillingHistoryData data;
            ItemisedBillingHistoryModel model;
            billPayHistoryList.ForEach(history =>
            {
                dataList = new List<ItemisedBillingHistoryModel.BillingHistoryData>();
                model = new ItemisedBillingHistoryModel();
                model.MonthYear = history.MonthYear;

                history.BillPayHistoryData.ForEach(historyData =>
                {
                    data = new ItemisedBillingHistoryModel.BillingHistoryData();
                    data.HistoryType = historyData.HistoryType;
                    data.DateAndHistoryType = historyData.DateAndHistoryType;
                    data.Amount = historyData.Amount;
                    data.DetailedInfoNumber = historyData.DetailedInfoNumber;
                    data.PaidVia = historyData.PaidVia;
                    data.HistoryTypeText = historyData.HistoryTypeText;
                    dataList.Add(data);
                });

                model.BillingHistoryDataList = dataList;
                modelList.Add(model);
            });

            return modelList;
        }

        public void EvaluateAmountDue(AccountChargeModel accountChargeModel)
        {
            if (accountChargeModel.AmountDue > 0f)
            {
                accountChargeModel.IsNeedPay = true;
            }

            if (accountChargeModel.AmountDue < 0f)
            {
                accountChargeModel.IsPaidExtra = true;
            }

            if (accountChargeModel.AmountDue == 0f)
            {
                accountChargeModel.IsCleared = true;
            }
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

            data = new ItemisedBillingHistoryModel.BillingHistoryData();

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

            dataList = new List<ItemisedBillingHistoryModel.BillingHistoryData>();
            data = new ItemisedBillingHistoryModel.BillingHistoryData();

            data.HistoryType = "PAYMENT";
            data.DateAndHistoryType = "03 May - Payment";
            data.Amount = "25.00";
            data.DetailedInfoNumber = "000530477074";
            data.PaidVia = "26 Feb 2019";
            dataList.Add(data);

            data = new ItemisedBillingHistoryModel.BillingHistoryData();

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
