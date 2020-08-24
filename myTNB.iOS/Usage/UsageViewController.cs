using Foundation;
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
                if (isREAccount)
                {
                    CallGetAccountStatusAPI(accNum);
                    CallGetAccountUsageAPI(accNum);
                }
                else if (isSmartMeterAccount)
                {
                    CallGetAccountStatusAPI(accNum);
                    CallGetAccountUsageSmartAPI(accNum);
                }
                else
                {
                    CallGetAccountStatusAPI(accNum);
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
                viewController.IsFromUsage = true;
                viewController.AccountNumber = DataManager.DataManager.SharedInstance.SelectedAccount.accNum ?? string.Empty;
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            }
        }
        internal override void OnSubmitMeterTap()
        {
            base.OnSubmitMeterTap();
            if (SSMRActivityInfoCache.DashboardPreviousReading != null &&
                SSMRActivityInfoCache.DashboardPreviousReading.Count > 0)
            {
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
            else
            {
                var title = SSMRActivityInfoCache.DashboardDataModel.DisplayTitle;
                var msg = SSMRActivityInfoCache.DashboardDataModel.DisplayMessage;
                string displayTitle = (!string.IsNullOrEmpty(title) && !string.IsNullOrWhiteSpace(title)) ? title : GetI18NValue(UsageConstants.I18N_PrevReadingEmptyTitle);
                string displayMsg = (!string.IsNullOrEmpty(msg) && !string.IsNullOrWhiteSpace(msg)) ? msg : GetI18NValue(UsageConstants.I18N_PrevReadingEmptyMsg);

                DisplayCustomAlert(displayTitle, displayMsg, GetCommonI18NValue(Constants.Common_GotIt), null);
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
                        NormalChartIsLoading = true;
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
                                        else if (AccountUsageCache.IsPlannedDownTime)
                                        {
                                            SetDowntimeScreen();
                                            if (isREAccount)
                                            {
                                                SetREAmountViewForRefresh();
                                            }
                                            else
                                            {
                                                HideREAmountView();
                                            }
                                            SetContentViewForRefresh();
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
                                            SetContentViewForRefresh();
                                        }
                                        NormalChartIsLoading = false;
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
                            NormalChartIsLoading = false;
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
                        SMChartIsLoading = true;
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
                                            _smartMeterIsAvailable = true;
                                            OtherUsageMetricsModel model = AccountUsageSmartCache.GetUsageMetrics();
                                            if (AccountUsageSmartCache.IsMDMSDown)
                                            {
                                                HideSmartMeterComponent();
                                            }
                                            else
                                            {
                                                SetSmartMeterComponent(false, model.Cost);
                                            }
                                            SetTariffButtonState();
                                            SetTariffLegendComponent();
                                            SetChartView(false);
                                        }
                                        else if (AccountUsageSmartCache.IsDataEmpty)
                                        {
                                            _smartMeterIsAvailable = false;
                                            HideSmartMeterComponent();
                                            SetEmptyDataComponent(AccountUsageSmartCache.EmptyDataMessage);
                                        }
                                        else if (AccountUsageSmartCache.IsPlannedDownTime)
                                        {
                                            _smartMeterIsAvailable = false;
                                            SetDowntimeScreen();
                                            SetContentViewForRefresh();
                                            HideREAmountView();
                                            HideSSMRViewForRefresh();
                                        }
                                        else
                                        {
                                            _smartMeterIsAvailable = false;
                                            SetRefreshScreen();
                                            SetContentViewForRefresh();
                                            HideREAmountView();
                                            HideSSMRViewForRefresh();
                                        }
                                        SMChartIsLoading = false;
                                    }

                                });
                            });
                        }
                        else
                        {
                            _smartMeterIsAvailable = true;
                            AccountUsageSmartCache.ClearTariffLegendList();
                            AccountUsageSmartCache.GetCachedData(DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
                            OtherUsageMetricsModel model = AccountUsageSmartCache.GetUsageMetrics();
                            SetSmartMeterComponent(false, model.Cost);
                            SetTariffButtonState();
                            SetTariffLegendComponent();
                            SetChartView(false);
                            SMChartIsLoading = false;
                        }
                    }
                    else
                    {
                        _smartMeterIsAvailable = false;
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
                            bool hasPendingPayment = false;
                            if (!isREAccount)
                            {
                                PendingPaymentResponseModel pendingPaymentResponse = await UsageServiceCall.CheckPendingPayments(new List<string> { account.accNum ?? string.Empty });
                                hasPendingPayment =
                                            pendingPaymentResponse != null && pendingPaymentResponse.d != null &&
                                            pendingPaymentResponse.d.IsSuccess && pendingPaymentResponse.d.data != null &&
                                            pendingPaymentResponse.d.data.Count > 0 && pendingPaymentResponse.d.data[0].HasPendingPayment;
                            }
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
                                            IncrementREDueDateByDays = model.IncrementREDueDateByDays,
                                            IsPayEnabled = dueAmountResponse.d.IsPayEnabled,
                                            ShowEppToolTip = model.ShowEppToolTip //Created by Syahmi ICS 05052020
                                        };
                                        AmountDueCache.SaveDues(item);
                                        if (isREAccount)
                                        {
                                            UpdateREAmountViewUI(false);
                                        }
                                        else
                                        {
                                            UpdateFooterUI(false, hasPendingPayment);
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
                                            UpdateFooterUI(false, hasPendingPayment);
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
                                if (AccountStatusCache.AccountStatusIsAvailable() && isNormalChart && !isSmartMeterAccount && !isREAccount)
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
                                                    response.d.IsSuccess)
                                                {
                                                    if (response.d.data != null && response.d.data.Count > 0)
                                                    {
                                                        SMRAccountStatusModel statusDetails = response.d.data[0];
                                                        if (statusDetails != null)
                                                        {
                                                            string accountNo = DataManager.DataManager.SharedInstance.SelectedAccount.accNum;
                                                            if (accountNo.Equals(statusDetails.ContractAccount))
                                                            {
                                                                DataManager.DataManager.SharedInstance.UpdateDueIsSSMR(accountNo, statusDetails.IsTaggedSMR);
                                                                if (statusDetails.isTaggedSMR)
                                                                {
                                                                    _smrIsAvailable = true;
                                                                    SSMRUsageParallelAPICalls();
                                                                }
                                                                else
                                                                {
                                                                    _smrIsAvailable = false;
                                                                    CallGetAccountUsageAPI(accNum);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                _smrIsAvailable = false;
                                                                CallGetAccountUsageAPI(accNum);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            _smrIsAvailable = false;
                                                            CallGetAccountUsageAPI(accNum);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        _smrIsAvailable = false;
                                                        CallGetAccountUsageAPI(accNum);
                                                    }
                                                }
                                                else if (response.d.IsPlannedDownTime || response.d.IsUnplannedDownTime)
                                                {
                                                    _smrIsAvailable = false;
                                                    CallGetAccountUsageAPI(accNum);
                                                }
                                                else
                                                {
                                                    _smrIsAvailable = false;
                                                    CallGetAccountUsageAPI(accNum);
                                                }
                                            });
                                        });
                                    }
                                    else
                                    {
                                        _smrIsAvailable = false;
                                        CallGetAccountUsageAPI(accNum);
                                    }
                                }
                                else if (isNormalChart && !isSmartMeterAccount && !isREAccount)
                                {
                                    _smrIsAvailable = false;
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
            NormalChartIsLoading = true;
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
                            else if (AccountUsageCache.IsPlannedDownTime)
                            {
                                SetDowntimeScreen();
                                SetSSMRComponent(false, true);
                                HideREAmountView();
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
                        NormalChartIsLoading = false;
                    });
                }
                catch (MonoTouchException m)
                {
                    Debug.WriteLine("Error in services: " + m.Message);
                    InvokeOnMainThread(() =>
                    {
                        SetRefreshScreen();
                        HideSSMRView();
                        HideREAmountView();
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