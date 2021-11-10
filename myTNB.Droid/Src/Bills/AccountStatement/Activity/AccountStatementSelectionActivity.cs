using Android.App;
using Android.Content;
using Android.OS;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Base.Activity;
using Android.Views;
using Android.Util;
using System;
using CheeseBind;
using Android.Widget;
using AndroidX.Core.Content;
using myTNB_Android.Src.myTNBMenu.Models;
using Newtonsoft.Json;
using myTNB.Mobile;

namespace myTNB_Android.Src.Bills.AccountStatement.Activity
{
    [Activity(Theme = "@style/Theme.Dashboard")]
    public class AccountStatementSelectionActivity : BaseActivityCustom
    {
        [BindView(Resource.Id.imgSixMonthsAction)]
        ImageView imgSixMonthsAction;

        [BindView(Resource.Id.imgTheeMonthsAction)]
        ImageView imgTheeMonthsAction;

        [BindView(Resource.Id.txtThreeMonths)]
        TextView txtThreeMonths;

        [BindView(Resource.Id.txtPageTitleInfo)]
        TextView txtPageTitleInfo;

        [BindView(Resource.Id.txtSixMonth)]
        TextView txtSixMonth;

        [BindView(Resource.Id.txtAcctStmtHeaderLabel)]
        TextView txtAcctStmtHeaderLabel;

        [BindView(Resource.Id.txtAcctStmtFooterLabel)]
        TextView txtAcctStmtFooterLabel;

        [BindView(Resource.Id.btnSubmit)]
        Button btnSubmit;

        bool isSixMonthSelected = false;
        bool isThreeMonthSelected = false;

        AccountData selectedAccount;
        private bool billHistoryIsEmpty;

        const string PAGE_ID = "StatementPeriod";

        public override int ResourceId()
        {
            return Resource.Layout.AccountStatementSelectionView;
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override View OnCreateView(string name, Context context, IAttributeSet attrs)
        {
            return base.OnCreateView(name, context, attrs);
        }

        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Account Statement");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnSubmit)]
        internal void OnSubmitClick(object sender, EventArgs e)
        {
            if (!this.GetIsClicked())
            {
                try
                {
                    DynatraceHelper.OnTrack(DynatraceConstants.BR.CTAs.StatementPeriod.Confirm);
                    this.SetIsClicked(true);
                    OnShowAccountStatementLoading();
                    FirebaseAnalyticsUtils.LogClickEvent(this, "View Bill Button Clicked");
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
            }
        }

        public override void OnBackPressed()
        {
            DynatraceHelper.OnTrack(DynatraceConstants.BR.CTAs.StatementPeriod.Back);
            base.OnBackPressed();
        }

        [OnClick(Resource.Id.sixMonthContainer)]
        internal void OnSixMonthsContainerClick(object sender, EventArgs e)
        {
            imgSixMonthsAction.Visibility = ViewStates.Visible;
            imgTheeMonthsAction.Visibility = ViewStates.Gone;
            isSixMonthSelected = true;
            isThreeMonthSelected = false;
            DynatraceHelper.OnTrack(DynatraceConstants.BR.CTAs.StatementPeriod.Past_6_Months);
            SetCTAEnable();
        }

        [OnClick(Resource.Id.threeMonthsContainer)]
        internal void OnthreeMonthsContainerClick(object sender, EventArgs e)
        {
            imgSixMonthsAction.Visibility = ViewStates.Gone;
            imgTheeMonthsAction.Visibility = ViewStates.Visible;
            isSixMonthSelected = false;
            isThreeMonthSelected = true;
            DynatraceHelper.OnTrack(DynatraceConstants.BR.CTAs.StatementPeriod.Past_3_Months);
            SetCTAEnable();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetUpViews();

                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                    {
                        selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                    }
                    if (extras.ContainsKey(Constants.BILL_HISTORY_IS_EMPTY))
                    {
                        billHistoryIsEmpty = extras.GetBoolean(Constants.BILL_HISTORY_IS_EMPTY);
                    }
                }
                imgTheeMonthsAction.SetMaxHeight(txtThreeMonths.Height);
                imgSixMonthsAction.SetMaxHeight(txtSixMonth.Height);

                SetCTAEnable();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void SetUpViews()
        {
            TextViewUtils.SetMuseoSans500Typeface(txtPageTitleInfo, btnSubmit);
            TextViewUtils.SetMuseoSans300Typeface(txtThreeMonths, txtSixMonth, txtAcctStmtHeaderLabel, txtAcctStmtFooterLabel);
            TextViewUtils.SetTextSize16(txtPageTitleInfo, txtThreeMonths, txtSixMonth, btnSubmit);
            TextViewUtils.SetTextSize14(txtAcctStmtHeaderLabel, txtAcctStmtFooterLabel);

            SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.TITLE));
            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);

            txtPageTitleInfo.Text = Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.REQUEST_TITLE);
            txtThreeMonths.Text = Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.PAST_3_MONTHS);
            txtSixMonth.Text = Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.PAST_6_MONTHS);

            txtAcctStmtFooterLabel.Text = Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.DISCLAIMER);
            txtAcctStmtHeaderLabel.Text = Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.STATEMENT_PERIOD_TITLE);
        }

        private void SetCTAEnable()
        {
            bool isEnabled = isSixMonthSelected || isThreeMonthSelected;
            btnSubmit.Enabled = isEnabled;
            btnSubmit.Background = ContextCompat.GetDrawable(this, isEnabled
                ? Resource.Drawable.green_button_background
                : Resource.Drawable.silver_chalice_button_background);
        }

        private void OnShowAccountStatementLoading()
        {
            this.SetIsClicked(true);
            if (billHistoryIsEmpty)
            {
                Intent acctStmntTimeOutIntent = new Intent(this, typeof(AccountStatementTimeOutActivity));
                acctStmntTimeOutIntent.PutExtra(Constants.ACCT_STMNT_EMPTY, billHistoryIsEmpty);
                acctStmntTimeOutIntent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
                StartActivity(acctStmntTimeOutIntent);
            }
            else
            {
                string selectedMonths = string.Empty;
                Intent acctStmntLoadingIntent = new Intent(this, typeof(AccountStatementLoadingActivity));
                acctStmntLoadingIntent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
                if (isSixMonthSelected)
                {
                    selectedMonths = AccountStatementConstants.PAST_6_MONTHS;
                }
                if (isThreeMonthSelected)
                {
                    selectedMonths = AccountStatementConstants.PAST_3_MONTHS;
                }
                acctStmntLoadingIntent.PutExtra(AccountStatementConstants.STATEMENT_PERIOD, selectedMonths);
                StartActivity(acctStmntLoadingIntent);
            }
        }
    }
}