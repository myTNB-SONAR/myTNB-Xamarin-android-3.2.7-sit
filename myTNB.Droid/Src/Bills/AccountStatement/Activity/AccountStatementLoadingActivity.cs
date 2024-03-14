using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using CheeseBind;
using Com.Airbnb.Lottie;
using myTNB.Android.Src.Base.Activity;
using myTNB.Android.Src.Bills.AccountStatement.MVP;
using myTNB.Android.Src.myTNBMenu.Models;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using Android.Views;
using myTNB;
using myTNB.Mobile;

namespace myTNB.Android.Src.Bills.AccountStatement.Activity
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class AccountStatementLoadingActivity : BaseToolbarAppCompatActivity, AccountStatementLoadingContract.IView
    {
        [BindView(Resource.Id.acctStmntLoadingLayout)]
        readonly LinearLayout acctStmntLoadingLayout;

        [BindView(Resource.Id.acctStmntRefreshLayout)]
        readonly LinearLayout acctStmntRefreshLayout;

        [BindView(Resource.Id.acctStmntRefreshButtonLayout)]
        readonly LinearLayout acctStmntRefreshButtonLayout;

        [BindView(Resource.Id.txtAcctStmntLoadingTitle)]
        TextView txtAcctStmntLoadingTitle;

        [BindView(Resource.Id.txtAcctStmntLoadingMsg)]
        TextView txtAcctStmntLoadingMsg;

        [BindView(Resource.Id.txtAcctStmntRefreshMsg)]
        TextView txtAcctStmntRefreshMsg;

        [BindView(Resource.Id.acctStmntBtnRefresh)]
        Button acctStmntBtnRefresh;

        private AccountStatementLoadingContract.IUserActionsListener presenter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                _ = new AccountStatementLoadingPresenter(this, this, this);

                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                    {
                        this.presenter?.SetSelectedAccount(JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT)));
                    }
                    if (extras.ContainsKey(AccountStatementConstants.STATEMENT_PERIOD))
                    {
                        this.presenter?.SetPreferredMonths(extras.GetString(AccountStatementConstants.STATEMENT_PERIOD));
                    }
                }

                this.presenter?.OnInitialize();
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
            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);

            TextViewUtils.SetMuseoSans500Typeface(txtAcctStmntLoadingTitle, acctStmntBtnRefresh);
            TextViewUtils.SetMuseoSans300Typeface(txtAcctStmntLoadingMsg, txtAcctStmntRefreshMsg);
            TextViewUtils.SetTextSize16(txtAcctStmntLoadingTitle, txtAcctStmntRefreshMsg, acctStmntBtnRefresh);
            TextViewUtils.SetTextSize14(txtAcctStmntLoadingMsg);

            txtAcctStmntLoadingTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.PROCESSING_TITLE);
            txtAcctStmntLoadingMsg.Text = Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.PROCESSING_MSG);
            txtAcctStmntRefreshMsg.Text = Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.REFRESH_MSG);
            acctStmntBtnRefresh.Text = Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.REFRESH_NOW);

            RunOnUiThread(() =>
            {
                try
                {
                    DynatraceHelper.OnTrack(DynatraceConstants.BR.Screens.LoadingAccountStatement.Loading);
                    LottieAnimationView loadingAnimation = FindViewById<LottieAnimationView>(Resource.Id.acctStmntLoadingView);
                    loadingAnimation.Progress = 0f;
                    loadingAnimation.PlayAnimation();

                    this.presenter?.RequestAccountStatement();
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            });
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                if (requestCode == Constants.ACCTSTMNT_PDFVIEW_REQUEST_CODE)
                {
                    if (resultCode == Result.Canceled)
                    {
                        Finish();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

        public override bool ShowBackArrowIndicator()
        {
            return false;
        }

        public override void OnBackPressed()
        {
            DynatraceHelper.OnTrack(DynatraceConstants.BR.CTAs.Error.Refresh_Back_To_Bills);
            base.OnBackPressed();
        }

        private void ShowBackButton(bool flag)
        {
            this.SupportActionBar.SetDisplayHomeAsUpEnabled(flag);
            this.SupportActionBar.SetDisplayShowHomeEnabled(flag);
        }

        [OnClick(Resource.Id.acctStmntBtnRefresh)]
        public void ButtonRefreshOnClick(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                DynatraceHelper.OnTrack(DynatraceConstants.BR.CTAs.Error.Refresh_Now);
                this.SetIsClicked(true);
                ShowLoadingView();
                this.presenter?.RequestAccountStatement();
            }
        }

        public void ShowRefreshView()
        {
            RunOnUiThread(() =>
            {
                ShowBackButton(true);
                acctStmntLoadingLayout.Visibility = ViewStates.Gone;
                acctStmntRefreshLayout.Visibility = ViewStates.Visible;
                acctStmntRefreshButtonLayout.Visibility = ViewStates.Visible;
            });
        }

        public void ShowLoadingView()
        {
            RunOnUiThread(() =>
            {
                DynatraceHelper.OnTrack(DynatraceConstants.BR.Screens.LoadingAccountStatement.Loading);
                ShowBackButton(false);
                acctStmntLoadingLayout.Visibility = ViewStates.Visible;
                acctStmntRefreshLayout.Visibility = ViewStates.Gone;
                acctStmntRefreshButtonLayout.Visibility = ViewStates.Gone;
                this.SetIsClicked(false);
            });
        }

        public void OnShowTimeOutScreen()
        {
            Intent acctStmntTimeOutIntent = new Intent(this, typeof(AccountStatementTimeOutActivity));
            acctStmntTimeOutIntent.PutExtra(Constants.ACCT_STMNT_EMPTY, false);
            acctStmntTimeOutIntent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(this.presenter?.GetSelectedAccount()));
            StartActivity(acctStmntTimeOutIntent);
        }

        public void OnShowAccountStamentScreen(string pdfFilePath)
        {
            var pdfViewActivity = new Intent(this, typeof(AccountStatementPDFActivity));
            pdfViewActivity.PutExtra(Constants.ACCT_STMNT_PDF_FILE_PATH, pdfFilePath);
            StartActivityForResult(pdfViewActivity, Constants.ACCTSTMNT_PDFVIEW_REQUEST_CODE);
        }
    }
}
