using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Widget;
using Android.Views;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using myTNB.Mobile;

namespace myTNB_Android.Src.Bills.AccountStatement.Activity
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class AccountStatementTimeOutActivity : BaseToolbarAppCompatActivity
    {
        [BindView(Resource.Id.txtAcctStmntTimeOutTitle)]
        TextView txtAcctStmntTimeOutTitle;

        [BindView(Resource.Id.txtAcctStmntTimeOutMsg)]
        TextView txtAcctStmntTimeOutMsg;

        [BindView(Resource.Id.acctStmntTimeOutBtnBack)]
        Button acctStmntTimeOutBtnBack;

        [BindView(Resource.Id.acctStmntTimeOutLayout)]
        readonly LinearLayout acctStmntTimeOutLayout;

        [BindView(Resource.Id.acctStmntEmptyLayout)]
        readonly LinearLayout acctStmntEmptyLayout;

        [BindView(Resource.Id.txtAcctStmntEmptyMsg)]
        TextView txtAcctStmntEmptyMsg;

        private AccountData selectedAccount;
        private bool isEmpty;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                    {
                        selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                    }
                    if (extras.ContainsKey(Constants.ACCT_STMNT_EMPTY))
                    {
                        isEmpty = extras.GetBoolean(Constants.ACCT_STMNT_EMPTY);
                    }
                }
                SetUpViews();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void SetUpViews()
        {
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

            SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.TITLE));
            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);

            TextViewUtils.SetMuseoSans500Typeface(acctStmntTimeOutBtnBack);
            TextViewUtils.SetTextSize16(acctStmntTimeOutBtnBack);
            acctStmntTimeOutBtnBack.Text = Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.BACK_TO_BILLS);

            if (isEmpty)
            {
                SetUpEmptyView();
            }
            else
            {
                SetUpTimeOutView();
            }
        }

        private void SetUpTimeOutView()
        {
            acctStmntTimeOutLayout.Visibility = ViewStates.Visible;
            acctStmntEmptyLayout.Visibility = ViewStates.Gone;

            TextViewUtils.SetMuseoSans500Typeface(txtAcctStmntTimeOutTitle);
            TextViewUtils.SetMuseoSans300Typeface(txtAcctStmntTimeOutMsg);
            TextViewUtils.SetTextSize16(txtAcctStmntTimeOutTitle);
            TextViewUtils.SetTextSize14(txtAcctStmntTimeOutMsg);

            txtAcctStmntTimeOutTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.TIMEOUT_TITLE);

            try
            {
                string email = UserEntity.GetActive().Email;
                string message = string.Format(Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.TIMEOUT_MSG), email);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtAcctStmntTimeOutMsg.TextFormatted = Html.FromHtml(message, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtAcctStmntTimeOutMsg.TextFormatted = Html.FromHtml(message);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void SetUpEmptyView()
        {
            acctStmntTimeOutLayout.Visibility = ViewStates.Gone;
            acctStmntEmptyLayout.Visibility = ViewStates.Visible;

            TextViewUtils.SetMuseoSans300Typeface(txtAcctStmntEmptyMsg);
            TextViewUtils.SetTextSize14(txtAcctStmntEmptyMsg);

            try
            {
                string message = Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.NO_TRANSACTION_MSG);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtAcctStmntEmptyMsg.TextFormatted = Html.FromHtml(message, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtAcctStmntEmptyMsg.TextFormatted = Html.FromHtml(message);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override bool ShowBackArrowIndicator()
        {
            return false;
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override int ResourceId()
        {
            return Resource.Layout.AccountStatementTimeOutView;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        [OnClick(Resource.Id.acctStmntTimeOutBtnBack)]
        public void BackToBillsOnClick(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                DynatraceHelper.OnTrack(isEmpty
                    ? DynatraceConstants.BR.CTAs.Error.No_History_Back_To_Bills
                    : DynatraceConstants.BR.CTAs.Error.Timeout_Back_To_Bills);
                this.SetIsClicked(true);
                Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
                DashboardIntent.PutExtra("FROM_ACCOUNT_STATEMENT", true);
                DashboardIntent.PutExtra("MENU", "BillMenu");
                DashboardIntent.PutExtra("DATA", JsonConvert.SerializeObject(selectedAccount));
                DashboardIntent.AddFlags(ActivityFlags.ClearTop);
                StartActivity(DashboardIntent);
            }
        }
    }
}
