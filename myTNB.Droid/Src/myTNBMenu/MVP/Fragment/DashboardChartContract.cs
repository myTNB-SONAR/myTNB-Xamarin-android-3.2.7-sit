using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using Refit;
using System;
using myTNB_Android.Src.AppLaunch.Models;
using static myTNB_Android.Src.myTNBMenu.Models.GetInstallationDetailsResponse;

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class DashboardChartContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show by kWh chart
            /// </summary>
            void ShowByKwh();

            /// <summary>
            /// Show by RM chart
            /// </summary>
            void ShowByRM();

            /// <summary>
            /// Show ViewBill
            /// </summary>
            void ShowViewBill(BillHistoryV5 selectedBill = null);

            /// <summary>
            /// Show Payment
            /// </summary>
            void ShowPayment();

            /// <summary>
            /// Show no internet
            /// </summary>
            void ShowNoInternet(string contentTxt, string buttonTxt);

            /// <summary>
            ///  Show no internet snackbar
            /// </summary>
            void ShowNoInternetSnackbar();

            /// <summary>
            /// Show tab refresh
            /// </summary>
            void ShowTapRefresh();

            /// <summary>
            /// Show notification list
            /// </summary>
            void ShowNotification();

            /// <summary>
            /// Show amount due
            /// </summary>
            /// <param name="accountDueAmount">AccountDueAmount</param>
            void ShowAmountDue(AccountDueAmount accountDueAmount);

            /// <summary>
            /// Enable pay button
            /// </summary>
            void EnablePayButton();

            /// <summary>
            /// Disable pay button
            /// </summary>
            void DisablePayButton();

            void ShowLoadBillRetryOptions();

            /// <summary>
            /// Show Account Status
            /// </summary>
            /// <param name="accountStatus">AccountStatusData</param>
            void ShowAccountStatus(AccountStatusData accountStatus);

            void ShowSSMRDashboardView(SMRActivityInfoResponse response);

            void HideSSMRDashboardView();

            void ShowDisconnectionRetrySnakebar();

            void ShowSMRRetrySnakebar();

            string GetDeviceId();

            void ShowProgress();

            void HideProgress();

            bool isSMDataError();

            bool IsBCRMDownFlag();

            bool IsLoadUsageNeeded();

            AccountData GetSelectedAccount();

            void DisableViewBillButton();

            void EnableViewBillButton();

            void ShowAmountDueFailed();

            void SetUsageData(UsageHistoryData data);

            UsageHistoryData GetUsageHistoryData();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action by Kwh
            /// </summary>
            void OnByKwh();

            /// <summary>
            /// Action by RM
            /// </summary>
            void OnByRM();

            /// <summary>
            /// Action to navigate to view bill
            /// </summary>
            void OnViewBill(AccountData selectedAccount);

            /// <summary>
            /// Action to navigate to pay
            /// </summary>
            void OnPay();

            /// <summary>
            /// Action on Tap refresh
            /// </summary>
            void OnTapRefresh();

            /// <summary>
            /// Action to navigate to notification
            /// </summary>
            void OnNotification();

            /// <summary>
            /// Action to navigate to learn more
            /// </summary>
            void OnLearnMore();

            bool IsOwnedSMR(string accountNumber);

            bool IsBillingAvailable();

        }
    }
}
