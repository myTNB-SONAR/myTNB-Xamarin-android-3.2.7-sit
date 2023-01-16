
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Net.Http;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using CheeseBind;
using myTNB;
using myTNB.Mobile;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.DigitalBill.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.MyHome.Activity
{
    [Activity(Label = "MyHomeMicrositeActivity"
      , ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.Dashboard")]
    public class MyHomeMicrositeActivity : BaseToolbarAppCompatActivity
    {

        [BindView(Resource.Id.micrositeWebview)]
        WebView micrositeWebview;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

            SetToolBarTitle(Utility.GetLocalizedLabel("ConnectMyPremise", "title"));

            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

            HideTopNavBar();
            SetUpWebView();
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override int ResourceId()
        {
            return Resource.Layout.MyHomeMicrositeView;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        internal void HideTopNavBar()
        {
            if (this.SupportActionBar != null)
            {
                this.SupportActionBar.Hide();
            }
        }

        internal void OnShowDashboard()
        {
            Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
            MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
            HomeMenuUtils.ResetAll();
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);
        }

        private void SetUpWebView()
        {
            try
            {
                //STUB
                string originURL = "mytnbapp://action=backToApp";
                string redirectURL = "https://52.76.106.232/Application/Offerings";

                UserEntity user = UserEntity.GetActive();
                string myTNBAccountName = user?.DisplayName ?? string.Empty;
                string signature = SSOManager.Instance.GetMyHomeSignature(myTNBAccountName
                , AccessTokenCache.Instance.GetAccessToken(this)
                , user.DeviceId ?? string.Empty
                , DeviceIdUtils.GetAppVersionName().Replace("v", string.Empty)
                , 16
                , (LanguageUtil.GetAppLanguage() == "MS"
                ? LanguageManager.Language.MS
                : LanguageManager.Language.EN).ToString()
                , TextViewUtils.FontInfo ?? "N"
                , originURL
                , redirectURL
                , user.UserID
                , myTNB.Mobile.MobileConstants.OSType.int_Android
                , user.Email);
                
                string ssoURL = string.Format(AWSConstants.Domains.SSO.MyHome, signature);

                micrositeWebview.SetWebChromeClient(new WebChromeClient());
                micrositeWebview.SetWebViewClient(new MyHomeWebViewClient(this));
                micrositeWebview.Settings.JavaScriptEnabled = true;
                micrositeWebview.Settings.AllowFileAccess = true;
                micrositeWebview.Settings.AllowFileAccessFromFileURLs = true;
                micrositeWebview.Settings.AllowUniversalAccessFromFileURLs = true;
                micrositeWebview.Settings.AllowContentAccess = true;
                micrositeWebview.Settings.JavaScriptCanOpenWindowsAutomatically = true;
                micrositeWebview.Settings.DomStorageEnabled = true;
                micrositeWebview.Settings.MediaPlaybackRequiresUserGesture = false;

                micrositeWebview.LoadUrl(ssoURL);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public class MyHomeWebViewClient : WebViewClient
        {
            private MyHomeMicrositeActivity mActivity;

            public MyHomeWebViewClient(MyHomeMicrositeActivity mActivity)
            {
                this.mActivity = mActivity;
            }

            public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
            {
                bool shouldOverride = false;

                if (ConnectionUtils.HasInternetConnection(mActivity) && request != null)
                {
                    string url = request.Url.ToString();
                    Log.Debug("[DEBUG]", "MyHomeWebViewClient url: " + url);
                    if (url.Contains("mytnbapp://action=backToApp"))
                    {
                        mActivity.OnBackPressed();
                        shouldOverride = true;
                    }
                    else if(url.Contains("mytnbapp://action=backToHome"))
                    {
                        mActivity.OnShowDashboard();
                        shouldOverride = true;
                    }
                }

                return shouldOverride;
            }

            public override void OnPageStarted(WebView view, string url, Bitmap favicon) { }

            public override void OnPageFinished(WebView view, string url) { }

            public override void OnReceivedError(WebView view, IWebResourceRequest request, WebResourceError error) { }

            public override void OnReceivedSslError(WebView view, SslErrorHandler handler, SslError error)
            {
                handler.Proceed();
            }
        }
    }
}

