using myTNB.DataManager;
using myTNB.Model;
using myTNB.Model.Usage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UIKit;

namespace myTNB
{
    public partial class UsageViewController : UsageBaseViewController
    {
        public UsageViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            InitiateAPICalls();
        }

        internal override void InitiateAPICalls()
        {
            if (!DataManager.DataManager.SharedInstance.IsSameAccount)
            {
                var accNum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum;
                if (isNormalChart)
                {
                    CallGetAccountStatusAPI(accNum);
                }
                else
                {
                    CallGetAccountStatusAPI(accNum);
                    if (isSmartMeterAccount)
                    {
                        CallGetAccountUsageSmartAPI(accNum);
                    }
                    else
                    {
                        CallGetAccountUsageAPI(accNum);
                    }
                }
                CallGetAccountDueAmountAPI(accNum);
            }
        }

        #region OVERRIDDEN Methods
        internal override void OnReadHistoryTap()
        {
            base.OnReadHistoryTap();
            UIStoryboard storyBoard = UIStoryboard.FromName("SSMR", null);
            SSMRReadingHistoryViewController viewController =
                storyBoard.InstantiateViewController("SSMRReadingHistoryViewController") as SSMRReadingHistoryViewController;
            if (viewController != null)
            {
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            }
        }
        internal override void OnSubmitMeterTap()
        {
            base.OnSubmitMeterTap();
            UIStoryboard storyBoard = UIStoryboard.FromName("SSMR", null);
            SSMRReadMeterViewController viewController =
                storyBoard.InstantiateViewController("SSMRReadMeterViewController") as SSMRReadMeterViewController;
            if (viewController != null)
            {
                viewController.IsFromDashboard = true;
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            }
        }
        internal override void OnViewDetailsButtonTap()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        UIStoryboard storyBoard = UIStoryboard.FromName("BillDetails", null);
                        BillDetailsViewController viewController =
                            storyBoard.InstantiateViewController("BillDetailsView") as BillDetailsViewController;
                        if (viewController != null)
                        {
                            viewController.AccountNumber = DataManager.DataManager.SharedInstance.SelectedAccount.accNum;
                            viewController.IsFreshCall = true;
                            UINavigationController navController = new UINavigationController(viewController);
                            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                            PresentViewController(navController, true, null);
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }
        internal override void OnCurrentBillButtonTap()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        DataManager.DataManager.SharedInstance.IsSameAccount = true;
                        UIStoryboard storyBoard = UIStoryboard.FromName("ViewBill", null);
                        ViewBillViewController viewController =
                            storyBoard.InstantiateViewController("ViewBillViewController") as ViewBillViewController;
                        if (viewController != null)
                        {
                            viewController.IsFromUsage = true;
                            UINavigationController navController = new UINavigationController(viewController);
                            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                            PresentViewController(navController, true, null);
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }
        internal override void OnPayButtonTap(double amountDue)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        DataManager.DataManager.SharedInstance.IsSameAccount = true;
                        UIStoryboard storyBoard = UIStoryboard.FromName("Payment", null);
                        SelectBillsViewController selectBillsVC =
                            storyBoard.InstantiateViewController("SelectBillsViewController") as SelectBillsViewController;
                        if (selectBillsVC != null)
                        {
                            selectBillsVC.SelectedAccountDueAmount = amountDue;
                            UINavigationController navController = new UINavigationController(selectBillsVC);
                            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                            PresentViewController(navController, true, null);
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }
        internal override void RefreshButtonOnTap()
        {
            base.RefreshButtonOnTap();
            InitiateAPICalls();
        }
        #endregion
        #region API Calls
        private string GetAccountType()
        {
            string accountType = UsageConstants.STR_NormalAccount;
            if (isREAccount)
            {
                accountType = UsageConstants.STR_REAccount;
            }
            return accountType;
        }
        private async Task<AccountUsageResponseModel> GetAccountUsage(CustomerAccountRecordModel account, bool isSSMR = false)
        {
            ServiceManager serviceManager = new ServiceManager();
            object requestParameter = new
            {
                contractAccount = account.accNum,
                isOwner = account.isOwned,
                serviceManager.usrInf,
                accountType = isSSMR ? UsageConstants.STR_SSMRAccount : GetAccountType()
            };

            _accountUsageResponse = await Task.Run(() =>
            {
                return serviceManager.OnExecuteAPIV6<AccountUsageResponseModel>("GetAccountUsage", requestParameter);
            });

            return _accountUsageResponse;
        }
        private async Task<SMRAccountActivityInfoResponseModel> GetSMRAccountActivityInfo(CustomerAccountRecordModel account)
        {
            ServiceManager serviceManager = new ServiceManager();
            object requestParameter = new
            {
                contractAccount = account.accNum,
                isOwnedAccount = account.isOwned,
                serviceManager.usrInf
            };

            _smrAccountActivityInfoResponse = await Task.Run(() =>
            {
                return serviceManager.OnExecuteAPIV6<SMRAccountActivityInfoResponseModel>("GetSMRAccountActivityInfo", requestParameter);
            });

            return _smrAccountActivityInfoResponse;
        }
        private void CallGetAccountUsageAPI(string accNum)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        if (AccountUsageCache.IsRefreshNeeded(DataManager.DataManager.SharedInstance.SelectedAccount.accNum))
                        {
                            AccountUsageCache.ClearTariffLegendList();
                            InvokeInBackground(async () =>
                            {
                                AccountUsageResponseModel accountUsageResponse = await GetAccountUsage(DataManager.DataManager.SharedInstance.SelectedAccount);
                                InvokeOnMainThread(() =>
                                {
                                    if (accNum == DataManager.DataManager.SharedInstance.SelectedAccount.accNum)
                                    {
                                        AccountUsageCache.SetData(DataManager.DataManager.SharedInstance.SelectedAccount.accNum, accountUsageResponse);
                                        if (AccountUsageCache.IsSuccess)
                                        {
                                            SetTariffButtonState();
                                            SetTariffLegendComponent();
                                            SetChartView(false);
                                        }
                                        else if (AccountUsageCache.IsDataEmpty)
                                        {
                                            SetEmptyDataComponent(AccountUsageCache.EmptyDataMessage);
                                        }
                                        else
                                        {
                                            SetRefreshScreen();
                                            if (isREAccount)
                                            {
                                                SetREAmountViewForRefresh();
                                            }
                                            else
                                            {
                                                HideREAmountView();
                                            }
                                        }
                                    }
                                });
                            });
                        }
                        else
                        {
                            AccountUsageCache.ClearTariffLegendList();
                            AccountUsageCache.GetCachedData(DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
                            SetTariffButtonState();
                            SetTariffLegendComponent();
                            SetChartView(false);
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }
        private void CallGetAccountUsageSmartAPI(string accNum)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        AccountUsageSmartCache.IsMDMSDown = false;
                        if (AccountUsageSmartCache.IsRefreshNeeded(DataManager.DataManager.SharedInstance.SelectedAccount.accNum))
                        {
                            SetSmartMeterComponent(true);
                            AccountUsageSmartCache.ClearTariffLegendList();
                            InvokeInBackground(async () =>
                            {
                                AccountUsageSmartResponseModel accountUsageSmartResponse = await UsageServiceCall.GetAccountUsageSmart(DataManager.DataManager.SharedInstance.SelectedAccount);
                                //AccountUsageSmartResponseModel accountUsageSmartResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<AccountUsageSmartResponseModel>(AccountUsageManager.GetData());

                                InvokeOnMainThread(() =>
                                {
                                    if (accNum == DataManager.DataManager.SharedInstance.SelectedAccount.accNum)
                                    {
                                        AccountUsageSmartCache.SetData(DataManager.DataManager.SharedInstance.SelectedAccount.accNum, accountUsageSmartResponse);
                                        if (AccountUsageSmartCache.IsSuccess || AccountUsageSmartCache.IsMDMSDown)
                                        {
                                            OtherUsageMetricsModel model = AccountUsageSmartCache.GetUsageMetrics();
                                            SetSmartMeterComponent(false, model.Cost);
                                            SetTariffButtonState();
                                            SetTariffLegendComponent();
                                            SetChartView(false);
                                        }
                                        else if (AccountUsageSmartCache.IsDataEmpty)
                                        {
                                            AccountUsageSmartCache.SetUsageMetrics(accountUsageSmartResponse);
                                            OtherUsageMetricsModel model = AccountUsageSmartCache.GetUsageMetrics();
                                            SetSmartMeterComponent(false, model.Cost);
                                            SetEmptyDataComponent(AccountUsageSmartCache.EmptyDataMessage);
                                        }
                                        else
                                        {
                                            SetRefreshScreen();
                                            SetContentViewForRefresh();
                                            HideREAmountView();
                                            HideSSMRViewForRefresh();
                                        }
                                    }

                                });
                            });
                        }
                        else
                        {
                            AccountUsageSmartCache.ClearTariffLegendList();
                            AccountUsageSmartCache.GetCachedData(DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
                            OtherUsageMetricsModel model = AccountUsageSmartCache.GetUsageMetrics();
                            SetSmartMeterComponent(false, model.Cost);
                            SetTariffButtonState();
                            SetTariffLegendComponent();
                            SetChartView(false);
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }
        private void CallGetAccountDueAmountAPI(string accNum)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        if (isREAccount)
                        {
                            UpdateREAmountViewUI(true);
                        }
                        else
                        {
                            UpdateFooterUI(true);
                        }
                        var account = DataManager.DataManager.SharedInstance.SelectedAccount;
                        InvokeInBackground(async () =>
                        {
                            DueAmountResponseModel dueAmountResponse = await UsageServiceCall.GetAccountDueAmount(account);
                            InvokeOnMainThread(() =>
                            {
                                if (accNum == DataManager.DataManager.SharedInstance.SelectedAccount.accNum)
                                {
                                    if (dueAmountResponse != null &&
                                    dueAmountResponse.d != null &&
                                    dueAmountResponse.d.IsSuccess &&
                                    dueAmountResponse.d.data != null &&
                                    dueAmountResponse.d.data.AccountAmountDue != null)
                                    {
                                        var model = dueAmountResponse.d.data.AccountAmountDue;
                                        var item = new DueAmountDataModel
                                        {
                                            accNum = account.accNum,
                                            accNickName = account.accountNickName,
                                            IsReAccount = account.IsREAccount,
                                            amountDue = model.amountDue,
                                            billDueDate = model.billDueDate,
                                            IncrementREDueDateByDays = model.IncrementREDueDateByDays
                                        };
                                        AmountDueCache.SaveDues(item);
                                        if (isREAccount)
                                        {
                                            UpdateREAmountViewUI(false);
                                        }
                                        else
                                        {
                                            UpdateFooterUI(false);
                                        }
                                    }
                                    else
                                    {
                                        if (isREAccount)
                                        {
                                            UpdateREAmountViewForRefreshState();
                                        }
                                        else
                                        {
                                            UpdateFooterForRefreshState();
                                        }
                                    }
                                }
                            });
                        });
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }
        private void CallGetAccountStatusAPI(string accNum)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        SetDisconnectionComponent(true);
                        AccountStatusCache.ClearAccountStatusData();
                        InvokeInBackground(async () =>
                        {
                            AccountStatusResponseModel accountStatusResponse = await UsageServiceCall.GetAccountStatus(DataManager.DataManager.SharedInstance.SelectedAccount);
                            InvokeOnMainThread(() =>
                            {
                                AccountStatusCache.AddAccountStatusData(accountStatusResponse);
                                SetDisconnectionComponent(false);

                                accountIsSSMR = false;
                                if (AccountStatusCache.AccountStatusIsAvailable() && isNormalChart)
                                {
                                    List<string> accounts = new List<string>
                                    {
                                        DataManager.DataManager.SharedInstance.SelectedAccount.accNum
                                    };

                                    var ssmrEligible = _dashboardHomeHelper.FilterAccountNoForSSMR(accounts, DataManager.DataManager.SharedInstance.AccountRecordsList.d);
                                    if (ssmrEligible != null && ssmrEligible.Count > 0)
                                    {
                                        InvokeInBackground(async () =>
                                        {
                                            SMRAccountStatusResponseModel response = await ServiceCall.GetAccountsSMRStatus(accounts);
                                            InvokeOnMainThread(() =>
                                            {
                                                if (response != null &&
                                                    response.d != null &&
                                                    response.d.IsSuccess &&
                                                    response.d.data != null &&
                                                    response.d.data.Count > 0)
                                                {
                                                    SMRAccountStatusModel statusDetails = response.d.data[0];
                                                    if (statusDetails != null)
                                                    {
                                                        string accountNo = DataManager.DataManager.SharedInstance.SelectedAccount.accNum;
                                                        if (accountNo.Equals(statusDetails.ContractAccount))
                                                        {
                                                            if (statusDetails.isTaggedSMR)
                                                            {
                                                                SSMRUsageParallelAPICalls();
                                                            }
                                                            else
                                                            {
                                                                CallGetAccountUsageAPI(accNum);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            CallGetAccountUsageAPI(accNum);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        CallGetAccountUsageAPI(accNum);
                                                    }
                                                }
                                                else
                                                {
                                                    CallGetAccountUsageAPI(accNum);
                                                }
                                            });
                                        });
                                    }
                                    else
                                    {
                                        CallGetAccountUsageAPI(accNum);
                                    }
                                }
                                else if (isNormalChart)
                                {
                                    CallGetAccountUsageAPI(accNum);
                                }
                            });
                        });
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }
        private void SSMRUsageParallelAPICalls()
        {
            accountIsSSMR = true;
            SetSSMRComponent(true, false);
            AccountUsageCache.ClearTariffLegendList();
            bool usageIsCached = false;
            InvokeInBackground(() =>
            {
                List<Task> taskList = new List<Task>();
                try
                {
                    taskList.Add(GetSMRAccountActivityInfo(DataManager.DataManager.SharedInstance.SelectedAccount));
                    if (AccountUsageCache.IsRefreshNeeded(DataManager.DataManager.SharedInstance.SelectedAccount.accNum))
                    {
                        taskList.Add(GetAccountUsage(DataManager.DataManager.SharedInstance.SelectedAccount, true));
                    }
                    else
                    {
                        usageIsCached = true;
                        AccountUsageCache.ClearTariffLegendList();
                        AccountUsageCache.GetCachedData(DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
                    }
                    Task.WaitAll(taskList.ToArray());

                    InvokeOnMainThread(() =>
                    {
                        bool ssmrInfoIsSuccess = false;
                        if (!usageIsCached)
                        {
                            AccountUsageCache.SetData(DataManager.DataManager.SharedInstance.SelectedAccount.accNum, _accountUsageResponse);
                        }
                        if (_smrAccountActivityInfoResponse != null &&
                               _smrAccountActivityInfoResponse.d != null &&
                               _smrAccountActivityInfoResponse.d.data != null &&
                               _smrAccountActivityInfoResponse.d.IsSuccess)
                        {
                            ssmrInfoIsSuccess = true;
                            SSMRActivityInfoCache.SetDashboardCache(_smrAccountActivityInfoResponse, DataManager.DataManager.SharedInstance.SelectedAccount);
                            SSMRActivityInfoCache.SetReadingHistoryCache(_smrAccountActivityInfoResponse, DataManager.DataManager.SharedInstance.SelectedAccount);
                        }

                        if ((AccountUsageCache.IsSuccess || usageIsCached) && ssmrInfoIsSuccess)
                        {
                            SetTariffButtonState();
                            SetTariffLegendComponent();
                            SetChartView(false);
                            SetSSMRComponent(false, false);
                            CheckTutorialOverlay();
                        }
                        else if (AccountUsageCache.IsSuccess || usageIsCached)
                        {
                            SetTariffButtonState();
                            SetTariffLegendComponent();
                            SetChartView(false);
                            HideSSMRView();
                        }
                        else if (ssmrInfoIsSuccess)
                        {
                            if (AccountUsageCache.IsDataEmpty)
                            {
                                SetEmptyDataComponent(AccountUsageCache.EmptyDataMessage);
                                SetSSMRComponent(false, false);
                                CheckTutorialOverlay();
                            }
                            else
                            {
                                SetRefreshScreen();
                                SetSSMRComponent(false, true);
                                HideREAmountView();
                            }
                        }
                        else
                        {
                            SetRefreshScreen();
                            HideSSMRView();
                            HideREAmountView();
                        }
                    });
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error in services: " + e.Message);
                    InvokeOnMainThread(() =>
                    {
                        SetRefreshScreen();
                        HideSSMRView();
                        HideREAmountView();
                    });
                }
            });
        }
        #endregion
    }
}