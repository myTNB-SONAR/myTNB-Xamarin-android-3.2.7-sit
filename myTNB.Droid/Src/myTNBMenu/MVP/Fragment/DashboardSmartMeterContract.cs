using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using Refit;
using System;
using myTNB_Android.Src.AppLaunch.Models;
using static myTNB_Android.Src.myTNBMenu.Models.GetInstallationDetailsResponse;

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class DashboardSmartMeterContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show by day chart
            /// </summary>
            void ShowByDay();

            /// <summary>
            /// Show by month chart
            /// </summary>
            void ShowByMonth();

            /// <summary>
            /// Show by hour chart
            /// </summary>
            void ShowByHour();

            /// <summary>
            /// Show ViewBill
            /// </summary>
            void ShowViewBill(BillHistoryV5 selectedBill = null);

            /// <summary>
            /// Show Payment
            /// </summary>
            void ShowPayment();

            /// <summary>
            /// Show not available data when day tab is clicked
            /// </summary>
            void ShowNotAvailableDayData();

            /// <summary>
            /// Show no internet
            /// </summary>
            void ShowNoInternet();

            void ShowNoInternetWithWord(string contentTxt, string buttonTxt);

            /// <summary>
            ///  Show no internet snackbar
            /// </summary>
            void ShowNoInternetSnackbar();

            /// <summary>
            /// Returns connectivity
            /// true has no internet
            /// false has internet
            /// </summary>
            /// <returns>bool</returns>
            bool HasNoInternet();

            /// <summary>
            /// Returns current chart is by day
            /// </summary>
            /// <returns>bool</returns>
            bool IsByDay();

            /// <summary>
            /// Returns day is empty
            /// </summary>
            /// <returns>bool</returns>
            bool IsByDayEmpty();

            /// <summary>
            /// Used for chart pagination in day
            /// Returns the current index of the array
            /// </summary>
            /// <returns>integer</returns>
            int GetCurrentParentIndex();

            /// <summary>
            /// Returns max day count
            /// </summary>
            /// <returns></returns>
            int GetMaxParentIndex();

            /// <summary>
            /// Sets the chart pagination in day
            /// </summary>
            /// <param name="newIndex">integer</param>
            void SetCurrentParentIndex(int newIndex);

            /// <summary>
            /// Enable left pagination button
            /// </summary>
            /// <param name="show">bool</param>
            void EnableLeftArrow(bool show);

            /// <summary>
            /// Enable right pagination button
            /// </summary>
            /// <param name="show"></param>
            void EnableRightArrow(bool show);

            /// <summary>
            /// Show tab refresh
            /// </summary>
            void ShowTapRefresh();

            /// <summary>
            /// Show notification list
            /// </summary>
            void ShowNotification();

            /// <summary>
            /// Show bottom view amount progress
            /// </summary>
            void ShowAmountProgress();

            /// <summary>
            /// Show learn more
            /// </summary>
            /// <param name="weblink">Weblink</param>
            void ShowLearnMore(Weblink weblink);

            /// <summary>
            /// Hide bottom view amount progress
            /// </summary>
            void HideAmountProgress();

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

            /// <summary>
            /// Show Account Status
            /// </summary>
            /// <param name="accountStatus">AccountStatusData</param>
            void ShowAccountStatus(AccountStatusData accountStatus);

            void ShowLoadBillRetryOptions();

            /// <summary>
            /// Shows a cancelled exception with an option to retry
            /// </summary>
            /// <param name="operationCanceledException">the returned exception</param>
            void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException);

            /// <summary>
            /// Shows an api exception with an option to retry
            /// </summary>
            /// <param name="apiException">the returned exception</param>
            void ShowRetryOptionsApiException(ApiException apiException);

            /// <summary>
            /// Shows an unknown exception with an option to retry
            /// </summary>
            /// <param name="exception">the returned exception</param>
            void ShowRetryOptionsUnknownException(Exception exception);

            string GetDeviceId();

        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action by day
            /// </summary>
            void OnByDay();

            /// <summary>
            /// Action by month
            /// </summary>
            void OnByMonth();

            /// <summary>
            /// Action by hour
            /// </summary>
            void OnByHour();

            /// <summary>
            /// Action to navigate to view bill
            /// </summary>
            void OnViewBill(AccountData selectedAccount);

            /// <summary>
            /// Action to navigate to pay
            /// </summary>
            void OnPay();

            /// <summary>
            /// Action left pagination
            /// </summary>
            void OnArrowBackClick();

            /// <summary>
            /// Action right pagination
            /// </summary>
            void OnArrowForwardClick();

            /// <summary>
            /// Action on Tap refresh
            /// </summary>
            void OnTapRefresh();

            /// <summary>
            /// Action to navigate to notification
            /// </summary>
            void OnNotification();

            /// <summary>
            /// Action to load amount of account
            /// </summary>
            /// <param name="accountNum">string</param>
            void OnLoadAmount(string accountNum);

            /// <summary>
            /// Action to navigate to learn more
            /// </summary>
            void OnLearnMore();

            /// <summary>
            /// Action to get Account Status
            /// </summary>
            /// <param name="accountNum">string</param>
            void GetAccountStatus(string accountNum);

        }
    }
}
