using System;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.Utils;
using System.Threading;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Services;
using myTNB.AndroidApp.Src.SiteCore;
using myTNB.SitecoreCMS.Model;
using Android.App;
using Newtonsoft.Json;
using myTNB.AndroidApp.Src.MyTNBService.Model;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using static myTNB.AndroidApp.Src.MyTNBService.Response.AccountChargesResponse;
using static myTNB.AndroidApp.Src.MyTNBService.Response.AccountBillPayHistoryResponse;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Base;
using Android.Util;
using Android.Gms.Common.Apis;
using myTNB.AndroidApp.Src.MyTNBService.Parser;
using myTNB.AndroidApp.Src.NewAppTutorial.MVP;
using Android.Content;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.Mobile;
using myTNB.AndroidApp.Src.myTNBMenu.Models;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP
{
    public class ItemisedBillingMenuPresenter
    {
        ItemisedBillingContract.IView mView;
        AccountChargesModel mAccountChargesModel;
        string storedAccountTypeValue = "";
        List<AccountBillPayHistoryModel> mainBillingHistoryList;
        List<AccountChargeModel> mainAccountChargeModelList;
        private ISharedPreferences mPref;

        public ItemisedBillingMenuPresenter(ItemisedBillingContract.IView view, ISharedPreferences pref)
        {
            mView = view;
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

                AccountsChargesRequest accountChargesRequest = new AccountsChargesRequest(
                    accountList,
                    isOwnedAccountValue
                    );
                AccountChargesResponse accountChargesResponse = await ServiceApiImpl.Instance.GetAccountsCharges(accountChargesRequest);
                if (accountChargesResponse.IsSuccessResponse())
                {
                    Utility.SetIsPayDisableNotFromAppLaunch(!accountChargesResponse.Response.IsPayEnabled);
                    accountChargeModelList = BillingResponseParser.GetAccountCharges(accountChargesResponse.GetData().AccountCharges);
                    MyTNBAppToolTipData.GetInstance().SetBillMandatoryChargesTooltipModelList(BillingResponseParser.GetMandatoryChargesTooltipModelList(accountChargesResponse.GetData().MandatoryChargesPopUpDetails));
                }
                else if (accountChargesResponse.Response != null && accountChargesResponse.Response.ErrorCode == "8400")
                {
                    Utility.SetIsPayDisableNotFromAppLaunch(!accountChargesResponse.Response.IsPayEnabled);
                    showChargeMaintenanceState = true;
                }
                else
                {
                    if (accountChargesResponse != null && accountChargesResponse.Response != null)
                    {
                        Utility.SetIsPayDisableNotFromAppLaunch(!accountChargesResponse.Response.IsPayEnabled);
                    }
                    showChargeRefreshState = true;
                }

                //Get Account Billing History
                AccountBillPayHistoryRequest accountBillPayRequest = new AccountBillPayHistoryRequest(
                    contractAccountValue,
                    isOwnedAccountValue,
                    accountTypeValue);

                AccountBillPayHistoryResponse accountBillPayResponse = await ServiceApiImpl.Instance.GetAccountBillPayHistory(accountBillPayRequest);
                if (accountBillPayResponse.IsSuccessResponse())
                {
                    billingHistoryList = GetBillingHistoryModelList(accountBillPayResponse.GetData().BillPayHistories);
                    billPayFilterList = GetAccountBillPayFilterList(accountBillPayResponse.GetData().BillPayFilterData);
                }
                else if (accountBillPayResponse.Response != null && accountBillPayResponse.Response.ErrorCode == "8400")
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

                    if (accountChargesResponse != null && accountChargesResponse.Response != null && !string.IsNullOrEmpty(accountChargesResponse.Response.DisplayMessage))
                    {
                        contentText = accountChargesResponse.Response.DisplayMessage;
                    }

                    if (accountChargesResponse != null && accountChargesResponse.Response != null && !string.IsNullOrEmpty(accountChargesResponse.Response.RefreshBtnText))
                    {
                        btnText = accountChargesResponse.Response.RefreshBtnText;
                    }

                    mView.ShowUnavailableContent(false, btnText, contentText);
                }
                else if (showBillRefreshState && showChargeRefreshState)
                {
                    string btnText = "";
                    string contentText = "";

                    if (accountChargesResponse != null && accountChargesResponse.Response != null && !string.IsNullOrEmpty(accountChargesResponse.Response.RefreshMessage))
                    {
                        contentText = accountChargesResponse.Response.RefreshMessage;
                    }

                    if (accountChargesResponse != null && accountChargesResponse.Response != null && !string.IsNullOrEmpty(accountChargesResponse.Response.RefreshBtnText))
                    {
                        btnText = accountChargesResponse.Response.RefreshBtnText;
                    }

                    mView.ShowUnavailableContent(true, btnText, contentText);
                }
                else
                {
                    if (showChargeMaintenanceState)
                    {
                        string btnText = "";
                        string contentText = "";

                        if (accountChargesResponse != null && accountChargesResponse.Response != null && !string.IsNullOrEmpty(accountChargesResponse.Response.DisplayMessage))
                        {
                            contentText = accountChargesResponse.Response.DisplayMessage;
                        }

                        if (accountChargesResponse != null && accountChargesResponse.Response != null && !string.IsNullOrEmpty(accountChargesResponse.Response.RefreshBtnText))
                        {
                            btnText = accountChargesResponse.Response.RefreshBtnText;
                        }

                        mView.ShowUnavailableChargeContent(false, btnText, contentText);
                    }
                    else if (showChargeRefreshState)
                    {
                        string btnText = "";
                        string contentText = "";

                        if (accountChargesResponse != null && accountChargesResponse.Response != null && !string.IsNullOrEmpty(accountChargesResponse.Response.RefreshMessage))
                        {
                            contentText = accountChargesResponse.Response.RefreshMessage;
                        }

                        if (accountChargesResponse != null && accountChargesResponse.Response != null && !string.IsNullOrEmpty(accountChargesResponse.Response.RefreshBtnText))
                        {
                            btnText = accountChargesResponse.Response.RefreshBtnText;
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

                        OnCleanUpNotifications(contractAccountValue, accountChargeModelList);
                    }

                    if (showBillMaintenanceState)
                    {
                        string btnText = "";
                        string contentText = "";

                        if (accountBillPayResponse != null && accountBillPayResponse.Response != null && !string.IsNullOrEmpty(accountBillPayResponse.Response.DisplayMessage))
                        {
                            contentText = accountBillPayResponse.Response.DisplayMessage;
                        }

                        if (accountBillPayResponse != null && accountBillPayResponse.Response != null && !string.IsNullOrEmpty(accountBillPayResponse.Response.RefreshBtnText))
                        {
                            btnText = accountBillPayResponse.Response.RefreshBtnText;
                        }

                        mView.ShowUnavailableBillContent(false, btnText, contentText);
                    }
                    else if (showBillRefreshState)
                    {
                        string btnText = "";
                        string contentText = "";

                        if (accountBillPayResponse != null && accountBillPayResponse.Response != null && !string.IsNullOrEmpty(accountBillPayResponse.Response.RefreshMessage))
                        {
                            contentText = accountBillPayResponse.Response.RefreshMessage;
                        }

                        if (accountBillPayResponse != null && accountBillPayResponse.Response != null && !string.IsNullOrEmpty(accountBillPayResponse.Response.RefreshBtnText))
                        {
                            btnText = accountBillPayResponse.Response.RefreshBtnText;
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

                AccountBillPayHistoryResponse accountBillPayResponse = await ServiceApiImpl.Instance.GetAccountBillPayHistory(accountBillPayRequest);
                if (accountBillPayResponse.IsSuccessResponse())
                {
                    billingHistoryList = GetBillingHistoryModelList(accountBillPayResponse.GetData().BillPayHistories);
                    billPayFilterList = GetAccountBillPayFilterList(accountBillPayResponse.GetData().BillPayFilterData);
                }
                else if (accountBillPayResponse.Response != null && accountBillPayResponse.Response.ErrorCode == "8400")
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

                    if (accountBillPayResponse != null && accountBillPayResponse.Response != null && !string.IsNullOrEmpty(accountBillPayResponse.Response.DisplayMessage))
                    {
                        contentText = accountBillPayResponse.Response.DisplayMessage;
                    }

                    if (accountBillPayResponse != null && accountBillPayResponse.Response != null && !string.IsNullOrEmpty(accountBillPayResponse.Response.RefreshBtnText))
                    {
                        btnText = accountBillPayResponse.Response.RefreshBtnText;
                    }

                    mView.ShowUnavailableBillContent(false, btnText, contentText);
                }
                else if (showBillRefreshState)
                {
                    string btnText = "";
                    string contentText = "";

                    if (accountBillPayResponse != null && accountBillPayResponse.Response != null && !string.IsNullOrEmpty(accountBillPayResponse.Response.RefreshMessage))
                    {
                        contentText = accountBillPayResponse.Response.RefreshMessage;
                    }

                    if (accountBillPayResponse != null && accountBillPayResponse.Response != null && !string.IsNullOrEmpty(accountBillPayResponse.Response.RefreshBtnText))
                    {
                        btnText = accountBillPayResponse.Response.RefreshBtnText;
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
                AccountChargesResponse accountChargesResponse = await ServiceApiImpl.Instance.GetAccountsCharges(accountChargeseRequest);
                if (accountChargesResponse.IsSuccessResponse())
                {
                    Utility.SetIsPayDisableNotFromAppLaunch(!accountChargesResponse.Response.IsPayEnabled);
                    accountChargeModelList = BillingResponseParser.GetAccountCharges(accountChargesResponse.GetData().AccountCharges);
                    MyTNBAppToolTipData.GetInstance().SetBillMandatoryChargesTooltipModelList(BillingResponseParser.GetMandatoryChargesTooltipModelList(accountChargesResponse.GetData().MandatoryChargesPopUpDetails));
                }
                else if (accountChargesResponse.Response != null && accountChargesResponse.Response.ErrorCode == "8400")
                {
                    Utility.SetIsPayDisableNotFromAppLaunch(!accountChargesResponse.Response.IsPayEnabled);
                    showChargeMaintenanceState = true;
                }
                else
                {
                    if (accountChargesResponse != null && accountChargesResponse.Response != null)
                    {
                        Utility.SetIsPayDisableNotFromAppLaunch(!accountChargesResponse.Response.IsPayEnabled);
                    }
                    showChargeRefreshState = true;
                }

                if (showChargeMaintenanceState)
                {
                    string btnText = "";
                    string contentText = "";

                    if (accountChargesResponse != null && accountChargesResponse.Response != null && !string.IsNullOrEmpty(accountChargesResponse.Response.DisplayMessage))
                    {
                        contentText = accountChargesResponse.Response.DisplayMessage;
                    }

                    if (accountChargesResponse != null && accountChargesResponse.Response != null && !string.IsNullOrEmpty(accountChargesResponse.Response.RefreshBtnText))
                    {
                        btnText = accountChargesResponse.Response.RefreshBtnText;
                    }

                    mView.ShowUnavailableChargeContent(false, btnText, contentText);
                }
                else if (showChargeRefreshState)
                {
                    string btnText = "";
                    string contentText = "";

                    if (accountChargesResponse != null && accountChargesResponse.Response != null && !string.IsNullOrEmpty(accountChargesResponse.Response.RefreshMessage))
                    {
                        contentText = accountChargesResponse.Response.RefreshMessage;
                    }

                    if (accountChargesResponse != null && accountChargesResponse.Response != null && !string.IsNullOrEmpty(accountChargesResponse.Response.RefreshBtnText))
                    {
                        btnText = accountChargesResponse.Response.RefreshBtnText;
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

                    OnCleanUpNotifications(contractAccountValue, accountChargeModelList);
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
                if (!this.mView.GetIsIneligiblePopUpActive())
                {
                    this.mView.OnShowItemizedFragmentTutorialDialog();
                }
            }
        }

        public bool IsTutorialShowNeeded()
        {
            return mainBillingHistoryList != null && mainAccountChargeModelList != null;
        }

        public List<NewAppModel> OnGeneraNewAppTutorialList(bool _isOwner, bool _isCADBREligible, bool _isBillStatement, AccountData accountData)
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            string DisplayMode = "NoExtra";

            int ItemCount = 0;

            for (int i = 0; i < mainBillingHistoryList.Count; i++)
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
                if (_isCADBREligible)
                {
                    if (_isOwner)
                    {
                        newList.Add(new NewAppModel()
                        {
                            ContentShowPosition = ContentType.TopLeft,
                            ContentTitle = Utility.GetLocalizedLabel("Tutorial", "dbrBillTitle"),
                            ContentMessage = Utility.GetLocalizedLabel("Tutorial", "dbrBillMessage"),
                            ItemCount = ItemCount,
                            DisplayMode = DisplayMode,
                            IsButtonShow = false
                        });
                    }
                    else
                    {
                        newList.Add(new NewAppModel()
                        {
                            ContentShowPosition = ContentType.TopLeft,
                            ContentTitle = Utility.GetLocalizedLabel("Tutorial", "dbrBillTitleTenant"),
                            ContentMessage = Utility.GetLocalizedLabel("Tutorial", "dbrBillMessageTenant"),
                            ItemCount = ItemCount,
                            DisplayMode = DisplayMode,
                            IsButtonShow = false
                        });
                    }
                }
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.TopLeft,
                    ContentTitle = Utility.GetLocalizedLabel("Bills", "tutorialHistoryNormalTitle"),//"Keep track of payments.",
                    ContentMessage = Utility.GetLocalizedLabel("Bills", "tutorialHistoryREAcctDesc"),//"View and access your advices and<br/>payment receipts from the<br/>previous six months. Use the filter<br/>to see only advices or receipts.",
                    ItemCount = ItemCount,
                    DisplayMode = DisplayMode,
                    IsButtonShow = false
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
                if (_isCADBREligible)
                {
                    if (_isOwner)
                    {
                        newList.Add(new NewAppModel()
                        {
                            ContentShowPosition = ContentType.TopLeft,
                            ContentTitle = Utility.GetLocalizedLabel("Tutorial", "dbrBillTitle"),
                            ContentMessage = Utility.GetLocalizedLabel("Tutorial", "dbrBillMessage"),
                            ItemCount = ItemCount,
                            DisplayMode = DisplayMode,
                            IsButtonShow = false
                        });
                    }
                    else
                    {
                        newList.Add(new NewAppModel()
                        {
                            ContentShowPosition = ContentType.TopLeft,
                            ContentTitle = Utility.GetLocalizedLabel("Tutorial", "dbrBillTitleTenant"),
                            ContentMessage = Utility.GetLocalizedLabel("Tutorial", "dbrBillMessageTenant"),
                            ItemCount = ItemCount,
                            DisplayMode = DisplayMode,
                            IsButtonShow = false
                        });
                    }
                }
                if (_isBillStatement)
                {
                    newList.Add(new NewAppModel()
                    {
                        ContentShowPosition = ContentType.TopRight,
                        ContentTitle = Utility.GetLocalizedLabel("Tutorial", "billStatementTitle"),
                        ContentMessage = Utility.GetLocalizedLabel("Tutorial", "billStatementMessage"),
                        ItemCount = ItemCount,
                        DisplayMode = DisplayMode,
                        IsButtonShow = false
                    });
                }
                if (accountData.IsOwner)
                {
                    newList.Add(new NewAppModel()
                    {
                        ContentShowPosition = ContentType.TopLeft,
                        ContentTitle = Utility.GetLocalizedLabel("Bills", "tutorialHistoryTitle"),//"Keep track of your charges.",
                        ContentMessage = Utility.GetLocalizedLabel("Bills", "tutorialHistoryNormalAcctDesc"),//"View and access your bills and<br/>payment receipts from the<br/>previous six months. Use the<br/>filter to see only bills or receipts.",
                        ItemCount = ItemCount,
                        DisplayMode = DisplayMode,
                        IsButtonShow = false
                    });

                }
                else
                {
                    newList.Add(new NewAppModel()
                    {
                        ContentShowPosition = ContentType.TopLeft,
                        ContentTitle = Utility.GetLocalizedLabel("Bills", "tutorialHistoryTitleNonOwner"),
                        ContentMessage = Utility.GetLocalizedLabel("Bills", "tutorialHistoryDescNonOwner"),
                        ItemCount = ItemCount,
                        DisplayMode = DisplayMode,
                        IsButtonShow = false
                    });

                }
                
            }

            return newList;
        }

        private void OnCleanUpNotifications(string accNum, List<AccountChargeModel> accountChargeModelList)
        {
            try
            {
                if (MyTNBAccountManagement.GetInstance().IsNotificationServiceCompleted())
                {
                    List<Notifications.Models.UserNotificationData> ToBeDeleteList = new List<Notifications.Models.UserNotificationData>();

                    AccountChargeModel accountChargeModel = accountChargeModelList[0];

                    double amtDue = 0.00;
                    if (storedAccountTypeValue == "RE")
                    {
                        amtDue = accountChargeModel.AmountDue * -1;
                    }
                    else
                    {
                        amtDue = accountChargeModel.AmountDue;
                    }

                    if (amtDue <= 0.00)
                    {
                        List<UserNotificationEntity> billDueList = UserNotificationEntity.ListFilteredNotificationsByBCRMType(accNum, Constants.BCRM_NOTIFICATION_BILL_DUE_ID);
                        if (billDueList != null && billDueList.Count > 0)
                        {
                            for (int j = 0; j < billDueList.Count; j++)
                            {
                                UserNotificationEntity.UpdateIsDeleted(billDueList[j].Id, true);
                                Notifications.Models.UserNotificationData temp = new Notifications.Models.UserNotificationData();
                                temp.Id = billDueList[j].Id;
                                temp.NotificationType = billDueList[j].NotificationType;
                                ToBeDeleteList.Add(temp);
                            }
                        }

                        List<UserNotificationEntity> disconnectNoticeList = UserNotificationEntity.ListFilteredNotificationsByBCRMType(accNum, Constants.BCRM_NOTIFICATION_DISCONNECT_NOTICE_ID);
                        if (disconnectNoticeList != null && disconnectNoticeList.Count > 0)
                        {
                            for (int j = 0; j < disconnectNoticeList.Count; j++)
                            {
                                UserNotificationEntity.UpdateIsDeleted(disconnectNoticeList[j].Id, true);
                                Notifications.Models.UserNotificationData temp = new Notifications.Models.UserNotificationData();
                                temp.Id = disconnectNoticeList[j].Id;
                                temp.NotificationType = disconnectNoticeList[j].NotificationType;
                                ToBeDeleteList.Add(temp);
                            }
                        }
                    }

                    if (ToBeDeleteList != null && ToBeDeleteList.Count > 0)
                    {
                        _ = OnBatchDeleteNotifications(ToBeDeleteList);
                    }
                }
            }
            catch (Exception unknownException)
            {
                Utility.LoggingNonFatalError(unknownException);
            }

        }

        private async Task OnBatchDeleteNotifications(List<Notifications.Models.UserNotificationData> accountList)
        {
            try
            {
                if (accountList != null && accountList.Count > 0)
                {
                    UserNotificationDeleteResponse notificationDeleteResponse = await ServiceApiImpl.Instance.DeleteUserNotification(new UserNotificationDeleteRequest(accountList));

                    if (notificationDeleteResponse.IsSuccessResponse())
                    {

                    }
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                Utility.LoggingNonFatalError(unknownException);
            }
        }
    }
}
