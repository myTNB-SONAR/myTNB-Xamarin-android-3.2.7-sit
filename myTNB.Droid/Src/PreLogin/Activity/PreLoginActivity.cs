﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Feedback_PreLogin_Menu.Activity;
using myTNB_Android.Src.FindUs.Activity;
using myTNB_Android.Src.Login.Activity;
using myTNB_Android.Src.PreLogin.MVP;
using myTNB_Android.Src.RegistrationForm.Activity;
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB_Android.Src.PreLogin.Activity
{
    [Activity(Label = "@string/app_name"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.PreLogin")]
    public class PreLoginActivity : BaseAppCompatActivity, PreLoginContract.IView
    {

        private PreLoginPresenter mPresenter;
        private PreLoginContract.IUserActionsListener userActionsListener;

        [BindView(Resource.Id.txtWelcome)]
        TextView txtWelcome;

        [BindView(Resource.Id.txtManageAccount)]
        TextView txtManageAccount;

        [BindView(Resource.Id.btnRegister)]
        Button btnRegister;

        [BindView(Resource.Id.btnLogin)]
        Button btnLogin;

        [BindView(Resource.Id.txtLikeToday)]
        TextView txtLikeToday;

        [BindView(Resource.Id.txtPromotion)]
        TextView txtPromotion;

        [BindView(Resource.Id.cardview_find_us)]
        CardView cardFindUs;

        [BindView(Resource.Id.img_find_us)]
        ImageView imgFindUs;

        [BindView(Resource.Id.txtFindUs)]
        TextView txtFindUs;

        [BindView(Resource.Id.txtChangeLanguage)]
        TextView txtChangeLanguage;

        [BindView(Resource.Id.cardview_call_us)]
        CardView cardCallUs;

        [BindView(Resource.Id.img_call_us)]
        ImageView imgCallUs;

        [BindView(Resource.Id.txtCallUs)]
        TextView txtCallUs;

        [BindView(Resource.Id.cardview_feedback)]
        CardView cardFeedback;

        [BindView(Resource.Id.img_feedback)]
        ImageView imgFeedback;

        [BindView(Resource.Id.txtFeedback)]
        TextView txtFeedback;

        [BindView(Resource.Id.progressBar)]
        ProgressBar progressBar;

        [BindView(Resource.Id.imgPromotion)]
        ImageView imgPromotion;

        [BindView(Resource.Id.img_logo)]
        ImageView img_logo;

        [BindView(Resource.Id.img_display)]
        ImageView img_display;

        private void UpdateLabels()
        {
            txtWelcome.Text = Utility.GetLocalizedLabel("Prelogin", "welcomeTitle");
            txtManageAccount.Text = Utility.GetLocalizedLabel("Prelogin", "tagline");
            btnRegister.Text = Utility.GetLocalizedLabel("Prelogin", "register");
            btnLogin.Text = Utility.GetLocalizedLabel("Prelogin", "login");
            txtLikeToday.Text = Utility.GetLocalizedLabel("Prelogin", "quickAccess");
            txtFindUs.Text = Utility.GetLocalizedLabel("Prelogin", "findUs");
            txtCallUs.Text = Utility.GetLocalizedLabel("Prelogin", "callUs");
            txtChangeLanguage.Text = Utility.GetLocalizedLabel("Prelogin", "changeLanguage");

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
            {
                txtFeedback.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("DashboardHome", "submitFeedback"), FromHtmlOptions.ModeLegacy);
            }
            else
            {
                txtFeedback.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("DashboardHome", "submitFeedback"));
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                mPresenter = new PreLoginPresenter(this);
                TextViewUtils.SetMuseoSans500Typeface(txtWelcome, txtLikeToday, txtFindUs, txtFeedback, txtCallUs, txtChangeLanguage);
                TextViewUtils.SetMuseoSans300Typeface(txtManageAccount, txtPromotion);
                TextViewUtils.SetMuseoSans500Typeface(btnLogin, btnRegister);
                UpdateLabels();

                GenerateTopLayoutLayout();
                GenerateFindUsCardLayout();
                GenerateCallUsCardLayout();
                GenerateFeedbackCardLayout();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void SetStatusBarBackground(int resId)
        {
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Lollipop)
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
            return Resource.Layout.PreLoginView;
        }

        public void SetPresenter(PreLoginContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowLogin()
        {
            // TODO : ADD START ACTIVITY LOGIN ACTIVITY
            StartActivity(typeof(LoginActivity));
        }

        public void ShowRegister()
        {
            // TODO : ADD START ACTIVITY REGISTER ACTIVITY
            StartActivity(typeof(RegistrationFormActivity));
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Pre Login");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
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

        [OnClick(Resource.Id.btnLogin)]
        void OnLogin(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    this.userActionsListener.NavigateToLogin();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [OnClick(Resource.Id.btnRegister)]
        void OnRegister(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    this.userActionsListener.NavigateToRegister();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [OnClick(Resource.Id.cardview_find_us)]
        void OnFindUs(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    this.userActionsListener.NavigateToFindUs();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [OnClick(Resource.Id.cardview_call_us)]
        void OnCallUs(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    this.userActionsListener.NavigateToCallUs();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }



        [OnClick(Resource.Id.cardview_feedback)]
        void OnFeedback(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    this.userActionsListener.NavigateToFeedback();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [OnClick(Resource.Id.txtChangeLanguage)]
        void OnChangeLanguage(object sender, EventArgs eventArgs)
        {
            string selectedLanguage = LanguageUtil.GetAppLanguage();
            string tooltipLanguage;
            if (selectedLanguage == "MS")
            {
                tooltipLanguage = "EN";
            }
            else
            {
                tooltipLanguage = "MS";
            }
            Utility.ShowChangeLanguageDialog(this, tooltipLanguage, ()=>
            {
                LanguageUtil.SaveAppLanguage(tooltipLanguage);
                UpdateLabels();
            });
        }

        public void ShowPreLoginPromotion(bool success)
        {
            if (success)
            {
                try
                {
                    RunOnUiThread(() =>
                    {
                        progressBar.Visibility = ViewStates.Gone;
                        PreLoginPromoEntity wtManager = new PreLoginPromoEntity();
                        List<PreLoginPromoEntity> items = wtManager.GetAllItems();
                        if (items != null)
                        {
                            if (items.Count == 0)
                            {
                                GetDataFromSiteCore();
                            }
                            else
                            {
                                foreach (PreLoginPromoModel obj in items)
                                {
                                    var imageBitmap = ImageUtils.GetImageBitmapFromUrl(obj.Image);
                                    imgPromotion.SetImageBitmap(imageBitmap);
                                    imgPromotion.Click += delegate
                                    {
                                        //Intent webIntent = new Intent(this, typeof(PromotionWebActivity));
                                        //webIntent.PutExtra(Constants.PROMOTIONS_LINK, obj.GeneralLinkUrl);
                                        //StartActivity(webIntent);
                                    };
                                }
                            }
                        }
                        else
                        {
                            imgPromotion.SetBackgroundResource(Resource.Drawable.promotion);
                        }
                    });
                }
                catch (Exception e)
                {
                    Log.Error("API Exception", e.StackTrace);
                    Utility.LoggingNonFatalError(e);
                }
            }

        }

        public void ShowFindUS()
        {
            StartActivity(new Intent(this, typeof(MapActivity)));
        }

        public void ShowCallUs(WeblinkEntity entity)
        {
            if (entity.OpenWith.Equals("PHONE"))
            {
                var uri = Android.Net.Uri.Parse("tel:" + entity.Url);
                var intent = new Intent(Intent.ActionDial, uri);
                StartActivity(intent);
            }
        }

        public void ShowFeedback()
        {
            var feedbackIntent = new Intent(this, typeof(FeedbackPreLoginMenuActivity));
            StartActivity(feedbackIntent);
        }

        public void GetDataFromSiteCore()
        {
            try
            {
                progressBar.Visibility = ViewStates.Visible;
                this.userActionsListener.OnGetPreLoginPromo();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
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

        private void GenerateTopLayoutLayout()
        {
            try
            {
                LinearLayout.LayoutParams currentLogoImg = img_logo.LayoutParameters as LinearLayout.LayoutParams;

                int imgWidth = GetDeviceHorizontalScaleInPixel(0.125f);

                currentLogoImg.Height = imgWidth;
                currentLogoImg.Width = imgWidth;

                LinearLayout.LayoutParams currentDisplayLogoImg = img_display.LayoutParameters as LinearLayout.LayoutParams;

                int imgDisplayWidth = GetDeviceHorizontalScaleInPixel(0.634f);

                float heightRatio = 132f / 203f;
                int imgDisplayHeight = (int)(imgDisplayWidth * (heightRatio));

                currentDisplayLogoImg.Height = imgDisplayHeight;
                currentDisplayLogoImg.Width = imgDisplayWidth;

            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void GenerateFindUsCardLayout()
        {
            try
            {
                LinearLayout.LayoutParams currentCard = cardFindUs.LayoutParameters as LinearLayout.LayoutParams;
                ViewGroup.LayoutParams currentImg = imgFindUs.LayoutParameters;

                int cardWidth = (this.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(32f) - GetDeviceHorizontalScaleInPixel(0.076f)) / 3;
                float heightRatio = 84f / 88f;
                int cardHeight = (int)(cardWidth * (heightRatio));

                currentCard.Height = cardHeight;
                currentCard.Width = cardWidth;
                currentCard.TopMargin = (int)DPUtils.ConvertDPToPx(12f);
                currentCard.LeftMargin = (int)DPUtils.ConvertDPToPx(16f);
                currentCard.RightMargin = GetDeviceHorizontalScaleInPixel(0.038f);
                currentCard.BottomMargin = (int)DPUtils.ConvertDPToPx(8f);

                float imgHeightRatio = 28f / 88f;
                int imgHeight = (int)(cardWidth * (imgHeightRatio));

                currentImg.Height = imgHeight;
                currentImg.Width = imgHeight;
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void GenerateCallUsCardLayout()
        {
            try
            {
                LinearLayout.LayoutParams currentCard = cardCallUs.LayoutParameters as LinearLayout.LayoutParams;
                ViewGroup.LayoutParams currentImg = imgCallUs.LayoutParameters;

                int cardWidth = (this.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(32f) - GetDeviceHorizontalScaleInPixel(0.076f)) / 3;
                float heightRatio = 84f / 88f;
                int cardHeight = (int)(cardWidth * (heightRatio));

                currentCard.Height = cardHeight;
                currentCard.Width = cardWidth;
                currentCard.TopMargin = (int)DPUtils.ConvertDPToPx(12f);
                currentCard.RightMargin = GetDeviceHorizontalScaleInPixel(0.038f);
                currentCard.BottomMargin = (int)DPUtils.ConvertDPToPx(8f);

                float imgHeightRatio = 28f / 88f;
                int imgHeight = (int)(cardWidth * (imgHeightRatio));

                currentImg.Height = imgHeight;
                currentImg.Width = imgHeight;
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void GenerateFeedbackCardLayout()
        {
            try
            {
                LinearLayout.LayoutParams currentCard = cardFeedback.LayoutParameters as LinearLayout.LayoutParams;
                ViewGroup.LayoutParams currentImg = imgFeedback.LayoutParameters;

                int cardWidth = (this.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(32f) - GetDeviceHorizontalScaleInPixel(0.076f)) / 3;
                float heightRatio = 84f / 88f;
                int cardHeight = (int)(cardWidth * (heightRatio));

                currentCard.Height = cardHeight;
                currentCard.Width = cardWidth;
                currentCard.TopMargin = (int)DPUtils.ConvertDPToPx(12f);
                currentCard.RightMargin = (int)DPUtils.ConvertDPToPx(16f);
                currentCard.BottomMargin = (int)DPUtils.ConvertDPToPx(8f);

                float imgHeightRatio = 28f / 88f;
                int imgHeight = (int)(cardWidth * (imgHeightRatio));

                currentImg.Height = imgHeight;
                currentImg.Width = imgHeight;
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

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
    }
}
