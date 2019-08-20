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
            CallGetAccountUsageAPI();
            CallGetAccountDueAmountAPI();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public void OnCurrentBillViewDone()
        {
            Debug.WriteLine("OnCurrentBillViewDone()");
        }


        #region OVERRIDDEN Methods
        internal override void OnCurrentBillButtonTap()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
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
                        ActivityIndicator.Show();
                        AccountUsageCache.ClearTariffLegendList();
                        AccountUsageResponseModel accountUsageResponse = await UsageServiceCall.GetAccountUsage(DataManager.DataManager.SharedInstance.SelectedAccount);
                        AccountUsageCache.AddTariffLegendList(accountUsageResponse);
                        AccountUsageCache.SetData(accountUsageResponse);
                        AddSubviews();
                        SetTariffLegendComponent();
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                    ActivityIndicator.Hide();
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
                        UpdateFooterUI(true);
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
                            UpdateFooterUI(false);
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
