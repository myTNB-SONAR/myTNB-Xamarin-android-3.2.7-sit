using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.ManageCards.Models;
using Refit;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.ManageCards.MVP
{
    public class ManageCardsContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {

            /// <summary>
            /// Shows account list
            /// </summary>
            /// <param name="accountList">List<paramref name="CustomerBillingAccount"/></param>
            void ShowAccountList(List<CustomerBillingAccount> accountList);

            /// <summary>
            /// Shows progress dialog
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            /// Hides progress dialog
            /// </summary>
            void HideProgressDialog();

            /// <summary>
            /// Shows an error message returned from api
            /// </summary>
            /// <param name="message">string</param>
            void ShowErrorMessage(string message);

            /// <summary>
            /// Shows users saved cards
            /// </summary>
            void ShowCards();

            /// <summary>
            /// Shows success dialog in removing cards
            /// </summary>
            /// <param name="RemovedCard">CreditCardData</param>
            /// <param name="position">integer</param>
            void ShowRemoveSuccess(CreditCardData RemovedCard, int position);

            /// <summary>
            /// Shows a removed card snackbar success and stays in the screen
            /// </summary>
            /// <param name="RemovedCard">CreditCardData</param>
            /// <param name="position">integer</param>
            void ShowSnackbarRemovedSuccess(CreditCardData RemovedCard, int position);

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
            /// Action removing card and goes back to previous activity
            /// </summary>
            /// <param name="Data">CreditCardData</param>
            /// <param name="position">integer</param>
            void OnRemove(CreditCardData Data, int position);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="RemovedCard"></param>
            /// <param name="position"></param>
            void OnRemoveStay(CreditCardData RemovedCard, int position);
        }
    }
}