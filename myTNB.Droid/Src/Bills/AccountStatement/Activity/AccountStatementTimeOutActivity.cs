using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

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

        private AccountData selectedAccount;

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

            TextViewUtils.SetMuseoSans500Typeface(txtAcctStmntTimeOutTitle, acctStmntTimeOutBtnBack);
            TextViewUtils.SetMuseoSans300Typeface(txtAcctStmntTimeOutMsg);
            TextViewUtils.SetTextSize16(txtAcctStmntTimeOutTitle, acctStmntTimeOutBtnBack);
            TextViewUtils.SetTextSize14(txtAcctStmntTimeOutMsg);

            txtAcctStmntTimeOutTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.TIMEOUT_TITLE);
            txtAcctStmntTimeOutMsg.Text = Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.TIMEOUT_MSG);
            acctStmntTimeOutBtnBack.Text = Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.BACK_TO_BILLS);
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
