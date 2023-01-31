using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Net.Http;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Webkit;
using CheeseBind;
using myTNB;
using myTNB.Mobile;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using MyHomeModel = myTNB_Android.Src.MyHome.Model.MyHomeModel;

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

        MyHomeModel model;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

                SetStatusBarBackground(Resource.Drawable.Background_Status_Bar);
                SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

                HideTopNavBar();

                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    if (extras.ContainsKey(MyHomeConstants.MYHOME_MODEL))
                    {
                        model = JsonConvert.DeserializeObject<MyHomeModel>(extras.GetString(MyHomeConstants.MYHOME_MODEL));
                        SetUpWebView();
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
                string originURL = model?.OriginURL ?? string.Empty;
                string redirectURL = model?.RedirectURL ?? string.Empty;

                //STUB
                //redirectURL = "https://stagingmyhome.mytnb.com.my/Application/Offerings";
                //redirectURL = "https://52.76.106.232/Application/Offerings";
                //redirectURL = "https://devmyhome.mytnb.com.my/Application/Offerings";

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
                , MobileConstants.OSType.int_Android
                , user.Email
                , string.Empty
                , null
                , string.Empty);

                string ssoURL = string.Format(model?.SSODomain ?? AWSConstants.Domains.SSO.MyHome, signature);

                //STUB
                //string ssoURL = string.Format("https://stagingmyhome.mytnb.com.my/Sso?s={0}", signature);
                //ssoURL = string.Format("https://52.76.106.232/Sso?s={0}", signature);
                //ssoURL = string.Format("https://devmyhome.mytnb.com.my/Sso?s={0}", signature);

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
                micrositeWebview.Settings.SetSupportZoom(false);

                micrositeWebview.LoadUrl(ssoURL);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnBackPressed()
        {
            micrositeWebview = null;
            Finish();
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
                    if (url.Contains(MyHomeConstants.BACK_TO_APP))
                    {
                        mActivity.OnBackPressed();
                        shouldOverride = true;
                    }
                    else if (url.Contains(MyHomeConstants.BACK_TO_HOME))
                    {
                        mActivity.Finish();
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
                //STUB
                //TODO: Remove for final release
                handler.Proceed();
            }
        }
    }
}