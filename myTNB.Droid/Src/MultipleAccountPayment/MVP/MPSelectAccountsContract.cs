using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.MyTNBService.Model;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.MultipleAccountPayment.MVP
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

            void SetAccountsDueAmountResult(List<MPAccount> updatedAccountList);

            void ShowHasMinimumAmoutToPayTooltip(MPAccount accounts, AccountChargeModel accountChargeModel);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            void GetAccountsCharges(List<string> accountList, string preSelectedAccount);
        }
    }
}
