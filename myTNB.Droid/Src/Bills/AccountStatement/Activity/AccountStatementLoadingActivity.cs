using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using CheeseBind;
using Com.Airbnb.Lottie;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Bills.AccountStatement.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Bills.AccountStatement.Activity
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class AccountStatementLoadingActivity : BaseToolbarAppCompatActivity, AccountStatementLoadingContract.IView
    {
        [BindView(Resource.Id.txtAcctStmntLoadingTitle)]
        TextView txtAcctStmntLoadingTitle;

        [BindView(Resource.Id.txtAcctStmntLoadingMsg)]
        TextView txtAcctStmntLoadingMsg;

        //stub
        [BindView(Resource.Id.accountStatementButton)]
        Button accountStatementButton;

        [BindView(Resource.Id.timeOutButton)]
        Button timeOutButton;

        [OnClick(Resource.Id.accountStatementButton)]
        public void AcctStmntsOnClick(object sender, EventArgs eventArgs)
        {
            OnShowAccountStamentScreen();
        }

        [OnClick(Resource.Id.timeOutButton)]
        public void TimeOutOnClick(object sender, EventArgs eventArgs)
        {
            OnShowTimeOutScreen();
        }
        //stub

        private AccountStatementLoadingContract.IUserActionsListener presenter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                _ = new AccountStatementLoadingPresenter(this);
                this.presenter?.OnInitialize();

                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                    {
                        this.presenter?.SetSelectedAccount(JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT)));
                    }
                    if (extras.ContainsKey(AccountStatementConstants.SELECTED_MONTH_FOR_ACCOUNT_STATEMENT))
                    {
                        this.presenter?.SetPreferredMonths(extras.GetString(AccountStatementConstants.SELECTED_MONTH_FOR_ACCOUNT_STATEMENT));
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetPresenter(AccountStatementLoadingContract.IUserActionsListener userActionListener)
        {
            this.presenter = userActionListener;
        }

        public void SetUpViews()
        {
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

            SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.TITLE));

            LottieAnimationView loadingAnimation = FindViewById<LottieAnimationView>(Resource.Id.acctStmntLoadingView);

            try
            {
                loadingAnimation.Progress = 0f;
                loadingAnimation.PlayAnimation();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            TextViewUtils.SetMuseoSans500Typeface(txtAcctStmntLoadingTitle);
            TextViewUtils.SetMuseoSans300Typeface(txtAcctStmntLoadingMsg);
            TextViewUtils.SetTextSize16(txtAcctStmntLoadingTitle);
            TextViewUtils.SetTextSize14(txtAcctStmntLoadingMsg);

            txtAcctStmntLoadingTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.PROCESSING_TITLE);
            txtAcctStmntLoadingMsg.Text = Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.PROCESSING_MSG);
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override int ResourceId()
        {
            return Resource.Layout.AccountStatementLoadingView;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        private void OnShowTimeOutScreen()
        {
            Intent acctStmntTimeOutIntent = new Intent(this, typeof(AccountStatementTimeOutActivity));
            acctStmntTimeOutIntent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(this.presenter?.GetSelectedAccount()));
            StartActivity(acctStmntTimeOutIntent);
        }

        private void OnShowAccountStamentScreen()
        {

        }
    }
}
