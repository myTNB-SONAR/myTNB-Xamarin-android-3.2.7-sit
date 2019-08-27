using myTNB.Model;
using myTNB.Model.Usage;
using System;
using System.Diagnostics;
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
                CallGetAccountStatusAPI();
                if (isSmartMeterAccount)
                {
                    CallGetAccountUsageSmartAPI();
                }
                else
                {
                    CallGetAccountUsageAPI();
                }
                CallGetAccountDueAmountAPI(DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
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
                var navController = new UINavigationController(viewController);
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
                var navController = new UINavigationController(viewController);
                PresentViewController(navController, true, null);
            }
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
                            var navController = new UINavigationController(viewController);
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
                            var navController = new UINavigationController(selectBillsVC);
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
        private void CallGetAccountUsageAPI()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        if (AccountUsageCache.IsRefreshNeeded(DataManager.DataManager.SharedInstance.SelectedAccount.accNum))
                        {
                            AccountUsageCache.ClearTariffLegendList();
                            AccountUsageResponseModel accountUsageResponse = await UsageServiceCall.GetAccountUsage(DataManager.DataManager.SharedInstance.SelectedAccount);
                            AccountUsageCache.SetData(DataManager.DataManager.SharedInstance.SelectedAccount.accNum, accountUsageResponse);
                            if (AccountUsageCache.IsSuccess)
                            {
                                SetTariffLegendComponent();
                                SetChartView(false);
                            }
                            else
                            {
                                SetRefreshScreen();
                                if (isREAccount)
                                {
                                    SetREAmountViewForRefresh();
                                }
                                CallGetSMRAccountActivityInfo(true);
                            }
                        }
                        else
                        {
                            AccountUsageCache.ClearTariffLegendList();
                            AccountUsageCache.GetCachedData(DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
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
        private void CallGetAccountUsageSmartAPI()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        if (AccountUsageSmartCache.IsRefreshNeeded(DataManager.DataManager.SharedInstance.SelectedAccount.accNum))
                        {
                            SetSmartMeterComponent(true);
                            AccountUsageSmartCache.ClearTariffLegendList();
                            AccountUsageSmartResponseModel accountUsageSmartResponse = await UsageServiceCall.GetAccountUsageSmart(DataManager.DataManager.SharedInstance.SelectedAccount);
                            AccountUsageSmartCache.SetData(DataManager.DataManager.SharedInstance.SelectedAccount.accNum, accountUsageSmartResponse);
                            if (AccountUsageSmartCache.IsSuccess)
                            {
                                OtherUsageMetricsModel model = AccountUsageSmartCache.GetUsageMetrics();
                                SetSmartMeterComponent(false, model.Cost);
                                SetTariffLegendComponent();
                                SetChartView(false);
                            }
                            else
                            {
                                SetRefreshScreen();
                            }
                        }
                        else
                        {
                            AccountUsageSmartCache.ClearTariffLegendList();
                            AccountUsageSmartCache.GetCachedData(DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
                            OtherUsageMetricsModel model = AccountUsageSmartCache.GetUsageMetrics();
                            SetSmartMeterComponent(false, model.Cost);
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
                InvokeOnMainThread(async () =>
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
                        DueAmountResponseModel dueAmountResponse = await UsageServiceCall.GetAccountDueAmount(account);
                        if (accNum == DataManager.DataManager.SharedInstance.SelectedAccount.accNum)
                        {
                            if (dueAmountResponse != null &&
                            dueAmountResponse.d != null &&
                            dueAmountResponse.d.didSucceed &&
                            dueAmountResponse.d.data != null)
                            {
                                var model = dueAmountResponse.d.data;
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
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }
        public void CallGetAccountStatusAPI()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        SetDisconnectionComponent(true);
                        AccountStatusCache.ClearAccountStatusData();

                        AccountStatusResponseModel accountStatusResponse = await UsageServiceCall.GetAccountStatus(DataManager.DataManager.SharedInstance.SelectedAccount);
                        AccountStatusCache.AddAccountStatusData(accountStatusResponse);
                        SetDisconnectionComponent(false);

                        if (AccountStatusCache.AccountStatusIsAvailable())
                        {
                            if (!isREAccount && accountIsSSMR)
                            {
                                CallGetSMRAccountActivityInfo();
                            }
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }
        private void CallGetSMRAccountActivityInfo(bool isForRefreshScreen = false)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        SetSSMRComponent(true, isForRefreshScreen);
                        SMRAccountActivityInfoResponseModel ssmrInfoResponse = await UsageServiceCall.GetSMRAccountActivityInfo(DataManager.DataManager.SharedInstance.SelectedAccount);
                        if (ssmrInfoResponse != null &&
                            ssmrInfoResponse.d != null &&
                            ssmrInfoResponse.d.data != null &&
                            ssmrInfoResponse.d.IsSuccess)
                        {
                            SSMRActivityInfoCache.SetDashboardCache(ssmrInfoResponse, DataManager.DataManager.SharedInstance.SelectedAccount);
                            SSMRActivityInfoCache.SetReadingHistoryCache(ssmrInfoResponse, DataManager.DataManager.SharedInstance.SelectedAccount);
                            SetSSMRComponent(false, isForRefreshScreen);
                        }
                        else
                        {
                            HideSSMRView();
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }
        #endregion
    }
}
