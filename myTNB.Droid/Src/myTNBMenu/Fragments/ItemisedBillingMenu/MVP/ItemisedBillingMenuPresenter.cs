﻿using System;
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
using Android.Util;
using Android.Gms.Common.Apis;
using myTNB_Android.Src.MyTNBService.Parser;
using myTNB_Android.Src.NewAppTutorial.MVP;
using Android.Content;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP
{
    public class ItemisedBillingMenuPresenter
    {
        BillingApiImpl api;
        ItemisedBillingContract.IView mView;
        AccountChargesModel mAccountChargesModel;
        string storedAccountTypeValue = "";
        List<AccountBillPayHistoryModel> mainBillingHistoryList;
        List<AccountChargeModel> mainAccountChargeModelList;
        private ISharedPreferences mPref;

        public ItemisedBillingMenuPresenter(ItemisedBillingContract.IView view, ISharedPreferences pref)
        {
            mView = view;
            api = new BillingApiImpl();
            mPref = pref;
        }

        public async void GetBillingHistoryDetails(string contractAccountValue, bool isOwnedAccountValue, string accountTypeValue)
        {
            try
            {
                //Get Account Charges Service Call
                storedAccountTypeValue = accountTypeValue;
                bool showChargeRefreshState = false;
                bool showBillRefreshState = false;
                bool showChargeMaintenanceState = false;
                bool showBillMaintenanceState = false;
                List<string> accountList = new List<string>();
                List<AccountChargeModel> accountChargeModelList = new List<AccountChargeModel>();
                List<AccountBillPayHistoryModel> billingHistoryList = new List<AccountBillPayHistoryModel>();
                List<AccountBillPayFilter> billPayFilterList = new List<AccountBillPayFilter>();
                accountList.Add(contractAccountValue);
                mView.ShowAvailableBillContent();

                AccountsChargesRequest accountChargeseRequest = new AccountsChargesRequest(
                    accountList,
                    isOwnedAccountValue
                    );
                AccountChargesResponse accountChargeseResponse = await api.GetAccountsCharges<AccountChargesResponse>(accountChargeseRequest);
                if (accountChargeseResponse.Data != null && accountChargeseResponse.Data.ErrorCode == "7200")
                {
                    Utility.SetIsPayDisableNotFromAppLaunch(!accountChargeseResponse.Data.IsPayEnabled);
                    accountChargeModelList = BillingResponseParser.GetAccountCharges(accountChargeseResponse.Data.ResponseData.AccountCharges);
                    MyTNBAppToolTipData.GetInstance().SetBillMandatoryChargesTooltipModelList(BillingResponseParser.GetMandatoryChargesTooltipModelList(accountChargeseResponse.Data.ResponseData.MandatoryChargesPopUpDetails));
                }
                else if (accountChargeseResponse.Data != null && accountChargeseResponse.Data.ErrorCode == "8400")
                {
                    Utility.SetIsPayDisableNotFromAppLaunch(!accountChargeseResponse.Data.IsPayEnabled);
                    showChargeMaintenanceState = true;
                }
                else
                {
                    if (accountChargeseResponse != null && accountChargeseResponse.Data != null)
                    {
                        Utility.SetIsPayDisableNotFromAppLaunch(!accountChargeseResponse.Data.IsPayEnabled);
                    }
                    showChargeRefreshState = true;
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
                    billPayFilterList = GetAccountBillPayFilterList(accountBillPayResponse.Data.ResponseData.BillPayFilterData);
                }
                else if (accountBillPayResponse.Data != null && accountBillPayResponse.Data.ErrorCode == "8400")
                {
                    showBillMaintenanceState = true;
                }
                else
                {
                    showBillRefreshState = true;
                }

                if (showBillMaintenanceState && showChargeMaintenanceState)
                {
                    string btnText = "";
                    string contentText = "";

                    if (accountChargeseResponse != null && accountChargeseResponse.Data != null && !string.IsNullOrEmpty(accountChargeseResponse.Data.DisplayMessage))
                    {
                        contentText = accountChargeseResponse.Data.DisplayMessage;
                    }

                    if (accountChargeseResponse != null && accountChargeseResponse.Data != null && !string.IsNullOrEmpty(accountChargeseResponse.Data.RefreshBtnText))
                    {
                        btnText = accountChargeseResponse.Data.RefreshBtnText;
                    }

                    mView.ShowUnavailableContent(false, btnText, contentText);
                }
                else if (showBillRefreshState && showChargeRefreshState)
                {
                    string btnText = "";
                    string contentText = "";

                    if (accountChargeseResponse != null && accountChargeseResponse.Data != null && !string.IsNullOrEmpty(accountChargeseResponse.Data.RefreshMessage))
                    {
                        contentText = accountChargeseResponse.Data.RefreshMessage;
                    }

                    if (accountChargeseResponse != null && accountChargeseResponse.Data != null && !string.IsNullOrEmpty(accountChargeseResponse.Data.RefreshBtnText))
                    {
                        btnText = accountChargeseResponse.Data.RefreshBtnText;
                    }

                    mView.ShowUnavailableContent(true, btnText, contentText);
                }
                else
                {
                    if (showChargeMaintenanceState)
                    {
                        string btnText = "";
                        string contentText = "";

                        if (accountChargeseResponse != null && accountChargeseResponse.Data != null && !string.IsNullOrEmpty(accountChargeseResponse.Data.DisplayMessage))
                        {
                            contentText = accountChargeseResponse.Data.DisplayMessage;
                        }

                        if (accountChargeseResponse != null && accountChargeseResponse.Data != null && !string.IsNullOrEmpty(accountChargeseResponse.Data.RefreshBtnText))
                        {
                            btnText = accountChargeseResponse.Data.RefreshBtnText;
                        }

                        mView.ShowUnavailableChargeContent(false, btnText, contentText);
                    }
                    else if (showChargeRefreshState)
                    {
                        string btnText = "";
                        string contentText = "";

                        if (accountChargeseResponse != null && accountChargeseResponse.Data != null && !string.IsNullOrEmpty(accountChargeseResponse.Data.RefreshMessage))
                        {
                            contentText = accountChargeseResponse.Data.RefreshMessage;
                        }

                        if (accountChargeseResponse != null && accountChargeseResponse.Data != null && !string.IsNullOrEmpty(accountChargeseResponse.Data.RefreshBtnText))
                        {
                            btnText = accountChargeseResponse.Data.RefreshBtnText;
                        }

                        mView.ShowUnavailableChargeContent(true, btnText, contentText);
                    }
                    else
                    {
                        mView.PopulateAccountCharge(accountChargeModelList);
                        if (accountChargeModelList != null)
                        {
                            mainAccountChargeModelList = accountChargeModelList;
                        }
                        else
                        {
                            mainAccountChargeModelList = new List<AccountChargeModel>();
                        }
                    }

                    if (showBillMaintenanceState)
                    {
                        string btnText = "";
                        string contentText = "";

                        if (accountBillPayResponse != null && accountBillPayResponse.Data != null && !string.IsNullOrEmpty(accountBillPayResponse.Data.DisplayMessage))
                        {
                            contentText = accountBillPayResponse.Data.DisplayMessage;
                        }

                        if (accountBillPayResponse != null && accountBillPayResponse.Data != null && !string.IsNullOrEmpty(accountBillPayResponse.Data.RefreshBtnText))
                        {
                            btnText = accountBillPayResponse.Data.RefreshBtnText;
                        }

                        mView.ShowUnavailableBillContent(false, btnText, contentText);
                    }
                    else if (showBillRefreshState)
                    {
                        string btnText = "";
                        string contentText = "";

                        if (accountBillPayResponse != null && accountBillPayResponse.Data != null && !string.IsNullOrEmpty(accountBillPayResponse.Data.RefreshMessage))
                        {
                            contentText = accountBillPayResponse.Data.RefreshMessage;
                        }

                        if (accountBillPayResponse != null && accountBillPayResponse.Data != null && !string.IsNullOrEmpty(accountBillPayResponse.Data.RefreshBtnText))
                        {
                            btnText = accountBillPayResponse.Data.RefreshBtnText;
                        }

                        mView.ShowUnavailableBillContent(true, btnText, contentText);
                    }
                    else
                    {
                        mView.PopulateBillingHistoryList(billingHistoryList, billPayFilterList);
                        if (billingHistoryList != null)
                        {
                            mainBillingHistoryList = billingHistoryList;
                        }
                        else
                        {
                            mainBillingHistoryList = new List<AccountBillPayHistoryModel>();
                        }
                    }

                    if (!showChargeRefreshState && !showBillRefreshState && !showChargeMaintenanceState && !showBillMaintenanceState)
                    {
                        OnCheckToCallItemizedTutorial();
                    }

                    OnGetBillTooltipContent();
                }
            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug("BillPayment Presenter", "Cancelled Exception");
                if (this.mView.IsActive())
                {
                    mView.ShowUnavailableContent(true, "", "");
                }

                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                Log.Debug("BillPayment Presenter", "Stack " + apiException.StackTrace);
                if (this.mView.IsActive())
                {
                    mView.ShowUnavailableContent(true, "", "");
                }

                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug("BillPayment Presenter", "Stack " + e.StackTrace);
                if (this.mView.IsActive())
                {
                    mView.ShowUnavailableContent(true, "", "");
                }
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void GetBillingBillHistoryDetails(string contractAccountValue, bool isOwnedAccountValue, string accountTypeValue)
        {
            try
            {
                //Get Account Charges Service Call
                storedAccountTypeValue = accountTypeValue;
                bool showBillRefreshState = false;
                bool showBillMaintenanceState = false;
                List<string> accountList = new List<string>();
                List<AccountChargeModel> accountChargeModelList = new List<AccountChargeModel>();
                List<AccountBillPayHistoryModel> billingHistoryList = new List<AccountBillPayHistoryModel>();
                List<AccountBillPayFilter> billPayFilterList = new List<AccountBillPayFilter>();
                accountList.Add(contractAccountValue);
                mView.ShowAvailableBillContent();

                //Get Account Billing History
                AccountBillPayHistoryRequest accountBillPayRequest = new AccountBillPayHistoryRequest(
                    contractAccountValue,
                    isOwnedAccountValue,
                    accountTypeValue);

                AccountBillPayHistoryResponse accountBillPayResponse = await api.GetAccountBillPayHistory<AccountBillPayHistoryResponse>(accountBillPayRequest);
                if (accountBillPayResponse.Data != null && accountBillPayResponse.Data.ErrorCode == "7200")
                {
                    billingHistoryList = GetBillingHistoryModelList(accountBillPayResponse.Data.ResponseData.BillPayHistories);
                    billPayFilterList = GetAccountBillPayFilterList(accountBillPayResponse.Data.ResponseData.BillPayFilterData);
                }
                else if (accountBillPayResponse.Data != null && accountBillPayResponse.Data.ErrorCode == "8400")
                {
                    showBillMaintenanceState = true;
                }
                else
                {
                    showBillRefreshState = true;
                }

                if (showBillMaintenanceState)
                {
                    string btnText = "";
                    string contentText = "";

                    if (accountBillPayResponse != null && accountBillPayResponse.Data != null && !string.IsNullOrEmpty(accountBillPayResponse.Data.DisplayMessage))
                    {
                        contentText = accountBillPayResponse.Data.DisplayMessage;
                    }

                    if (accountBillPayResponse != null && accountBillPayResponse.Data != null && !string.IsNullOrEmpty(accountBillPayResponse.Data.RefreshBtnText))
                    {
                        btnText = accountBillPayResponse.Data.RefreshBtnText;
                    }

                    mView.ShowUnavailableBillContent(false, btnText, contentText);
                }
                else if (showBillRefreshState)
                {
                    string btnText = "";
                    string contentText = "";

                    if (accountBillPayResponse != null && accountBillPayResponse.Data != null && !string.IsNullOrEmpty(accountBillPayResponse.Data.RefreshMessage))
                    {
                        contentText = accountBillPayResponse.Data.RefreshMessage;
                    }

                    if (accountBillPayResponse != null && accountBillPayResponse.Data != null && !string.IsNullOrEmpty(accountBillPayResponse.Data.RefreshBtnText))
                    {
                        btnText = accountBillPayResponse.Data.RefreshBtnText;
                    }

                    mView.ShowUnavailableBillContent(true, btnText, contentText);
                }
                else
                {
                    mView.PopulateBillingHistoryList(billingHistoryList, billPayFilterList);
                    if (billingHistoryList != null)
                    {
                        mainBillingHistoryList = billingHistoryList;
                    }
                    else
                    {
                        mainBillingHistoryList = new List<AccountBillPayHistoryModel>();
                    }

                    OnCheckToCallItemizedTutorial();
                }
            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug("BillPayment Presenter", "Cancelled Exception");
                if (this.mView.IsActive())
                {
                    mView.ShowUnavailableBillContent(true, "", "");
                }

                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                Log.Debug("BillPayment Presenter", "Stack " + apiException.StackTrace);
                if (this.mView.IsActive())
                {
                    mView.ShowUnavailableBillContent(true, "", "");
                }

                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug("BillPayment Presenter", "Stack " + e.StackTrace);
                if (this.mView.IsActive())
                {
                    mView.ShowUnavailableBillContent(true, "", "");
                }
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void GetBillingChargeHistoryDetails(string contractAccountValue, bool isOwnedAccountValue, string accountTypeValue)
        {
            try
            {
                //Get Account Charges Service Call
                storedAccountTypeValue = accountTypeValue;
                bool showChargeRefreshState = false;
                bool showChargeMaintenanceState = false;
                List<string> accountList = new List<string>();
                List<AccountChargeModel> accountChargeModelList = new List<AccountChargeModel>();
                List<AccountBillPayHistoryModel> billingHistoryList = new List<AccountBillPayHistoryModel>();
                List<AccountBillPayFilter> billPayFilterList = new List<AccountBillPayFilter>();
                accountList.Add(contractAccountValue);
                mView.ShowAvailableBillContent();

                AccountsChargesRequest accountChargeseRequest = new AccountsChargesRequest(
                    accountList,
                    isOwnedAccountValue
                    );
                AccountChargesResponse accountChargeseResponse = await api.GetAccountsCharges<AccountChargesResponse>(accountChargeseRequest);
                if (accountChargeseResponse.Data != null && accountChargeseResponse.Data.ErrorCode == "7200")
                {
                    Utility.SetIsPayDisableNotFromAppLaunch(!accountChargeseResponse.Data.IsPayEnabled);
                    accountChargeModelList = BillingResponseParser.GetAccountCharges(accountChargeseResponse.Data.ResponseData.AccountCharges);
                    MyTNBAppToolTipData.GetInstance().SetBillMandatoryChargesTooltipModelList(BillingResponseParser.GetMandatoryChargesTooltipModelList(accountChargeseResponse.Data.ResponseData.MandatoryChargesPopUpDetails));
                }
                else if (accountChargeseResponse.Data != null && accountChargeseResponse.Data.ErrorCode == "8400")
                {
                    Utility.SetIsPayDisableNotFromAppLaunch(!accountChargeseResponse.Data.IsPayEnabled);
                    showChargeMaintenanceState = true;
                }
                else
                {
                    if (accountChargeseResponse != null && accountChargeseResponse.Data != null)
                    {
                        Utility.SetIsPayDisableNotFromAppLaunch(!accountChargeseResponse.Data.IsPayEnabled);
                    }
                    showChargeRefreshState = true;
                }

                if (showChargeMaintenanceState)
                {
                    string btnText = "";
                    string contentText = "";

                    if (accountChargeseResponse != null && accountChargeseResponse.Data != null && !string.IsNullOrEmpty(accountChargeseResponse.Data.DisplayMessage))
                    {
                        contentText = accountChargeseResponse.Data.DisplayMessage;
                    }

                    if (accountChargeseResponse != null && accountChargeseResponse.Data != null && !string.IsNullOrEmpty(accountChargeseResponse.Data.RefreshBtnText))
                    {
                        btnText = accountChargeseResponse.Data.RefreshBtnText;
                    }

                    mView.ShowUnavailableChargeContent(false, btnText, contentText);
                }
                else if (showChargeRefreshState)
                {
                    string btnText = "";
                    string contentText = "";

                    if (accountChargeseResponse != null && accountChargeseResponse.Data != null && !string.IsNullOrEmpty(accountChargeseResponse.Data.RefreshMessage))
                    {
                        contentText = accountChargeseResponse.Data.RefreshMessage;
                    }

                    if (accountChargeseResponse != null && accountChargeseResponse.Data != null && !string.IsNullOrEmpty(accountChargeseResponse.Data.RefreshBtnText))
                    {
                        btnText = accountChargeseResponse.Data.RefreshBtnText;
                    }

                    mView.ShowUnavailableChargeContent(true, btnText, contentText);
                }
                else
                {
                    mView.PopulateAccountCharge(accountChargeModelList);
                    if (accountChargeModelList != null)
                    {
                        mainAccountChargeModelList = accountChargeModelList;
                    }
                    else
                    {
                        mainAccountChargeModelList = new List<AccountChargeModel>();
                    }

                    OnCheckToCallItemizedTutorial();
                }
            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug("BillPayment Presenter", "Cancelled Exception");
                if (this.mView.IsActive())
                {
                    mView.ShowUnavailableChargeContent(true, "", "");
                }

                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                Log.Debug("BillPayment Presenter", "Stack " + apiException.StackTrace);
                if (this.mView.IsActive())
                {
                    mView.ShowUnavailableChargeContent(true, "", "");
                }

                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug("BillPayment Presenter", "Stack " + e.StackTrace);
                if (this.mView.IsActive())
                {
                    mView.ShowUnavailableChargeContent(true, "", "");
                }
                Utility.LoggingNonFatalError(e);
            }
        }


        public void OnGetBillTooltipContent()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    BillDetailsTooltipResponseModel responseModel = getItemsService.GetBillDetailsTooltipItem();
                    SitecoreCmsEntity.InsertSiteCoreItem(SitecoreCmsEntity.SITE_CORE_ID.BILL_TOOLTIP, JsonConvert.SerializeObject(responseModel.Data),"");
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, new CancellationTokenSource().Token);
        }

        private List<AccountBillPayFilter> GetAccountBillPayFilterList(List<BillPayFilterData> billPayFilters)
        {
            List<AccountBillPayFilter> accountBillPayFilters = new List<AccountBillPayFilter>();
            AccountBillPayFilter billPayFilter;
            billPayFilters.ForEach(filter =>
            {
                billPayFilter = new AccountBillPayFilter();
                billPayFilter.Text = filter.Text;
                billPayFilter.Type = filter.Type;
                accountBillPayFilters.Add(billPayFilter);
            });
            return accountBillPayFilters;
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
                    data.IsPaymentPending = historyData.IsPaymentPending;
                    dataList.Add(data);
                });

                model.BillingHistoryDataList = dataList;
                modelList.Add(model);
            });

            return modelList;
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

        public void OnCheckToCallItemizedTutorial()
        {
            if ((storedAccountTypeValue == "RE" && !UserSessions.HasItemizedBillingRETutorialShown(this.mPref)) || (storedAccountTypeValue != "RE" && !UserSessions.HasItemizedBillingNMSMTutorialShown(this.mPref)))
            {
                this.mView.OnShowItemizedFragmentTutorialDialog();
            }
        }

        public bool IsTutorialShowNeeded()
        {
            return mainBillingHistoryList != null && mainAccountChargeModelList != null;
        }

        public List<NewAppModel> OnGeneraNewAppTutorialList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            string DisplayMode = "NoExtra";

            int ItemCount = 0;

            for(int i = 0; i < mainBillingHistoryList.Count; i++)
            {
                ItemCount += mainBillingHistoryList[i].BillingHistoryDataList.Count;
            }

            AccountChargeModel accountChargeModel = mainAccountChargeModelList[0];
            if (storedAccountTypeValue == "RE")
            {
                if (accountChargeModel.IsPaidExtra)
                {
                    DisplayMode = "NoExtra";
                }
                else
                {
                    DisplayMode = "Extra";
                }
            }
            else
            {
                if (this.mView.GetIsPendingPayment())
                {
                    DisplayMode = "Extra";
                }
                else
                {
                    if (accountChargeModel.IsNeedPay)
                    {
                        DisplayMode = "NoExtra";
                    }
                    else
                    {
                        DisplayMode = "Extra";
                    }
                }
            }

            if (storedAccountTypeValue == "RE")
            {
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.BottomLeft,
                    ContentTitle = Utility.GetLocalizedLabel("Bills", "tutorialAdviceTitle"), //"Your advice overview.",
                    ContentMessage = Utility.GetLocalizedLabel("Bills", "tutorialBillREAcctDesc"),//"Tap \" " +Constants.APP_TUTORIAL_PATTERN+ " \" to switch between<br/>different accounts. You’ll see how<br/>much you have earned or if you’ve <br/>been paid extra.",
                    ItemCount = ItemCount,
                    DisplayMode = DisplayMode,
                    IsButtonShow = false
                });

                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.TopLeft,
                    ContentTitle = Utility.GetLocalizedLabel("Bills", "tutorialHistoryNormalTitle"),//"Keep track of payments.",
                    ContentMessage = Utility.GetLocalizedLabel("Bills", "tutorialHistoryREAcctDesc"),//"View and access your advices and<br/>payment receipts from the<br/>previous six months. Use the filter<br/>to see only advices or receipts.",
                    ItemCount = ItemCount,
                    DisplayMode = DisplayMode,
                    IsButtonShow = true
                });
            }
            else
            {
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.BottomLeft,
                    ContentTitle = Utility.GetLocalizedLabel("Bills", "tutorialBillTitle"),//"Your bill overview.",
                    ContentMessage = Utility.GetLocalizedLabel("Bills", "tutorialBillNormalAcctDesc"),//"Tap \" " + Constants.APP_TUTORIAL_PATTERN + " \" to switch between<br/>different accounts. You’ll see how<br/>much is due, if you’ve cleared your<br/>bill or if you’ve paid extra.",
                    ItemCount = ItemCount,
                    DisplayMode = DisplayMode,
                    IsButtonShow = false
                });

                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.TopRight,
                    ContentTitle = Utility.GetLocalizedLabel("Bills", "tutorialPayTitle"),//"Pay without hassle.",
                    ContentMessage = Utility.GetLocalizedLabel("Bills", "tutorialPayDesc"),//"Tap here to pay your bill.",
                    ItemCount = ItemCount,
                    DisplayMode = DisplayMode,
                    IsButtonShow = false
                });

                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.TopLeft,
                    ContentTitle = Utility.GetLocalizedLabel("Bills", "tutorialViewDetailsTitle"),//"Understand your bill.",
                    ContentMessage = Utility.GetLocalizedLabel("Bills", "tutorialViewDetailsDesc"),//"‘View Details’ to review your<br/>bill breakdown.",
                    ItemCount = ItemCount,
                    DisplayMode = DisplayMode,
                    IsButtonShow = false
                });

                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.TopLeft,
                    ContentTitle = Utility.GetLocalizedLabel("Bills", "tutorialHistoryTitle"),//"Keep track of your charges.",
                    ContentMessage = Utility.GetLocalizedLabel("Bills", "tutorialHistoryNormalAcctDesc"),//"View and access your bills and<br/>payment receipts from the<br/>previous six months. Use the<br/>filter to see only bills or receipts.",
                    ItemCount = ItemCount,
                    DisplayMode = DisplayMode,
                    IsButtonShow = true
                });
            }

            return newList;
        }
    }
}
