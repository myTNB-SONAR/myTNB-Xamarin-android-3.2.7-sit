using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB.Android.Src.Base.MVP;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.ManageCards.Models;
using myTNB.Android.Src.myTNBMenu.Models;
using Refit;
using System;
using System.Collections.Generic;

namespace myTNB.Android.Src.MyAccount.MVP
{
    public class MyAccountContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            void ShowDeleteMessageResponse(bool click);

            void ShowErrorMessageResponse(string error);

            /// <summary>
            /// Show Account Data per list item row click 
            /// </summary>
            /// <param name="accountData"></param>
            void ShowManageSupplyAccount(AccountData accountData, int position);

            /// <summary>
            /// Shows removed account success
            /// </summary>
            /// <param name="accountData">AccountData</param>
            /// <param name="position">integer</param>
            void ShowRemovedSupplyAccountSuccess(AccountData accountData, int position);

            /// <summary>
            /// Shows add account screen
            /// </summary>
            void ShowAddAccount();

            /// <summary>
            /// Shows an api exception with an option to retry
            /// </summary>
            /// <param name="apiException">the returned exception</param>
            void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException);

            /// <summary>
            /// Shows an unknown exception with an option to retry
            /// </summary>
            /// <param name="unkownException">the returned exception</param>
            void ShowRetryOptionsApiException(ApiException apiException);

            /// <summary>
            /// Shows an unknown exception with an option to retry
            /// </summary>
            /// <param name="exception">the returned exception</param>
            void ShowRetryOptionsUnknownException(Exception exception);

            /// <summary>
            /// Shows a progress dialog
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            /// Hide progress dialog
            /// </summary>
            void HideShowProgressDialog();

            /// <summary>
            /// Shows get cards progress dialog
            /// </summary>
            void ShowGetCardsProgressDialog();

            /// <summary>
            /// Hide get cards progress dialog
            /// </summary>
            void HideGetCardsProgressDialog();

            /// <summary>
            /// Clears the account adapter
            /// </summary>
            void ClearAccountsAdapter();

            /// <summary>
            /// Shows account list
            /// </summary>
            /// <param name="accountList">List<paramref name="CustomerBillingAccount"/></param>
            void ShowAccountList(List<CustomerBillingAccount> accountList);

            /// <summary>
            /// Shows an empty account view
            /// </summary>
            void ShowEmptyAccount();

            ///<summary>
            ///Show account removed success
            ///</summary>
            void ShowAccountRemovedSuccess();
            void showsuccessDelete();
        }

        public interface IUserActionsListener : IBasePresenter
        {

            void OnRemoveAccount(string numacc, bool isOwner, bool IsInManageAccessList);

            /// <summary>
            /// Action to add accounts
            /// </summary>
            void OnAddAccount();

            /// <summary>
            /// The returned result from another activity
            /// </summary>
            /// <param name="requestCode">integer</param>
            /// <param name="resultCode">enum</param>
            /// <param name="data">intent</param>
            void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data);


        }
    }
}