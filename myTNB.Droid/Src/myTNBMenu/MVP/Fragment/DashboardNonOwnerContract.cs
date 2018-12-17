using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class DashboardNonOwnerContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show get access form
            /// </summary>
            void ShowGetAccessForm();

            /// <summary>
            /// Show select payment screen
            /// </summary>
            void ShowSelectPaymentScreen();

            /// <summary>
            /// Show notification
            /// </summary>
            void ShowNotification();

            /// <summary>
            /// Returns connectivity
            /// </summary>
            /// <returns>bool</returns>
            bool HasInternet();

            /// <summary>
            /// Show no internet snackbar
            /// </summary>
            void ShowNoInternetSnackbar();

            /// <summary>
            /// Show bottom view amount progress 
            /// </summary>
            void ShowAmountProgress();

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
            /// Show view bill
            /// </summary>
            void ShowViewBill(BillHistoryV5 selectedBill = null);

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
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to navigate on GetAccess
            /// </summary>
            void OnGetAccess();

            /// <summary>
            /// Action to navigate to ViewBill
            /// </summary>
            void OnViewBill(AccountData selectedAccount);

            /// <summary>
            /// Action to navigate to Pay
            /// </summary>
            void OnPay();

            /// <summary>
            /// Action to navigate to Notification
            /// </summary>
            void OnNotification();

            /// <summary>
            /// Action to load amount by account
            /// </summary>
            /// <param name="accountNum">string</param>
            void OnLoadAmount(string accountNum);
        }
    }
}