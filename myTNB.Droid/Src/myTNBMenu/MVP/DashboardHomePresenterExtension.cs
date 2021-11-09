using System.Collections.Generic;
using myTNB.Mobile;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.myTNBMenu.MVP
{
    internal static class DashboardHomePresenterExtension
    {
        internal static void GetBillEligibilityCheck(this DashboardHomePresenter presenter, string accountNumber)
        {
            if (CAIsInTheList(accountNumber, out CustomerBillingAccount account))
            {
                if (BillRedesignUtility.Instance.IsAccountStatementEligible(accountNumber, account.isOwned))
                {
                    presenter.mView.NavigateToViewAccountStatement(account);
                }
                else
                {
                    presenter.mView.TriggerIneligiblePopUp();
                }
            }
            else
            {
                presenter.mView.NavigateToAddAccount();
            }
        }

        private static bool CAIsInTheList(string accountNumber, out CustomerBillingAccount account)
        {
            account = null;
            List<CustomerBillingAccount> accountList = CustomerBillingAccount.List();
            if (accountList.Count > 0)
            {
                CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(accountNumber);
                account = customerBillingAccount;
                return customerBillingAccount != null;
            }
            return false;
        }
    }
}
