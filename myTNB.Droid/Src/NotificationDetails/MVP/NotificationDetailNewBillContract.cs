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

namespace myTNB_Android.Src.NotificationDetails.MVP
{
    public class NotificationDetailNewBillContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show selected account payment 
            /// </summary>
            /// <param name="selectedAccount">AccountData</param>
            void ShowPayment(AccountData selectedAccount);

            /// <summary>
            /// Show selected account details
            /// </summary>
            /// <param name="selectedAccount">AccountData</param>
            void ShowDetails(AccountData selectedAccount);

            /// <summary>
            /// Show progress dialog
            /// </summary>
            void ShowRetrievalProgress();

            /// <summary>
            /// Hide progress dialog
            /// </summary>
            void HideRetrievalProgress();

            /// <summary>
            /// Show month 
            /// </summary>
            void ShowMonthWildCard();

            /// <summary>
            /// Show billdated 
            /// </summary>
            void ShowBillDatedWildcard();

            /// <summary>
            /// Show total outstanding amount
            /// </summary>
            void ShowTotalOutstandingAmtWildcard();

            /// <summary>
            /// Show payment due
            /// </summary>
            void ShowPaymentDueWildcard();

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
            /// Action to navigate payment
            /// </summary>
            /// <param name="notificationDetails">NotificationDetails.Models.NotificationDetails</param>
            void OnPayment(NotificationDetails.Models.NotificationDetails notificationDetails);

            /// <summary>
            /// Action to navigate view details
            /// </summary>
            /// <param name="notificationDetails">NotificationDetails.Models.NotificationDetails</param>
            void OnViewDetails(NotificationDetails.Models.NotificationDetails notificationDetails);
        }
    }
}