using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Net.Http;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
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
using myTNB_Android.Src.Login.Models;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Deeplink;
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

        MyHomeModel _model;
        string _accessToken;

        private Action<int, Result, Intent> _resultCallbackvalue;

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
                        _model = JsonConvert.DeserializeObject<MyHomeModel>(extras.GetString(MyHomeConstants.MYHOME_MODEL));
                        _accessToken = extras.GetString(MyHomeConstants.ACCESS_TOKEN);
                        SetUpWebView(_accessToken);
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

        private void SetUpWebView(string accessToken)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    string originURL = _model?.OriginURL ?? MyHomeConstants.BACK_TO_APP;
                    string redirectURL = _model?.RedirectURL ?? string.Empty;

                    UserEntity user = UserEntity.GetActive();
                    string myTNBAccountName = user?.DisplayName ?? string.Empty;
                    string signature = SSOManager.Instance.GetMyHomeSignature(myTNBAccountName
                    , accessToken
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
                    , user.IdentificationNo
                    , MobileConstants.OSType.int_Android
                    , user.Email
                    , string.Empty
                    , null
                    , user.MobileNo);

                    string ssoURL = string.Format(_model?.SSODomain ?? AWSConstants.Domains.SSO.MyHome, signature);

                    micrositeWebview.SetWebChromeClient(new MyHomeWebChromeClient(this));
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

                    //STUB
                    //global::Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
                    micrositeWebview.LoadUrl(ssoURL);
                });
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

        public void LoadToExternalBrowser(string url)
        {
            Intent intent = new Intent(Intent.ActionView);
            intent.SetData(Android.Net.Uri.Parse(url));
            StartActivity(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void StartsActivity(Intent intent, int requestCode, Action<int, Result, Intent> resultCallback)
        {
            this._resultCallbackvalue = resultCallback;
            StartActivityForResult(intent, requestCode);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (this._resultCallbackvalue != null)
            {
                this._resultCallbackvalue(requestCode, resultCode, data);
                this._resultCallbackvalue = null;
            }
        }

        internal class MyHomeWebChromeClient : WebChromeClient
        {
            MyHomeMicrositeActivity _micrositeWebViewActivity;

            private static int filechooser = 1;
            private IValueCallback message;

            public MyHomeWebChromeClient(MyHomeMicrositeActivity webViewActivity)
            {
                _micrositeWebViewActivity = webViewActivity;
            }

            public override bool OnShowFileChooser(WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
            {
                this.message = filePathCallback;
                Intent captureIntent = fileChooserParams.CreateIntent();
                Intent contentSelectionIntent = new Intent(Intent.ActionGetContent);
                contentSelectionIntent.AddCategory(Intent.CategoryOpenable);
                contentSelectionIntent.SetType("*/*");
                Intent[] intentArray;
                if (captureIntent != null)
                {
                    intentArray = new Intent[] { captureIntent };
                }
                else
                {
                    intentArray = new Intent[0];
                }

                this._micrositeWebViewActivity.StartsActivity(contentSelectionIntent, filechooser, this.OnActivityResult);

                return true;
            }

            private void OnActivityResult(int requestCode, Result resultCode, Intent data)
            {
                if (data != null)
                {
                    if (requestCode == filechooser)
                    {
                        if (null == this.message)
                        {
                            return;
                        }
                        this.message.OnReceiveValue(WebChromeClient.FileChooserParams.ParseResult((int)resultCode, data));
                        this.message = null;
                    }
                }
                else
                {
                    this.message.OnReceiveValue(null);
                    this.message = null;
                    return;
                }
            }

            public override void OnPermissionRequest(PermissionRequest? request)
            {
                request?.Grant(new String[] { PermissionRequest.ResourceVideoCapture});
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

                string url = request.Url.ToString();
                Log.Debug("[DEBUG]", "MyHomeWebViewClient url: " + url);
                if (url.Contains(MyHomeConstants.BACK_TO_APP))
                {
                    shouldOverride = true;
                    mActivity.OnBackPressed();
                }
                else if (url.Contains(MyHomeConstants.BACK_TO_HOME))
                {
                    shouldOverride = true;
                    mActivity.OnShowDashboard();
                }
                else if (url.Contains(MyHomeConstants.EXTERNAL_BROWSER))
                {
                    shouldOverride = true;

                    string value = string.Empty;
                    string pattern = string.Format(MyHomeConstants.PATTERN, MyHomeConstants.EXTERNAL_BROWSER);
                    Regex regex = new Regex(pattern);
                    Match match = regex.Match(url);
                    if (match.Success)
                    {
                        value = match.Value.Replace(string.Format(MyHomeConstants.REPLACE_KEY, MyHomeConstants.EXTERNAL_BROWSER), string.Empty);
                        if (value.IsValid())
                        {
                            mActivity.LoadToExternalBrowser(value);
                        }
                    }
                }
                else if (url.Contains(MyHomeConstants.BACK_TO_APPLICATION_STATUS_LANDING))
                {
                    shouldOverride = true;
                    DeeplinkUtil.Instance.SetTargetScreen(Deeplink.ScreenEnum.ApplicationListing);
                    mActivity.OnShowDashboard();
                }

                return shouldOverride;
            }

            public override void OnPageStarted(WebView view, string url, Bitmap favicon) { }

            public override void OnPageFinished(WebView view, string url) { }

            public override void OnReceivedError(WebView view, IWebResourceRequest request, WebResourceError error) { }

            public override void OnReceivedSslError(WebView view, SslErrorHandler handler, SslError error)
            {
                //STUB
                //handler.Proceed();
            }
        }
    }
}