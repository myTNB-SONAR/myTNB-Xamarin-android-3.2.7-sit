using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using CheeseBind;
using myTNB;
using myTNB.Mobile;
using myTNB.Mobile.SessionCache;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.MVP;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Feedback_PreLogin_Menu.Activity;
using myTNB_Android.Src.FindUs.Activity;
using myTNB_Android.Src.Login.Activity;
using myTNB_Android.Src.Maintenance.Activity;
using myTNB_Android.Src.PreLogin.MVP;
using myTNB_Android.Src.RegistrationForm.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Threading.Tasks;

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

        [BindView(Resource.Id.txtPromotion)]
        TextView txtPromotion;

        [BindView(Resource.Id.cardview_find_us)]
        CardView cardFindUs;

        [BindView(Resource.Id.findUsLayout)]
        LinearLayout findUsLayout;

        [BindView(Resource.Id.secondLayout)]
        LinearLayout secondLayout;


        [BindView(Resource.Id.img_find_us)]
        ImageView imgFindUs;

        [BindView(Resource.Id.txtFindUs)]
        TextView txtFindUs;

        [BindView(Resource.Id.txtChangeLanguage)]
        TextView txtChangeLanguage;

        [BindView(Resource.Id.cardview_call_us)]
        CardView cardCallUs;

        [BindView(Resource.Id.callUsLayout)]
        LinearLayout callUsLayout;

        [BindView(Resource.Id.img_call_us)]
        ImageView imgCallUs;

        [BindView(Resource.Id.txtCallUs)]
        TextView txtCallUs;

        [BindView(Resource.Id.cardview_check_us)]
        CardView cardCheckStatus;

        [BindView(Resource.Id.checkUsLayout)]
        LinearLayout checkStatusLayout;

        [BindView(Resource.Id.img_check_us)]
        ImageView imgCheckStatus;

        [BindView(Resource.Id.txtCheckUs)]
        TextView txtCheckStatus;

        [BindView(Resource.Id.cardview_feedbackfirstrow)]
        CardView cardFeedbackFirstRow;

        [BindView(Resource.Id.cardview_feedbacksecondrow)]
        CardView cardFeedbackSecondRow;

        [BindView(Resource.Id.feedbackLayout)]
        LinearLayout feedbackLayout;

        [BindView(Resource.Id.img_feedback)]
        ImageView imgFeedback;

        [BindView(Resource.Id.txtFeedbackFirst)]
        TextView txtFeedbackFirst;

        [BindView(Resource.Id.txtFeedbackSecond)]
        TextView txtFeedbackSecond;

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
        }

        private void OnMaintenanceProceed()
        {
            DismissProgressDialog();
            Intent maintenanceScreen = new Intent(this, typeof(MaintenanceActivity));
            maintenanceScreen.PutExtra(Constants.MAINTENANCE_TITLE_KEY, MyTNBAccountManagement.GetInstance().GetMaintenanceTitle());
            maintenanceScreen.PutExtra(Constants.MAINTENANCE_MESSAGE_KEY, MyTNBAccountManagement.GetInstance().GetMaintenanceContent());
            StartActivity(maintenanceScreen);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                mPresenter = new PreLoginPresenter(this);
                TextViewUtils.SetMuseoSans500Typeface(txtWelcome, txtFindUs, txtFeedbackFirst, txtFeedbackSecond, txtCallUs, txtCheckStatus, txtChangeLanguage);
                TextViewUtils.SetMuseoSans300Typeface(txtManageAccount, txtPromotion);
                TextViewUtils.SetMuseoSans500Typeface(btnLogin, btnRegister);
                txtWelcome.TextSize = TextViewUtils.GetFontSize(16f);
                txtManageAccount.TextSize = TextViewUtils.GetFontSize(12f);
                txtFindUs.TextSize = TextViewUtils.GetFontSize(10f);
                txtCallUs.TextSize = TextViewUtils.GetFontSize(10f);
                txtCheckStatus.TextSize = TextViewUtils.GetFontSize(10f);
                txtFeedbackFirst.TextSize = TextViewUtils.GetFontSize(10f);
                txtFeedbackSecond.TextSize = TextViewUtils.GetFontSize(10f);
                txtChangeLanguage.TextSize = TextViewUtils.GetFontSize(12f);
                txtPromotion.TextSize = TextViewUtils.GetFontSize(14f);
                btnRegister.TextSize = TextViewUtils.GetFontSize(16f);
                UpdateLabels();

                GenerateTopLayoutLayout();
                GenerateFindUsCardLayout();
                GenerateCallUsCardLayout();
                GenerateFeedbackCardLayout();
                GenerateCheckStatusCardLayout();
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
            SearchApplicationTypeCache.Instance.Clear();
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

        [OnClick(Resource.Id.cardview_check_us)]
        void OnCheckStatus(object sender, EventArgs eventArgs)
        {
            try
            {
                this.userActionsListener.NavigateToCheckStatus();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [OnClick(Resource.Id.cardview_feedbackfirstrow)]
        void OnFeedbackfirstrow(object sender, EventArgs eventArgs)
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
        [OnClick(Resource.Id.cardview_feedbacksecondrow)]
        void OnFeedbacksecondrow(object sender, EventArgs eventArgs)
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
            Utility.ShowChangeLanguageDialog(this, selectedLanguage, () =>
            {
                ShowProgressDialog();
                if (tooltipLanguage == "MS")
                {
                    AppInfoManager.Instance.SetLanguage(LanguageManager.Language.MS);
                }
                else
                {
                    AppInfoManager.Instance.SetLanguage(LanguageManager.Language.EN);
                }
                _ = RunUpdateLanguage(tooltipLanguage);
            });
        }

        private Task RunUpdateLanguage(string language)
        {
            return Task.Run(() =>
            {
                SearchApplicationTypeCache.Instance.Clear();
                LanguageUtil.SaveAppLanguage(language);
                MyTNBAccountManagement.GetInstance().UpdateAppMasterData();
                _ = CheckAppMasterDataDone();
            });
        }

        private Task CheckAppMasterDataDone()
        {
            return Task.Delay(Constants.LANGUAGE_MASTER_DATA_CHECK_TIMEOUT).ContinueWith(_ =>
            {
                if (MyTNBAccountManagement.GetInstance().GetIsAppMasterComplete())
                {
                    if (MyTNBAccountManagement.GetInstance().GetIsAppMasterFailed())
                    {
                        MyTNBAccountManagement.GetInstance().UpdateAppMasterData();
                        _ = CheckAppMasterDataDone();
                    }
                    else if (MyTNBAccountManagement.GetInstance().GetIsAppMasterMaintenance())
                    {
                        try
                        {
                            RunOnUiThread(() =>
                            {
                                MyTNBAccountManagement.GetInstance().ClearSitecoreItem();
                                MyTNBAccountManagement.GetInstance().ClearAppCacheItem();
                                SMRPopUpUtils.OnResetSSMRMeterReadingTimestamp();
                                OnMaintenanceProceed();
                            });
                        }
                        catch (Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                    }
                    else
                    {
                        try
                        {
                            RunOnUiThread(() =>
                            {
                                MyTNBAccountManagement.GetInstance().ClearSitecoreItem();
                                MyTNBAccountManagement.GetInstance().ClearAppCacheItem();
                                SMRPopUpUtils.OnResetSSMRMeterReadingTimestamp();
                                UpdateLabels();
                            });
                        }
                        catch (Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                    }
                }
                else
                {
                    _ = CheckAppMasterDataDone();
                }
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
                                        //Intent webIntent = new Intent(this, typeof(BaseWebviewActivity));
                                        //webIntent.PutExtra(Constants.IN_APP_LINK, obj.GeneralLinkUrl);
                                        //webIntent.PutExtra(Constants.IN_APP_TITLE, "Promotions");
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

        public async void ShowCheckStatus()
        {
            SearchApplicationTypeResponse searchApplicationTypeResponse = SearchApplicationTypeCache.Instance.GetData();
            if (searchApplicationTypeResponse == null)
            {
                ShowProgressDialog();
                searchApplicationTypeResponse = await ApplicationStatusManager.Instance.SearchApplicationType("0", UserEntity.GetActive() != null);
                if (searchApplicationTypeResponse != null
                    && searchApplicationTypeResponse.StatusDetail != null
                    && searchApplicationTypeResponse.StatusDetail.IsSuccess)
                {
                    SearchApplicationTypeCache.Instance.SetData(searchApplicationTypeResponse);
                }
                DismissProgressDialog();
            }
            if (searchApplicationTypeResponse != null
                && searchApplicationTypeResponse.StatusDetail != null
                && searchApplicationTypeResponse.StatusDetail.IsSuccess)
            {
                Intent applicationLandingIntent = new Intent(this, typeof(SearchApplicationStatusActivity));
                applicationLandingIntent.PutExtra("searchApplicationType", JsonConvert.SerializeObject(searchApplicationTypeResponse.Content));
                StartActivity(applicationLandingIntent);
            }
            else
            {
                MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                     .SetTitle(searchApplicationTypeResponse.StatusDetail.Title)
                     .SetMessage(searchApplicationTypeResponse.StatusDetail.Message)
                     .SetCTALabel(searchApplicationTypeResponse.StatusDetail.PrimaryCTATitle)
                     .Build();
                errorPopup.Show();
            }
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

                int cardWidth = (this.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(32f)) / 4;
                float heightRatio = 84f / 72f;
                int cardHeight = (int)(cardWidth * (heightRatio));

                currentCard.Height = cardHeight;
                currentCard.Width = TextViewUtils.IsLargeFonts ? cardWidth + 80 : cardWidth;

                float paddingRatio = 10f / 72f;
                int padding = (int)(cardWidth * (paddingRatio));
                findUsLayout.SetPadding(padding, padding, padding, padding);

                float imgHeightRatio = 28f / 72f;
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

                int cardWidth = (this.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(32f)) / 4;
                float heightRatio = 84f / 72f;
                int cardHeight = (int)(cardWidth * (heightRatio));

                currentCard.Height = cardHeight;
                currentCard.Width = TextViewUtils.IsLargeFonts ? cardWidth + 80 : cardWidth;

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
        }

        private void GenerateFeedbackCardLayout()
        {
            try
            {
                if (TextViewUtils.IsLargeFonts)
                {
                    cardFeedbackFirstRow.Visibility = ViewStates.Gone;
                    secondLayout.Visibility = ViewStates.Visible;
                    LinearLayout.LayoutParams currentCard = cardFeedbackSecondRow.LayoutParameters as LinearLayout.LayoutParams;
                    ViewGroup.LayoutParams currentImg = imgFeedback.LayoutParameters;

                    int cardWidth = (this.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(32f)) / 4;
                    float heightRatio = 84f / 72f;
                    int cardHeight = (int)(cardWidth * (heightRatio));

                    currentCard.Height = cardHeight;
                    currentCard.Width = cardWidth + 80;

                    float paddingRatio = 10f / 72f;
                    int padding = (int)(cardWidth * (paddingRatio));
                    feedbackLayout.SetPadding(padding, padding, padding, padding);

                    float imgHeightRatio = 28f / 72f;
                    int imgHeight = (int)(cardWidth * (imgHeightRatio));

                    currentImg.Height = imgHeight;
                    currentImg.Width = imgHeight;
                }
                else
                {
                    cardFeedbackSecondRow.Visibility = ViewStates.Gone;
                    secondLayout.Visibility = ViewStates.Gone;
                    LinearLayout.LayoutParams currentCard = cardFeedbackFirstRow.LayoutParameters as LinearLayout.LayoutParams;
                    ViewGroup.LayoutParams currentImg = imgFeedback.LayoutParameters;

                    int cardWidth = (this.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(32f)) / 4;
                    float heightRatio = 84f / 72f;
                    int cardHeight = (int)(cardWidth * (heightRatio));

                    currentCard.Height = cardHeight;
                    currentCard.Width = cardWidth;

                    float paddingRatio = 10f / 72f;
                    int padding = (int)(cardWidth * (paddingRatio));
                    feedbackLayout.SetPadding(padding, padding, padding, padding);

                    float imgHeightRatio = 28f / 72f;
                    int imgHeight = (int)(cardWidth * (imgHeightRatio));

                    currentImg.Height = imgHeight;
                    currentImg.Width = imgHeight;
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void GenerateCheckStatusCardLayout()
        {
            try
            {
                LinearLayout.LayoutParams currentCard = cardCheckStatus.LayoutParameters as LinearLayout.LayoutParams;
                ViewGroup.LayoutParams currentImg = imgCheckStatus.LayoutParameters;

                int cardWidth = (this.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(30f)) / 4;
                float heightRatio = 84f / 72f;
                int cardHeight = (int)(cardWidth * (heightRatio));

                currentCard.Height = cardHeight;
                currentCard.Width = (TextViewUtils.IsLargeFonts ? cardWidth + 80 : cardWidth) + 5;

                float paddingRatio = 10f / 72f;
                int padding = (int)(cardWidth * (paddingRatio));
                checkStatusLayout.SetPadding(padding, padding, padding, padding);

                float imgHeightRatio = 28f / 72f;
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
