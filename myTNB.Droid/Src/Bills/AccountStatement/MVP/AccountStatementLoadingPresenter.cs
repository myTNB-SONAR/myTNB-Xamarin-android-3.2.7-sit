using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Util;
using myTNB.Mobile;
using myTNB.Mobile.AWS.Models.AccountStatement;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.Bills.AccountStatement.MVP
{
    public class AccountStatementLoadingPresenter : AccountStatementLoadingContract.IUserActionsListener
    {
        private readonly AccountStatementLoadingContract.IView view;

        private AccountData selectedAccount;
        private string preferredMonths;
        private BaseAppCompatActivity mActivity;
        private Context mContext;

        public AccountStatementLoadingPresenter(AccountStatementLoadingContract.IView view, BaseAppCompatActivity activity, Context context)
        {
            this.mActivity = activity;
            this.mContext = context;
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

        public void RequestAccountStatement()
        {
            Task.Run(() =>
            {
                _ = OnPostAccountStatement();
            });
        }

        private async Task OnPostAccountStatement()
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
                if (accountStatementResponse.Content.AccountStatement == null)
                {
                    System.Console.WriteLine("Account Statement is Empty");
                    Log.Debug("[DEBUG]", "Account Statement is Empty");
                    this.view?.OnShowTimeOutScreen(true);
                }
                else
                {
                    try
                    {
                        Log.Debug("[DEBUG]", "Account Statement is Not Empty");
                        string refNo = accountStatementResponse.Content.ReferenceNo;
                        string fileName = string.Format("{0} {1}.pdf", Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.TITLE), this.selectedAccount.AccountNum);
                        byte[] pdfByte = accountStatementResponse.Content.AccountStatement;
                        string filePath = await FileUtils.SaveAsyncPDF(this.mContext, pdfByte, FileUtils.PDF_FOLDER, fileName);
                        this.view?.OnShowAccountStamentScreen(filePath);
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                }
            }
            else if (accountStatementResponse != null &&
                accountStatementResponse.StatusDetail != null &&
                accountStatementResponse.StatusDetail.IsTimeout)
            {
                this.view?.OnShowTimeOutScreen(false);
                System.Console.WriteLine("Account Statement has Timed Out");
                Log.Debug("[DEBUG]", "Account Statement has Timed Out");
            }
            else
            {
                this.view?.ShowRefreshView();
                System.Console.WriteLine("Account Statement is in Refresh View");
                Log.Debug("[DEBUG]", "Account Statement is in Refresh View");
            }
        }
    }
}
