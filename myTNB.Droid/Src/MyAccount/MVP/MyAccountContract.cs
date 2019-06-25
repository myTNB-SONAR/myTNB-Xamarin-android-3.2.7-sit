using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.myTNBMenu.Models;
using Refit;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.MyAccount.MVP
{
    public class MyAccountContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Shows the update mobile screen
            /// </summary>
            void ShowUpdateMobileNo();

            /// <summary>
            /// Show mobile no update success
            /// </summary>
            /// <param name="newPhoneNo">string</param>
            void ShowMobileUpdateSuccess(string newPhoneNo);

            /// <summary>
            /// Show password update success
            /// </summary>
            void ShowPasswordUpdateSuccess();

            /// <summary>
            /// Shows the update password screen
            /// </summary>
            void ShowUpdatePassword();

            /// <summary>
            /// Shows manage credit cards / debit cards screen
            /// </summary>
            void ShowManageCards(List<CreditCardData> cardsList);

            /// <summary>
            /// Show removed card success
            /// </summary>
            /// <param name="creditCard">CreditCardData</param>
            /// <param name="numOfCards">integer</param>
            void ShowRemovedCardSuccess(CreditCardData creditCard, int numOfCards);

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
            /// Shows Logout Screen
            /// </summary>
            void ShowLogout();

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

            /// <summary>
            /// Shows user data
            /// </summary>
            /// <param name="user">UserEntity</param>
            /// <param name="numOfCards">integer</param>
            void ShowUserData(UserEntity user, int numOfCards);

            /// <summary>
            /// Enable manage cards button
            /// </summary>
            void EnableManageCards();

            /// <summary>
            /// Disable manage cards button
            /// </summary>
            void DisableManageCards();

            /// <summary>
            /// Show logout error message from api response
            /// </summary>
            /// <param name="message">string</param>
            void ShowLogoutErrorMessage(string message);

            /// <summary>
            /// Show logout progress dialog
            /// </summary>
            void ShowLogoutProgressDialog();

            /// <summary>
            /// Hides logout progress dialog
            /// </summary>
            void HideLogoutProgressDialog();

            ///<summary>
            ///Show account removed success
            ///</summary>
            void ShowAccountRemovedSuccess();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to navigate to manage mobile
            /// </summary>
            void OnUpdateMobileNo();

            /// <summary>
            /// Action to navigate to manage password
            /// </summary>
            void OnUpdatePassword();

            /// <summary>
            /// Action to navigate manage cards
            /// </summary>
            void OnManageCards();

            /// <summary>
            /// Action to manage account
            /// </summary>
            /// <param name="customerBillingAccount">CustomerBillingAccount</param>
            /// <param name="position">integer</param>
            void OnManageSupplyAccount(CustomerBillingAccount customerBillingAccount, int position);

            /// <summary>
            /// Action to add accounts
            /// </summary>
            void OnAddAccount();

            /// <summary>
            /// Action to logout
            /// </summary>
            /// <param name="deviceId">string</param>
            void OnLogout(string deviceId);

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