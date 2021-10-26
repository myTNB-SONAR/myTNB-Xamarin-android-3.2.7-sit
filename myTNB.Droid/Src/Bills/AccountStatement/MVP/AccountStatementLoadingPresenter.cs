using System;
using myTNB_Android.Src.myTNBMenu.Models;

namespace myTNB_Android.Src.Bills.AccountStatement.MVP
{
    public class AccountStatementLoadingPresenter : AccountStatementLoadingContract.IUserActionsListener
    {
        private readonly AccountStatementLoadingContract.IView view;

        private AccountData selectedAccount;
        private string preferredMonths;

        public AccountStatementLoadingPresenter(AccountStatementLoadingContract.IView view)
        {
            this.view = view;
            this.view?.SetPresenter(this);
        }

        public void OnInitialize()
        {
            this.view?.SetUpViews();
        }

        public void Start() { }

        public void SetSelectedAccount(AccountData accountData)
        {
            this.selectedAccount = accountData;
        }

        public AccountData GetSelectedAccount()
        {
            return this.selectedAccount;
        }

        public void SetPreferredMonths(string value)
        {
            this.preferredMonths = value;
        }
    }
}
