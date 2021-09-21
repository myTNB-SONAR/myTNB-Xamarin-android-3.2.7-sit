using System.Collections.Generic;
using myTNB.Mobile;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.myTNBMenu.MVP
{
    internal static class DashboardHomePresenterExtension
    {
        internal static void GetBillEligibilityCheck(this DashboardHomePresenter presenter, string accountNumber)
        {
            if (EligibilitySessionCache.Instance.IsFeatureEligible(EligibilitySessionCache.Features.DBR
                    , EligibilitySessionCache.FeatureProperty.Enabled))
            {
                if (EligibilitySessionCache.Instance.IsFeatureEligible(EligibilitySessionCache.Features.DBR,
                    EligibilitySessionCache.FeatureProperty.TargetGroup))
                {
                    if (CAIsInTheList(accountNumber, out CustomerBillingAccount account))
                    {
                        if (BillRedesignUtility.Instance.IsCAEligible(accountNumber))
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
                else
                {
                    if (CAIsInTheList(accountNumber, out CustomerBillingAccount account))
                    {
                        if (account.IsSmartMeter && account.isOwned)
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
