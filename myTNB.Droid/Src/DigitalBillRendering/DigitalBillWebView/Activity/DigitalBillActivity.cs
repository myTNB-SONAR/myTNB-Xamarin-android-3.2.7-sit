using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Net.Http;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Google.Android.Material.Snackbar;
using myTNB;
using myTNB.Mobile;
using myTNB.Mobile.AWS.Models;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.DigitalBill.MVP;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.DigitalBill.Activity
{
    [Activity(Label = "@string/terms_conditions_activity_title"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.TnC")]
    public class DigitalBillActivity : BaseActivityCustom
        , DigitalBillContract.IView
    {
        private static Snackbar mErrorMessageSnackBar;
        private static FrameLayout mainView;
        private WebView micrositeWebView;

        private DigitalBillPresenter mPresenter;
        private DigitalBillContract.IUserActionsListener userActionsListener;
        private GetBillRenderingResponse BillRendering;

        private const string PAGE_ID = "DBRWebview";
        private const string SELECTED_ACCOUNT_KEY = ".selectedAccount";

        private string _accountNumber = string.Empty;
        private bool IsDBR = true;

        internal bool ShouldBackToHome = false;

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.DigitalBillView;
        }

        public void SetPresenter(DigitalBillContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowDigitalBill(bool success)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        SetDefaultData();
                    }
                    catch (System.Exception er)
                    {
                        Utility.LoggingNonFatalError(er);
                    }
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                Bundle extras = Intent.Extras;

                if ((extras != null) && extras.ContainsKey(SELECTED_ACCOUNT_KEY))
                {
                    _accountNumber = extras.GetString(SELECTED_ACCOUNT_KEY);
                }
                if ((extras != null) && extras.ContainsKey("billRenderingResponse"))
                {
                    BillRendering = JsonConvert.DeserializeObject<GetBillRenderingResponse>(extras.GetString("billRenderingResponse"));
                }

                mPresenter = new DigitalBillPresenter(this);

                micrositeWebView = FindViewById<WebView>(Resource.Id.tncWebView);
                micrositeWebView.Settings.JavaScriptEnabled = true;
                micrositeWebView.SetWebViewClient(new MyTNBWebViewClient(this));
                SetToolBarTitle(GetLabelByLanguage(BillRendering.Content.DBRType == MobileEnums.DBRTypeEnum.Paper
                    ? "goPaperless"
                    : "updateBillDelivery"));
                OnTag();
                ShowDigitalBill(true);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void OnTag(bool isWebViewClient = false, bool isFail = false)
        {
            if (isWebViewClient)
            {
                if (isFail)
                {
                    DynatraceHelper.OnTrack(BillRendering.Content.DBRType == MobileEnums.DBRTypeEnum.Paper
                        ? DynatraceConstants.DBR.CTAs.Webview.Start_Paperless_Confirm
                        : DynatraceConstants.DBR.CTAs.Webview.Back_To_Paper_Confirm);
                    DynatraceHelper.OnTrack(DynatraceConstants.DBR.Screens.Webview.Fail);
                }
                else
                {
                    if (BillRendering.Content.DBRType == MobileEnums.DBRTypeEnum.Paper)
                    {
                        DynatraceHelper.OnTrack(DynatraceConstants.DBR.CTAs.Webview.Start_Paperless_Confirm);
                        DynatraceHelper.OnTrack(DynatraceConstants.DBR.Screens.Webview.Start_Paperless_Success);
                    }
                    else
                    {
                        DynatraceHelper.OnTrack(DynatraceConstants.DBR.CTAs.Webview.Back_To_Paper_Confirm);
                        DynatraceHelper.OnTrack(DynatraceConstants.DBR.Screens.Webview.Back_To_Paper_Success);
                    }
                }
            }
            else
            {
                DynatraceHelper.OnTrack(BillRendering.Content.DBRType == MobileEnums.DBRTypeEnum.Paper
                    ? DynatraceConstants.DBR.Screens.Webview.Start_Paperless
                    : DynatraceConstants.DBR.Screens.Webview.Back_To_Paper);
            }
        }

        private void OnTagRatingDynatrace(bool isSuccess)
        {
            if (isSuccess)
            {
                DynatraceHelper.OnTrack(DynatraceConstants.DBR.CTAs.Webview.Submit_Rating);
            }
            else
            {
                DynatraceHelper.OnTrack(DynatraceConstants.DBR.CTAs.Webview.Start_Paperless_Share_Feedback);
                DynatraceHelper.OnTrack(DynatraceConstants.DBR.Screens.Webview.Start_Paperless_Share_Feedback);
            }
        }

        private void SetShareFeedbackTitle()
        {
            SetToolBarTitle(GetLabelByLanguage("shareFeedbackTitle"));
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.DBRWebViewMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        /// <summary>
        /// X Button Action
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            MyTNBAppToolTipBuilder exitTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER_TWO_BUTTON)
                .SetHeaderImage(Resource.Drawable.ic_display_validation_success)
                .SetTitle(Utility.GetLocalizedLabel("DBRWebview", "confirmPopupTitle"))
                .SetMessage(Utility.GetLocalizedLabel("DBRWebview", "confirmPopupMessage"))
                .SetCTALabel(Utility.GetLocalizedLabel("DBRWebview", "nevermind"))
                .SetSecondaryCTALabel(Utility.GetLocalizedLabel("DBRWebview", "confirm"))
                .SetSecondaryCTAaction(() =>
                {
                    OnTagCloseDynatrace();
                    Log.Debug("[DEBUG]", "ShouldBackToHome: " + ShouldBackToHome);
                    if (ShouldBackToHome)
                    {
                        OnShowDashboard();
                    }
                    else
                    {
                        OnBackPressed();
                    }
                })
                .IsIconImage(true)
                .SetContentGravity(GravityFlags.Center)
                .Build();
            exitTooltip.Show();
            return true;
        }

        private void OnTagCloseDynatrace()
        {
            if (IsDBR)
            {
                DynatraceHelper.OnTrack(BillRendering.Content.DBRType == MobileEnums.DBRTypeEnum.Paper
                    ? DynatraceConstants.DBR.CTAs.Webview.Start_Paperless_Close
                    : DynatraceConstants.DBR.CTAs.Webview.Back_To_Paper_Close);
            }
            else
            {
                DynatraceHelper.OnTrack(DynatraceConstants.DBR.CTAs.Webview.Submit_Rating_Close);
            }
        }

        public override bool ShowBackArrowIndicator()
        {
            return false;
        }

        public void SetDefaultData()
        {
            try
            {
                UserEntity user = UserEntity.GetActive();
                string myTNBAccountName = user?.DisplayName ?? string.Empty;
                string signature = SSOManager.Instance.GetSignature(myTNBAccountName
                    , AccessTokenCache.Instance.GetAccessToken(this)
                    , user.DeviceId ?? string.Empty
                    , DeviceIdUtils.GetAppVersionName().Replace("v", string.Empty)
                    , 16
                    , (LanguageUtil.GetAppLanguage() == "MS"
                        ? LanguageManager.Language.MS
                        : LanguageManager.Language.EN).ToString()
                    , TextViewUtils.FontInfo ?? "N"
                    , BillRendering.Content.OriginURL
                    , BillRendering.Content.RedirectURL
                    , _accountNumber);

                string ssoURL = string.Format(AWSConstants.Domains.SSO, signature);
                micrositeWebView.LoadUrl(ssoURL);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressBar()
        {
            try
            {

            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Terms And Conditions");
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        //  TODO: AndroidX Temporary Fix for Android 5,5.1 
        //  TODO: AndroidX Due to this: https://github.com/xamarin/AndroidX/issues/131
        public override AssetManager Assets =>
            (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop && Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.M)
            ? Resources.Assets : base.Assets;

        internal void OnShowDashboard()
        {
            Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
            MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
            HomeMenuUtils.ResetAll();
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);
        }

        public void ShowProgressDialog()
        {
            try
            {
                //LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            try
            {
                //LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public class MyTNBWebViewClient : WebViewClient
        {
            private DigitalBillActivity mActivity;
            private ProgressBar progressBar;

            public MyTNBWebViewClient(DigitalBillActivity mActivity)
            {
                this.mActivity = mActivity;
            }

            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                bool shouldOverride = false;
                if (ConnectionUtils.HasInternetConnection(mActivity))
                {
                    Log.Debug("[DEBUG]", "MyTNBWebViewClient url: " + url.ToString());
                    if (url.Contains("mytnbapp://action=backToApp"))
                    {
                        mActivity.OnBackPressed();
                        shouldOverride = true;
                    }
                    else if (url.Contains("mytnbapp://action=backToHome"))
                    {
                        mActivity.OnShowDashboard();
                        shouldOverride = true;
                    }

                    //Update for X button
                    if (url.ToString().Contains("BillDelivery/Success"))
                    {
                        mActivity.ShouldBackToHome = true;
                        mActivity.IsDBR = true;
                    }
                    else if (url.ToString().Contains("Feedback/Rate")
                        || url.ToString().Contains("FeedbackRating/Success"))
                    {
                        mActivity.ShouldBackToHome = true;
                        mActivity.IsDBR = false;
                    }
                    Log.Debug("[DEBUG]", "MyTNBWebViewClient ShouldBackToHome: " + mActivity.ShouldBackToHome);
                }
                return shouldOverride;
            }

            public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
            {
                try
                {
                    base.OnPageStarted(view, url, favicon);
                    Log.Debug("[DEBUG]", "OnPageStarted url: " + url.ToString());
                    //Update for X button
                    if (url.ToString().Contains("BillDelivery/Success"))
                    {
                        mActivity.ShouldBackToHome = true;
                        mActivity.IsDBR = true;
                        mActivity.OnTag(true);
                    }
                    else if (url.ToString().Contains("Home/Error"))
                    {
                        mActivity.ShouldBackToHome = true;
                        mActivity.IsDBR = true;
                        mActivity.OnTag(true, true);
                    }
                    else if (url.ToString().Contains("Feedback/Rate")
                       || url.ToString().Contains("FeedbackRating/Success"))
                    {
                        mActivity.ShouldBackToHome = true;
                        mActivity.IsDBR = false;
                        mActivity.OnTagRatingDynatrace(url.ToString().Contains("FeedbackRating/Success"));
                        mActivity.SetShareFeedbackTitle();
                    }
                    Log.Debug("[DEBUG]", "OnPageStarted ShouldBackToHome: " + mActivity.ShouldBackToHome);
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public override void OnPageFinished(WebView view, string url)
            {
                try
                {
                    Log.Debug("[DEBUG]", "OnPageFinished url: " + url.ToString());
                    //Update for X button
                    if (url.ToString().Contains("BillDelivery/Success"))
                    {
                        mActivity.ShouldBackToHome = true;
                        mActivity.IsDBR = true;
                    }
                    else if (url.ToString().Contains("Feedback/Rate")
                       || url.ToString().Contains("FeedbackRating/Success"))
                    {
                        mActivity.ShouldBackToHome = true;
                        mActivity.IsDBR = false;
                    }
                    Log.Debug("[DEBUG]", "OnPageFinished ShouldBackToHome: " + mActivity.ShouldBackToHome);
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public override bool OnRenderProcessGone(WebView view, RenderProcessGoneDetail detail)
            {
                return true;
            }

            public override void OnReceivedError(WebView view, ClientError errorCode, string description, string failingUrl)
            {
                try
                {
                    string message = "Please check your internet connection.";
                    if (ConnectionUtils.HasInternetConnection(mActivity))
                    {
                        switch (errorCode)
                        {
                            case ClientError.FileNotFound:
                                message = "File Not Found."; break;
                            case ClientError.Authentication:
                                message = "Authetication Error."; break;
                            case ClientError.FailedSslHandshake:
                                message = "SSL Handshake Failed."; break;
                            case ClientError.Unknown:
                                message = "Unkown Error."; break;
                        }
                        ShowErrorMessage(failingUrl);
                    }
                    else
                    {
                        ShowErrorMessage(failingUrl);
                    }
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public override void OnReceivedSslError(WebView view, SslErrorHandler handler, SslError error)
            {
                handler.Proceed();
            }

            public override void OnLoadResource(WebView view, string url)
            {
                if (!ConnectionUtils.HasInternetConnection(mActivity))
                {
                    view.StopLoading();
                }
            }

            public static void ShowErrorMessage(string failingUrl)
            {
                if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
                {
                    mErrorMessageSnackBar.Dismiss();
                }

                mErrorMessageSnackBar = Snackbar.Make(mainView
                    , Utility.GetLocalizedErrorLabel("noDataConnectionMessage")
                    , Snackbar.LengthIndefinite)
                    .SetAction(Utility.GetLocalizedLabel("Common", "tryAgain")
                        , delegate
                    {
                        mErrorMessageSnackBar.Dismiss();
                    });
                View v = mErrorMessageSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mErrorMessageSnackBar.Show();
            }
        }
    }
}