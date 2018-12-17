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
using myTNB_Android.Src.Database.Model;
using Refit;
using myTNB_Android.Src.myTNBMenu.Models;

namespace myTNB_Android.Src.SelectSupplyAccount.MVP
{
    public class SelectSupplyAccountContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            void ShowList(List<CustomerBillingAccount> customerBillingAccountList);

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

            /// <summary>
            /// Show no internet connection
            /// </summary>
            void ShowNoInternetConnection();

            /// <summary>
            /// Show query error from api response
            /// </summary>
            /// <param name="errorMessage">string</param>
            void ShowQueryError(string errorMessage);

            /// <summary>
            /// Show dashboard chart
            /// </summary>
            /// <param name="response">UsageHistoryResponse</param>
            /// <param name="accountData">AccountData</param>
            void ShowDashboardChart(UsageHistoryResponse response, AccountData accountData);

            /// <summary>
            /// Show progress dialog
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            /// Hide progress dialog
            /// </summary>
            void HideShowProgressDialog();

            /// <summary>
            /// Returns connectivity
            /// </summary>
            /// <returns>bool</returns>
            bool HasInternetConnection();

#if STUB || DEVELOP
            string GetUsageHistoryStub();

            string GetAccountDetailsStub(string accNum);
#endif

        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to select account
            /// </summary>
            /// <param name="selectedCustomerBilling">CustomerBillingAccount</param>
            void OnSelectAccount(CustomerBillingAccount selectedCustomerBilling);

        }
    }
}