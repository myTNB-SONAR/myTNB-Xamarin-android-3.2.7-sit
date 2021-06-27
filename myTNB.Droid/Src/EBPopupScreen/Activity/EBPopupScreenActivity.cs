using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
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

        /*[BindView(Resource.Id.imgPromotion)]
        ImageView imgPromotion;

        [BindView(Resource.Id.img_logo)]
        ImageView img_logo;

        [BindView(Resource.Id.img_display)]
        ImageView img_display;*/

        /*private void UpdateLabels()
        {
            string textFindUs = Utility.GetLocalizedLabel("Prelogin", "findUs");
            string textCallUs = Utility.GetLocalizedLabel("Prelogin", "callUs");
            string textCheckStatus = Utility.GetLocalizedLabel("Prelogin", "applicationStatus");
            string textSubmitFeedback = Utility.GetLocalizedLabel("DashboardHome", "submitEnquiry");
            // textFindUs = textFindUs.Replace(" ", "<br>");
            // textCallUs = textCallUs.Replace(" ", "<br>");
            textCheckStatus = textCheckStatus.Replace(" ", "<br>");
            textSubmitFeedback = textSubmitFeedback.Replace(" ", "<br>");


            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
            {
                txtFindUs.TextFormatted = Html.FromHtml(textFindUs, FromHtmlOptions.ModeLegacy);
                txtCallUs.TextFormatted = Html.FromHtml(textCallUs, FromHtmlOptions.ModeLegacy);
                txtCheckStatus.TextFormatted = Html.FromHtml(textCheckStatus, FromHtmlOptions.ModeLegacy);
                txtFeedbackFirst.TextFormatted = Html.FromHtml(textSubmitFeedback, FromHtmlOptions.ModeLegacy);
                txtFeedbackSecond.TextFormatted = Html.FromHtml(textSubmitFeedback, FromHtmlOptions.ModeLegacy);
            }
            else
            {
                txtFindUs.TextFormatted = Html.FromHtml(textFindUs);
                txtCallUs.TextFormatted = Html.FromHtml(textCallUs);
                txtCheckStatus.TextFormatted = Html.FromHtml(textCheckStatus);
                txtFeedbackFirst.TextFormatted = Html.FromHtml(textSubmitFeedback);
                txtFeedbackSecond.TextFormatted = Html.FromHtml(textSubmitFeedback);
            }

            txtWelcome.Text = Utility.GetLocalizedLabel("Prelogin", "welcomeTitle");
            txtManageAccount.Text = Utility.GetLocalizedLabel("Prelogin", "tagline");
            btnRegister.Text = Utility.GetLocalizedLabel("Prelogin", "register");
            btnLogin.Text = Utility.GetLocalizedLabel("Prelogin", "login");
            txtChangeLanguage.Text = Utility.GetLocalizedLabel("Prelogin", "changeLanguage");

            DismissProgressDialog();
        }*/

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                mPresenter = new EBPopupScreenPresenter(this);
                TextViewUtils.SetMuseoSans500Typeface(txtTitlePopupEB, txtThreeStep, txtSetbudget, txtMonitor, txtSavingbudget);
                TextViewUtils.SetMuseoSans500Typeface(btnGetStarted, btnMaybeLater);
                txtTitlePopupEB.TextSize = TextViewUtils.GetFontSize(16f);
                txtThreeStep.TextSize = TextViewUtils.GetFontSize(12f);
                txtSetbudget.TextSize = TextViewUtils.GetFontSize(14f);
                txtMonitor.TextSize = TextViewUtils.GetFontSize(14f);
                txtSavingbudget.TextSize = TextViewUtils.GetFontSize(14f);
                btnGetStarted.TextSize = TextViewUtils.GetFontSize(16f);
                btnMaybeLater.TextSize = TextViewUtils.GetFontSize(16f);

                /*txtTitlePopupEB.Text = Utility.GetLocalizedLabel("Prelogin", "welcomeTitle");
                txtThreeStep.Text = Utility.GetLocalizedLabel("Prelogin", "tagline");
                txtSetbudget.Text = Utility.GetLocalizedLabel("Prelogin", "register");
                txtMonitor.Text = Utility.GetLocalizedLabel("Prelogin", "login");
                btnGetStarted.Text = Utility.GetLocalizedLabel("Prelogin", "changeLanguage");
                btnMaybeLater.Text = Utility.GetLocalizedLabel("Prelogin", "changeLanguage");*/

                //txtSetbudget.Text = Html.FromHtml(Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "title"), FromHtmlOptions.ModeLegacy).ToString();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void SetStatusBarBackground(int resId)
        {
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
            {
                Drawable drawable = Resources.GetDrawable(resId);
                this.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                this.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                this.Window.SetBackgroundDrawable(drawable);
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

        /*private void GenerateCallUsCardLayout()
        {
            try
            {
                LinearLayout.LayoutParams currentCard = cardCallUs.LayoutParameters as LinearLayout.LayoutParams;
                ViewGroup.LayoutParams currentImg = imgCallUs.LayoutParameters;

                int cardWidth = (this.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(32f)) / cardCount;
                float heightRatio = 84f / 72f;
                int cardHeight = (int)(cardWidth * (heightRatio));

                //currentCard.Height = cardHeight;
                currentCard.Width = TextViewUtils.IsLargeFonts ? cardWidth + 85 : cardWidth;

                float paddingRatio = 10f / 72f;
                int padding = (int)(cardWidth * (paddingRatio));
                callUsLayout.SetPadding(padding, padding, padding, padding);

                float imgHeightRatio = 28f / 72f;
                int imgHeight = (int)(cardWidth * (imgHeightRatio));

                currentImg.Height = imgHeight;
                currentImg.Width = imgHeight;
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }*/

                private int GetDeviceHorizontalScaleInPixel(float percentageValue)
        {
            var deviceWidth = Resources.DisplayMetrics.WidthPixels;
            return GetScaleInPixel(deviceWidth, percentageValue);
        }

        private int GetScaleInPixel(int basePixel, float percentageValue)
        {
            int scaledInPixel = (int)((float)basePixel * percentageValue);
            return scaledInPixel;
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
    }
}
