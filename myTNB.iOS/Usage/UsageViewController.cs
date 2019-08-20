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
            PageName = "UsageView";
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (!DataManager.DataManager.SharedInstance.IsSameAccount)
            {
                CallGetAccountUsageAPI();
                CallGetAccountDueAmountAPI();
            }
        }

        public void OnCurrentBillViewDone()
        {
            Debug.WriteLine("OnCurrentBillViewDone()");
        }

        #region OVERRIDDEN Methods
        internal override void OnReadHistoryTap()
        {
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
                            viewController.OnDone = OnCurrentBillViewDone;
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
        internal override void OnSelectAccount(int index)
        {
            CallGetAccountStatusAPI();
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
                            AccountUsageCache.AddTariffLegendList(accountUsageResponse);
                            AccountUsageCache.SetData(DataManager.DataManager.SharedInstance.SelectedAccount.accNum, accountUsageResponse);
                        }
                        else
                        {
                            AccountUsageCache.ClearTariffLegendList();
                            AccountUsageCache.GetCachedData(DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
                        }
                        SetTariffLegendComponent();
                        SetChartView(false);
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }
        private void CallGetAccountDueAmountAPI()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        if (!isREAccount)
                        {
                            UpdateFooterUI(true);
                        }
                        else
                        {
                            UpdateREAmountViewUI(true);
                        }
                        var account = DataManager.DataManager.SharedInstance.SelectedAccount;
                        DueAmountResponseModel dueAmountResponse = await UsageServiceCall.GetAccountDueAmount(account);
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
                            if (!isREAccount)
                            {
                                UpdateFooterUI(false);
                            }
                            else
                            {
                                UpdateREAmountViewUI(false);
                            }
                        }
                        else
                        {
                            //TO DO: Add Fail Handling here...
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
                        ActivityIndicator.Show();
                        AccountStatusCache.ClearAccountStatusData();

                        AccountStatusResponseModel accountStatusResponse = await UsageServiceCall.GetAccountStatus(DataManager.DataManager.SharedInstance.SelectedAccount);
                        AccountStatusCache.AddAccountStatusData(accountStatusResponse);
                        SetDisconnectionComponent();

                        if (AccountStatusCache.AccountStatusIsAvailable())
                        {
                            if (accountIsSSMR)
                            {
                                GetSMRAccountActivityInfo();
                            }
                        }
                        ActivityIndicator.Hide();
                    }
                    else
                    {
                        ActivityIndicator.Hide();
                        DisplayNoDataAlert();
                    }
                });
            });
        }
        private void GetSMRAccountActivityInfo()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        SMRAccountActivityInfoResponseModel ssmrInfoResponse = await UsageServiceCall.GetSMRAccountActivityInfo(DataManager.DataManager.SharedInstance.SelectedAccount);
                        if (ssmrInfoResponse != null &&
                            ssmrInfoResponse.d != null &&
                            ssmrInfoResponse.d.data != null &&
                            ssmrInfoResponse.d.IsSuccess)
                        {
                            SSMRActivityInfoCache.SetDashboardCache(ssmrInfoResponse);
                        }
                        SetSSMRComponent();
                        ActivityIndicator.Hide();
                    }
                    else
                    {
                        ActivityIndicator.Hide();
                        DisplayNoDataAlert();
                    }
                });
            });
        }
        #endregion
    }
}
