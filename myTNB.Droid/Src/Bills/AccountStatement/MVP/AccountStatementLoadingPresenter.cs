using System;
using myTNB.Mobile;
using myTNB.Mobile.AWS.Models.AccountStatement;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.myTNBMenu.Models;

namespace myTNB_Android.Src.Bills.AccountStatement.MVP
{
    public class AccountStatementLoadingPresenter : AccountStatementLoadingContract.IUserActionsListener
    {
        private readonly AccountStatementLoadingContract.IView view;

        private AccountData selectedAccount;
        private string preferredMonths;
        private BaseAppCompatActivity mActivity;

        public AccountStatementLoadingPresenter(AccountStatementLoadingContract.IView view, BaseAppCompatActivity activity)
        {
            this.mActivity = activity;
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

        public async void RequestAccountStatement()
        {
            string accNo = selectedAccount.AccountNum;
            bool isOwner = selectedAccount.IsOwner;

            if (!AccessTokenCache.Instance.HasTokenSaved(this.mActivity))
            {
                string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(UserEntity.GetActive().UserID ?? string.Empty);
                AccessTokenCache.Instance.SaveAccessToken(this.mActivity, accessToken);
            }

            PostAccountStatementResponse accountStatementResponse = await AccountStatementManager.Instance.PostAccountStatement(accNo, preferredMonths, isOwner, AccessTokenCache.Instance.GetAccessToken(this.mActivity));
            if (accountStatementResponse != null &&
                accountStatementResponse.StatusDetail != null &&
                accountStatementResponse.StatusDetail.IsSuccess &&
                accountStatementResponse.Content != null)
            {

            }
        }
    }
}
