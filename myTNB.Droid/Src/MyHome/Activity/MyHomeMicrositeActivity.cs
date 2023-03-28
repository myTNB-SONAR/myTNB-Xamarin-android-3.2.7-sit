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
using myTNB.Mobile.SessionCache;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Bills.AccountStatement.Activity;
using myTNB_Android.Src.Bills.AccountStatement.MVP;
using myTNB_Android.Src.Bills.NewBillRedesign.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.Login.Models;
using myTNB_Android.Src.MyHome.MVP;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Deeplink;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using MyHomeModel = myTNB_Android.Src.MyHome.Model.MyHomeModel;

namespace myTNB_Android.Src.MyHome.Activity
{
    [Activity(Label = "MyHomeMicrositeActivity"
      , ScreenOrientation = ScreenOrientation.Portrait
        , WindowSoftInputMode = SoftInput.AdjustResize
      , Theme = "@style/Theme.Dashboard")]
    public class MyHomeMicrositeActivity : BaseToolbarAppCompatActivity, MyHomeMicrositeContract.IView
    {
        [BindView(Resource.Id.micrositeWebview)]
        WebView micrositeWebview;

        MyHomeModel _model;
        string _accessToken;

        private Action<int, Result, Intent> _resultCallbackvalue;
        private MyHomeMicrositeContract.IUserActionsListener presenter;

        private string _dowloadedFilePath;
        private string _downloadedFileExt;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                _ = new MyHomeMicrositePresenter(this, this, this);

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

                        this.presenter?.OnInitialize();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetPresenter(MyHomeMicrositeContract.IUserActionsListener userActionListener)
        {
            this.presenter = userActionListener;
        }

        public void SetUpViews()
        {
            _dowloadedFilePath = string.Empty;
            _downloadedFileExt = string.Empty;
            SetUpWebView(_accessToken);
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

        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
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
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ViewDownloadedFile(string filePath, string fileExtension, string fileTitle)
        {
            var pdfViewActivity = new Intent(this, typeof(CustomPDFImageViewerActivity));
            pdfViewActivity.PutExtra(Constants.PDF_FILE_TITLE, fileTitle);
            pdfViewActivity.PutExtra(Constants.PDF_IMAGE_VIWER_FILE_PATH, filePath);
            pdfViewActivity.PutExtra(Constants.PDF_IMAGE_VIEWER_EXTENSION, fileExtension);
            StartActivity(pdfViewActivity);
        }

        public void ShareDownloadedFile(string filePath, string fileExtension, string fileTitle)
        {
            _dowloadedFilePath = filePath;
            _downloadedFileExt = fileExtension;
            OnSharePermission();
        }

        private void OnSharePermission()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted && ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
            {
                RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE);
            }
            else
            {
                OnShareFile();
            }
        }

        private void OnShareFile()
        {
            try
            {
                if (_dowloadedFilePath.IsValid())
                {
                    Java.IO.File file = new Java.IO.File(_dowloadedFilePath);
                    Android.Net.Uri fileUri = FileProvider.GetUriForFile(this,
                                                ApplicationContext.PackageName + ".fileprovider", file);

                    Intent shareIntent = new Intent(Intent.ActionSend);
                    shareIntent.SetType("application/" + _downloadedFileExt.ToLower());
                    shareIntent.PutExtra(Intent.ExtraStream, fileUri);
                    StartActivity(Intent.CreateChooser(shareIntent, Utility.GetLocalizedLabel("Profile", "share")));
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void InterceptViewFileWithURL(string url)
        {
            this.presenter.ViewFile(url);
        }

        public void InterceptDownloadFileWithURL(string url)
        {
            this.presenter.DownloadFile(url);
        }

        public void InterceptForSuccessfulRating()
        {
            Intent finishRateIntent = new Intent();
            finishRateIntent.PutExtra(Constants.APPLICATION_STATUS_DETAIL_RELOAD, true);
            finishRateIntent.PutExtra(Constants.APPLICATION_STATUS_DETAIL_RATED_TOAST_MESSAGE, RatingCache.Instance.GetRatingToastMessage());
            RatingCache.Instance.Clear();
            SetResult(Result.Ok, finishRateIntent);
            Finish();
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
                    string cancelURL = _model?.CancelURL ?? string.Empty;

                    //STUB
                    //redirectURL = "https://https://18.139.216.169/Application/Offerings";
                    //redirectURL = "https://stagingmyhome.mytnb.com.my/Application/Offerings";

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
                    , user.MobileNo
                    , cancelURL);

                    string ssoURL = string.Format(_model?.SSODomain ?? AWSConstants.Domains.SSO.MyHome, signature);

                    //STUB
                    //ssoURL = string.Format("https://18.139.216.169/Sso?s={0}", signature);

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
                    Log.Debug("[DEBUG]", "ssoURL: " + ssoURL);
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

            try
            {
                if (requestCode == Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE)
                {
                    if (Utility.IsPermissionHasCount(grantResults))
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            RunOnUiThread(() =>
                            {
                                OnShareFile();
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

            private static readonly int _fileChooser = 1;
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

                this._micrositeWebViewActivity.StartsActivity(contentSelectionIntent, _fileChooser, this.OnActivityResult);

                return true;
            }

            private void OnActivityResult(int requestCode, Result resultCode, Intent data)
            {
                if (data != null)
                {
                    if (requestCode == _fileChooser)
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

        internal class MyHomeWebViewClient : WebViewClient
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

                //STUB
                //string pdf = "https://stagingmyhome.mytnb.com.my/Utility/FileUploadWithoutAuth/GetFileByFileID?fileID=59324d20-2089-427a-ab10-27ca87119620";
                //string image = "https://stagingmyhome.mytnb.com.my/Utility/FileUploadWithoutAuth/GetFileByFileID?fileID=4ac61fbf-1c94-4ac8-a21f-9d0bcc88c50c";
                //var encrypted = SecurityManager.Instance.AES256_Encrypt(AWSConstants.MyHome_SaltKey, AWSConstants.MyHome_Passphrase, pdf);
                //url = "mytnbapp://action=openPDF&extension=pdf&&title=ICCopy_202211.pdf&file=" + encrypted;
                //url = MyHomeConstants.RATE_SUCCESSFUL;
                Log.Debug("[DEBUG]", "MyHomeWebViewClient url: " + url);

                if (url.Contains(MyHomeConstants.RATE_SUCCESSFUL))
                {
                    shouldOverride = true;
                    RatingCache.Instance.Clear();
                    RatingCache.Instance.SetRatingToast(string.Empty);
                    this.mActivity.InterceptForSuccessfulRating();
                }
                if (url.Contains(MyHomeConstants.DOWNLOAD_FILE))
                {
                    shouldOverride = true;
                    this.mActivity.InterceptDownloadFileWithURL(url);
                }
                else if (url.Contains(MyHomeConstants.OPEN_FILE))
                {
                    shouldOverride = true;
                    this.mActivity.InterceptViewFileWithURL(url);
                }
                else if (url.Contains(MyHomeConstants.BACK_TO_APP))
                {
                    shouldOverride = true;
                    mActivity.OnBackPressed();
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
                else if (url.Contains(AWSConstants.BackToHomeCancelURL))
                {
                    shouldOverride = true;
                    DeeplinkUtil.Instance.SetTargetScreen(Deeplink.ScreenEnum.Home);
                    DeeplinkUtil.Instance.SetToastMessage(Utility.GetLocalizedCommonLabel(I18NConstants.Cancelled_Application));
                    mActivity.OnShowDashboard();
                }
                else if (url.Contains(AWSConstants.ApplicationStatusLandingCancelURL))
                {
                    shouldOverride = true;
                    DeeplinkUtil.Instance.SetTargetScreen(Deeplink.ScreenEnum.ApplicationListing);
                    DeeplinkUtil.Instance.SetToastMessage(Utility.GetLocalizedCommonLabel(I18NConstants.Cancelled_Application));
                    mActivity.OnShowDashboard();
                }
                else if (url.Contains(MyHomeConstants.BACK_TO_APPLICATION_STATUS_LANDING))
                {
                    shouldOverride = true;
                    DeeplinkUtil.Instance.SetTargetScreen(Deeplink.ScreenEnum.ApplicationListing);
                    mActivity.OnShowDashboard();
                }
                else if (url.Contains(MyHomeConstants.BACK_TO_HOME))
                {
                    shouldOverride = true;
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