using System;
using System.Collections.Generic;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using System.Threading;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Services;
using myTNB_Android.Src.SiteCore;
using myTNB.SitecoreCMS.Model;
using Android.App;
using Newtonsoft.Json;
using myTNB_Android.Src.MyTNBService.Billing;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using static myTNB_Android.Src.MyTNBService.Response.AccountChargesResponse;
using static myTNB_Android.Src.MyTNBService.Response.AccountBillPayHistoryResponse;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP
{
    public class ItemisedBillingMenuPresenter
    {
        BillingApiImpl api;
        ItemisedBillingContract.IView mView;
        AccountChargesModel mAccountChargesModel;


        public ItemisedBillingMenuPresenter(ItemisedBillingContract.IView view)
        {
            mView = view;
            api = new BillingApiImpl();
        }

        public async void GetBillingHistoryDetails(string contractAccountValue, bool isOwnedAccountValue, string accountTypeValue)
        {
            //Get Account Charges Service Call
            bool showRefreshState = false;
            List<string> accountList = new List<string>();
            List<AccountChargeModel> accountChargeModelList = new List<AccountChargeModel>();
            List<AccountBillPayHistoryModel> billingHistoryList = new List<AccountBillPayHistoryModel>();
            accountList.Add(contractAccountValue);
            AccountsChargesRequest accountChargeseRequest = new AccountsChargesRequest(
                accountList,
                isOwnedAccountValue
                );
            AccountChargesResponse accountChargeseResponse = await api.GetAccountsCharges<AccountChargesResponse>(accountChargeseRequest);
            if (accountChargeseResponse.Data != null && accountChargeseResponse.Data.ErrorCode == "7200")
            {
                accountChargeModelList = GetAccountCharges(accountChargeseResponse.Data.ResponseData.AccountCharges);
                MyTNBAppToolTipData.GetInstance().SetBillMandatoryChargesTooltipModelList(GetMandatoryChargesTooltipModelList(accountChargeseResponse.Data.ResponseData.MandatoryChargesPopUpDetails));
            }
            else
            {
                showRefreshState = true;
            }

            //Get Account Billing History
            AccountBillPayHistoryRequest accountBillPayRequest = new AccountBillPayHistoryRequest(
                contractAccountValue,
                isOwnedAccountValue,
                accountTypeValue);

            AccountBillPayHistoryResponse accountBillPayResponse = await api.GetAccountBillPayHistory<AccountBillPayHistoryResponse>(accountBillPayRequest);
            if (accountBillPayResponse.Data != null && accountBillPayResponse.Data.ErrorCode == "7200")
            {
                billingHistoryList = GetBillingHistoryModelList(accountBillPayResponse.Data.ResponseData.BillPayHistories);
            }
            else
            {
                showRefreshState = true;
            }

            if (showRefreshState)
            {
                mView.ShowRefreshPage(showRefreshState);
            }
            else
            {
                mView.ShowRefreshPage(showRefreshState);
                if (billingHistoryList.Count > 0)
                {
                    mView.PopulateAccountCharge(accountChargeModelList);
                    mView.PopulateBillingHistoryList(billingHistoryList);
                }
                else
                {
                    mView.ShowEmptyState();
                }
            }

            OnGetEnergySavingTips();
        }


        public void OnGetEnergySavingTips()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    BillDetailsTooltipResponseModel responseModel = getItemsService.GetBillDetailsTooltipItem();
                    SitecoreCmsEntity.InsertListOfItems("BILL_TOOLTIP", JsonConvert.SerializeObject(responseModel.Data));
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, new CancellationTokenSource().Token);
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

        private List<AccountBillPayHistoryModel> GetBillingHistoryModelList(List<BillPayHistory> billPayHistoryList)
        {
            List<AccountBillPayHistoryModel> modelList = new List<AccountBillPayHistoryModel>();
            List<AccountBillPayHistoryModel.BillingHistoryData> dataList;
            AccountBillPayHistoryModel.BillingHistoryData data;
            AccountBillPayHistoryModel model;
            billPayHistoryList.ForEach(history =>
            {
                dataList = new List<AccountBillPayHistoryModel.BillingHistoryData>();
                model = new AccountBillPayHistoryModel();
                model.MonthYear = history.MonthYear;

                history.BillPayHistoryData.ForEach(historyData =>
                {
                    data = new AccountBillPayHistoryModel.BillingHistoryData();
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

        private List<BillMandatoryChargesTooltipModel> GetMandatoryChargesTooltipModelList(List<MandatoryChargesPopUpDetail> mandatoryChargesPopUpDetailList)
        {
            List<BillMandatoryChargesTooltipModel> billMandatoryChargesTooltipModelList = new List<BillMandatoryChargesTooltipModel>();
            BillMandatoryChargesTooltipModel model;
            mandatoryChargesPopUpDetailList.ForEach(popupDetail =>
            {
                model = new BillMandatoryChargesTooltipModel();
                model.Title = popupDetail.Title;
                model.Description = popupDetail.Description;
                model.Type = popupDetail.Type;
                model.CTA = popupDetail.CTA;
                billMandatoryChargesTooltipModelList.Add(model);
            });
            return billMandatoryChargesTooltipModelList;
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

        public List<AccountBillPayHistoryModel> GetBillingHistoryList()
        {
            List<AccountBillPayHistoryModel> modelList = new List<AccountBillPayHistoryModel>();

            AccountBillPayHistoryModel model = new AccountBillPayHistoryModel();
            model.MonthYear = "Feb 2019";

            List<AccountBillPayHistoryModel.BillingHistoryData> dataList = new List<AccountBillPayHistoryModel.BillingHistoryData>();

            AccountBillPayHistoryModel.BillingHistoryData data = new AccountBillPayHistoryModel.BillingHistoryData();
            data.HistoryType = "BILL";
            data.DateAndHistoryType = "24 Feb - Bill";
            data.Amount = "257.50";
            data.DetailedInfoNumber = "000530477074";
            data.PaidVia = "24 Feb 2019";
            dataList.Add(data);

            data = new AccountBillPayHistoryModel.BillingHistoryData();

            data.HistoryType = "PAYMENT";
            data.DateAndHistoryType = "05 Mar - Payment";
            data.Amount = "257.00";
            data.DetailedInfoNumber = "";
            data.PaidVia = "via CIMB BATCH";
            dataList.Add(data);


            model.BillingHistoryDataList = dataList;
            modelList.Add(model);

            model = new AccountBillPayHistoryModel();
            model.MonthYear = "Mar 2019";

            dataList = new List<AccountBillPayHistoryModel.BillingHistoryData>();
            data = new AccountBillPayHistoryModel.BillingHistoryData();

            data.HistoryType = "PAYMENT";
            data.DateAndHistoryType = "03 May - Payment";
            data.Amount = "25.00";
            data.DetailedInfoNumber = "000530477074";
            data.PaidVia = "26 Feb 2019";
            dataList.Add(data);

            data = new AccountBillPayHistoryModel.BillingHistoryData();

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
