using System.Collections.Generic;
using myTNB;
using myTNB.Mobile;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;

namespace myTNB_Android.Src.myTNBMenu.MVP
{
    internal static class DashboardHomePresenterExtension
    {
        internal static void GetBillEligibilityCheck(this DashboardHomePresenter presenter, string accountNumber)
        {
            if (BillRedesignUtility.Instance.IsCAEligible(accountNumber))
            {
                if (CAIsInTheList(accountNumber, out CustomerBillingAccount account))
                {
                    bool isEligibleForNonOwner = LanguageManager.Instance.GetConfigToggleValue(LanguageManager.ConfigPropertyEnum.ShouldShowAccountStatementToNonOwner);
                    if (isEligibleForNonOwner)
                    {
                        presenter.mView.NavigateToViewAccountStatement(account);
                    }
                    else
                    {
                        ValidateRealOwnerAsync(presenter, account);
                    }
                }
                else
                {
                    presenter.mView.NavigateToAddAccount();
                }
            }
            else if (!CAIsInTheList(accountNumber, out CustomerBillingAccount account))
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

        private static async void ValidateRealOwnerAsync(this DashboardHomePresenter presenter, CustomerBillingAccount account)
        {
            presenter.mView.ShowProgressDialog();
            GetSearchForAccountRequest request = new GetSearchForAccountRequest(account.AccNum);

            var result = await ServiceApiImpl.Instance.ValidateAccIsExist(request);

            if (result != null &&
                result.GetSearchForAccount != null &&
                result.GetSearchForAccount.Count > 0)
            {
                presenter.mView.HideProgressDialog();
                var data = result.GetSearchForAccount[0];
                var ic = data.IC.Trim();
                var icAcct = UserEntity.GetActive().IdentificationNo.Trim();
                if (ic.Equals(icAcct))
                {
                    presenter.mView.NavigateToViewAccountStatement(account);
                }
                else
                {
                    presenter.mView.TriggerIneligiblePopUp();
                }
            }
            else if (account.isOwned)
            {
                presenter.mView.NavigateToViewAccountStatement(account);
            }
            else
            {
                presenter.mView.TriggerIneligiblePopUp();
            }
        }
    }
}
