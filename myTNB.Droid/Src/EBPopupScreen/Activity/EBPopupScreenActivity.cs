using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.EBPopupScreen.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Runtime;

namespace myTNB_Android.Src.EBPopupScreen.Activity
{
    [Activity(Label = "@string/app_name"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.PreLogin")]
    public class EBPopupScreenActivity : BaseAppCompatActivity, EBPopupScreenContract.IView
    {

        private EBPopupScreenPresenter mPresenter;
        private EBPopupScreenContract.IUserActionsListener userActionsListener;

        [BindView(Resource.Id.txtTitlePopupEB)]
        TextView txtTitlePopupEB;

        [BindView(Resource.Id.txtThreeStep)]
        TextView txtThreeStep;

        [BindView(Resource.Id.btnGetStarted)]
        Button btnGetStarted;

        [BindView(Resource.Id.btnMaybeLater)]
        Button btnMaybeLater;

        [BindView(Resource.Id.txtSetbudget)]
        TextView txtSetbudget;

        [BindView(Resource.Id.txtMonitor)]
        TextView txtMonitor;

        [BindView(Resource.Id.txtSavingbudget)]
        TextView txtSavingbudget;

        private string fromLogin = string.Empty;

        private string fromDiscoverMore = string.Empty;

        private bool isLogin = false;

        private bool isDiscoverMore = false;

        internal static readonly int SELECT_SM_POPUP_REQUEST_CODE = 8810;

        internal static readonly int SELECT_SM_POPUP_DISCOVERMORE_REQUEST_CODE = 8811;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey("fromLogin"))
                    {
                        fromLogin = extras.GetString("fromLogin");
                    }
                    else if (extras.ContainsKey("fromDashboard"))
                    {
                        isDiscoverMore = true;
                    }
                }

                mPresenter = new EBPopupScreenPresenter(this);
                TextViewUtils.SetMuseoSans500Typeface(txtTitlePopupEB, txtThreeStep, txtSetbudget, txtMonitor, txtSavingbudget);
                TextViewUtils.SetMuseoSans500Typeface(btnGetStarted, btnMaybeLater);

                TextViewUtils.SetTextSize16(txtTitlePopupEB);
                TextViewUtils.SetTextSize16(btnGetStarted, btnMaybeLater);
                TextViewUtils.SetTextSize14(txtSetbudget, txtMonitor, txtSavingbudget);
                TextViewUtils.SetTextSize12(txtThreeStep);

                txtTitlePopupEB.Text = Utility.GetLocalizedLabel("EnergyBudgetPopup", "title");
                txtThreeStep.Text = Utility.GetLocalizedLabel("EnergyBudgetPopup", "threeEasyStep");
                btnMaybeLater.Text = Utility.GetLocalizedLabel("EnergyBudgetPopup", "txtMaybelater");

                string Set = Utility.GetLocalizedLabel("EnergyBudgetPopup", "firstStep");
                string[] OneWord = Set.Trim().Split(" ");
                OneWord[0] = "<font color='#1C79CA'>" + OneWord[0] + "</font>";
                string combineAgain = string.Join(" ", OneWord);
                txtSetbudget.TextFormatted = GetFormattedText(combineAgain);

                string MONITOR = Utility.GetLocalizedLabel("EnergyBudgetPopup", "secondStep");
                string[] OneWordMonitor = MONITOR.Trim().Split(" ");
                OneWordMonitor[0] = "<font color='#1C79CA'>" + OneWordMonitor[0] + "</font>";
                string combineAgainMonitor = string.Join(" ", OneWordMonitor);
                txtMonitor.TextFormatted = GetFormattedText(combineAgainMonitor);

                string SAVINGS = Utility.GetLocalizedLabel("EnergyBudgetPopup", "thirdStep");
                string[] OneWordSavings = SAVINGS.Trim().Split(" ");
                OneWordSavings[0] = "<font color='#1C79CA'>" + OneWordSavings[0] + "</font>";
                string combineAgainSavings = string.Join(" ", OneWordSavings);
                txtSavingbudget.TextFormatted = GetFormattedText(combineAgainSavings);

                if (isDiscoverMore)
                {
                    btnGetStarted.Text = Utility.GetLocalizedLabel("EnergyBudgetPopup", "txtViewmybudget");
                }
                else
                {
                    btnGetStarted.Text = Utility.GetLocalizedLabel("EnergyBudgetPopup", "txtLetgetstarted");
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [OnClick(Resource.Id.btnGetStarted)]
        internal void OnGetStarted(object sender, EventArgs e)
        {
            try
            {
                UserSessions.DoSmartMeterShown(PreferenceManager.GetDefaultSharedPreferences(this));
                CustomClassAnalytics.SetScreenNameDynaTrace("EB_initiate_Start");
                FirebaseAnalyticsUtils.SetScreenName(this, "EB_initiate_Start");
                Intent result = new Intent();
                result.PutExtra("EBList", "EBList");
                SetResult(Result.Ok, result);
                Finish();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [OnClick(Resource.Id.btnMaybeLater)]
        internal void OnMaybeLater(object sender, EventArgs e)
        {
            try
            {
                CustomClassAnalytics.SetScreenNameDynaTrace("EB_initiate_Later");
                FirebaseAnalyticsUtils.SetScreenName(this, "EB_initiate_Later");
                Intent result = new Intent();
                result.PutExtra("MaybeLater", "MaybeLater");
                SetResult(Result.Ok, result);
                Finish();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.EBPopupView;
        }

        public void SetPresenter(EBPopupScreenContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Energy budget Pop up");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            Intent result = new Intent();
            SetResult(Result.Ok, result);
            Finish();
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
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

        public void DismissProgressDialog()
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

        private ISpanned GetFormattedText(string stringValue)
        {
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                return Html.FromHtml(stringValue, FromHtmlOptions.ModeLegacy);
            }
            else
            {
                return Html.FromHtml(stringValue);
            }
        }
    }
}
