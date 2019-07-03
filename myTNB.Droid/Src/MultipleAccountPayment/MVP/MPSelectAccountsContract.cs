using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using System.Collections.Generic;

namespace myTNB_Android.Src.MultipleAccountPayment.MVP
{
    public class MPSelectAccountsContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {

            ///<summary>
            ///Disable pay button
            /// </summary>
            void DisablePayButton();

            ///<summary>
            ///Enable pay button
            /// </summary>
            void EnablePayButton();

            /// <summary>
            /// 
            /// Update total on selected accounts 
            /// </summary>
            void UpdateTotal(List<MPAccount> selectedAccounts);

            /// <summary>
            /// Check if amount is > 5000 to inform user 
            /// </summary>
            /// <param name="amount"></param>
            /// <returns></returns>
            void IsValidAmount(double amount);

            /// <summary>
            /// Show error message to user if amount is > 5000 (which can be only paid by FPX)
            /// </summary>
            /// <param name="messge"></param>
            void ShowError(string messge);

            /// <summary>
            /// Navigate to next screen
            /// </summary>
            void NavigateToPayment();

            /// <summary>
            /// Show progress dialog for get registered cards 
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            /// Hide progress dialog for get registered cards 
            /// </summary>
            void HideProgressDialog();


            /// <summary>
            /// Show due amount for all accounts
            /// </summary>
            void GetAccountDueAmountResult(MPAccountDueResponse response);

            /// <summary>
            /// Show due amount for all accounts from stored accounts
            /// </summary>
            void GetAccountDueAmountResult(List<MPAccount> accounts);

            void ShowDashboardChart(AccountData accountData);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Get due amount for all accounts
            /// </summary>
            void GetMultiAccountDueAmount(string apiKeyID, List<string> accounts, string preSelectedAccount);

            void OnSelectAccount(CustomerBillingAccount selectedCustomerBilling);
        }
    }
}