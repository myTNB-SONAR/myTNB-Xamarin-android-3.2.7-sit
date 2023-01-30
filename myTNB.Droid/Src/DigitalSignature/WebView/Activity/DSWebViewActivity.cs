using Android.App;
using Android.Content.PM;
using Android.OS;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.DigitalSignature.WebView.MVP;
using myTNB_Android.Src.Utils;
using Android.Webkit;
using myTNB.Mobile;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;
using myTNB;
using myTNB.Mobile.AWS.Models.DS.Identification;
using Newtonsoft.Json;
using Android.Widget;
using Android.Util;
using Android.Net.Http;
using myTNB_Android.Src.myTNBMenu.Activity;
using Android.Content;
using Google.Android.Material.Snackbar;
using Android.Views;
using AndroidX.Core.Content;
using Android;
using System;
using AndroidX.AppCompat.App;
using myTNB.Mobile.Constants.DS;
using myTNB_Android.Src.DigitalSignature.IdentityVerification.MVP;

namespace myTNB_Android.Src.DigitalSignature.WebView.Activity
{
    [Activity(Label = "DS WebView", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class DSWebViewActivity : BaseActivityCustom, DSWebViewContract.IView
    {
        private const string PAGE_ID = DSConstants.PageName_DSWebview;

        private DSWebViewContract.IUserActionsListener userActionsListener;
        private Android.Webkit.WebView micrositeWebView;

        DSDynamicLinkParamsModel _dsDynamicLinkParamsModel;

        private static Snackbar mErrorMessageSnackBar;
        private static FrameLayout mainView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                Bundle extras = Intent.Extras;
                if ((extras != null) && extras.ContainsKey(DigitalSignatureConstants.DS_DYNAMIC_LINK_PARAMS_MODEL))
                {
                    _dsDynamicLinkParamsModel = JsonConvert.DeserializeObject<DSDynamicLinkParamsModel>(extras.GetString(DigitalSignatureConstants.DS_DYNAMIC_LINK_PARAMS_MODEL));
                }

                _ = new DSWebViewPresenter(this);
                this.userActionsListener?.OnInitialize();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetUpViews()
        {
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

            SetToolBarTitle(GetLabelByLanguage(DSConstants.I18N_Title));
            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

            micrositeWebView = FindViewById<Android.Webkit.WebView>(Resource.Id.digitalSignatureWebView);

            PrepareWebView();
        }

        public void SetPresenter(DSWebViewContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override int ResourceId()
        {
            return Resource.Layout.DSWebView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        public override void OnBackPressed()
        {
            ShowBackPopUpConfirmation();
        }

        public override bool CameraPermissionRequired()
        {
            return true;
        }

        private void ShowBackPopUpConfirmation()
        {
            RunOnUiThread(() =>
            {
                MyTNBAppToolTipBuilder marketingTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.MYTNB_DIALOG_ICON_TWO_BUTTON)
                   .SetHeaderImage(Resource.Drawable.ic_display_validation_success)
                   .SetTitle(GetLabelByLanguage(DSConstants.I18N_ConfirmPopupTitle))
                   .SetMessage(GetLabelByLanguage(DSConstants.I18N_ConfirmPopupMessage))
                   .SetCTALabel(GetLabelByLanguage(DSConstants.I18N_Stay))
                   .SetSecondaryCTALabel(GetLabelByLanguage(DSConstants.I18N_Leave))
                   .SetCTAaction(() => { })
                   .SetSecondaryCTAaction(() => LeaveOnClick())
                   .Build();
                marketingTooltip.Show();

                DynatraceHelper.OnTrack(DynatraceConstants.DS.Screens.Popup.Leave_Site);
            });
        }

        public void LeaveOnClick()
        {
            DynatraceHelper.OnTrack(DynatraceConstants.DS.CTAs.Microsite.PopUp_Leave);
            SetResult(Result.Canceled);
            Finish();
        }

        private void PrepareWebView()
        {
            UserEntity user = UserEntity.GetActive();
            string myTNBAccountName = user?.DisplayName ?? string.Empty;
            string email = string.Empty;
            bool isContractorApplied = false;
            string appRef = string.Empty;
            int? ApplicationModuleID = null;
            int? IdentificationType = null;
            string identificationNo = string.Empty;

            if (_dsDynamicLinkParamsModel != null)
            {
                if (_dsDynamicLinkParamsModel.Email != null)
                {
                    email = _dsDynamicLinkParamsModel.Email;
                }

                isContractorApplied = _dsDynamicLinkParamsModel.IsContractorApplied;
                if (_dsDynamicLinkParamsModel.AppRef != null)
                {
                    appRef = _dsDynamicLinkParamsModel.AppRef;
                }

                if (_dsDynamicLinkParamsModel.ApplicationModuleID != null)
                {
                    ApplicationModuleID = int.Parse(_dsDynamicLinkParamsModel.ApplicationModuleID);
                }

                IdentificationType = _dsDynamicLinkParamsModel.IdentificationType;

                if (_dsDynamicLinkParamsModel.IdentificationNo != null)
                {
                    identificationNo = _dsDynamicLinkParamsModel.IdentificationNo;
                }
            }

            string signature = SSOManager.Instance.GetDSSignature(myTNBAccountName
                , AccessTokenCache.Instance.GetAccessToken(this)
                , user?.DeviceId ?? string.Empty
                , DeviceIdUtils.GetAppVersionName().Replace("v", string.Empty)
                , 16
                , (LanguageUtil.GetAppLanguage() == "MS" ? LanguageManager.Language.MS : LanguageManager.Language.EN).ToString()
                , TextViewUtils.FontInfo ?? "N"
                , user?.UserID
                , IdentificationType
                , identificationNo
                , GetIntFromStringValue(Constants.DEVICE_PLATFORM)
                , email
                , isContractorApplied
                , appRef
                , ApplicationModuleID);

            string ssoURL = string.Format(AWSConstants.Domains.DSSSO, signature);

            micrositeWebView.SetWebChromeClient(new MyTNBWebChromeClient(this) { });
            micrositeWebView.SetWebViewClient(new MyTNBWebViewClient(this));
            micrositeWebView.Settings.JavaScriptEnabled = true;
            micrositeWebView.Settings.AllowFileAccess = true;
            micrositeWebView.Settings.AllowFileAccessFromFileURLs = true;
            micrositeWebView.Settings.AllowUniversalAccessFromFileURLs = true;
            micrositeWebView.Settings.AllowContentAccess = true;
            micrositeWebView.Settings.JavaScriptCanOpenWindowsAutomatically = true;
            micrositeWebView.Settings.DomStorageEnabled = true;
            micrositeWebView.Settings.MediaPlaybackRequiresUserGesture = false;
            micrositeWebView.LoadUrl(ssoURL);
        }

        internal void OnShowDashboard()
        {
            Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
            HomeMenuUtils.ResetAll();
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);
        }

        private int GetIntFromStringValue(string val)
        {
            try
            {
                Int32.TryParse(val, out int parsedValue);
                return parsedValue;
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
                return 1;
            }
        }

        internal class MyTNBWebChromeClient : WebChromeClient
        {
            DSWebViewActivity _dSWebViewActivity;
            View? _customView = null;
            ICustomViewCallback? _customViewCallback = null;
            ScreenOrientation _originalOrientation = ScreenOrientation.Unspecified; 
            ViewStates _originalVisibility = ViewStates.Invisible;

            public MyTNBWebChromeClient(DSWebViewActivity dSWebViewActivity)
            {
                _dSWebViewActivity = dSWebViewActivity;
            }

            public override void OnPermissionRequest(PermissionRequest? request)
            {
                DynatraceHelper.OnTrack(DynatraceConstants.DS.Screens.Popup.Camera_Permission);

                if (ContextCompat.CheckSelfPermission(_dSWebViewActivity, Manifest.Permission.Camera) == (int)Permission.Granted)
                {
                    request?.Grant(new String[] { PermissionRequest.ResourceVideoCapture });
                }
                else
                {
                    request?.Deny();
                }
            }

            public override void OnShowCustomView(View? view, ICustomViewCallback? callback)
            {
                base.OnShowCustomView(view, callback);
                
                try
                {
                    if (_customView != null)
                    {
                        OnHideCustomView();
                        return;
                    }

                    _customView = view;
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
                    {
                        _originalVisibility = _dSWebViewActivity.Window.DecorView.Visibility;
                        (_dSWebViewActivity.Window.DecorView as FrameLayout).AddView(_customView, new FrameLayout.LayoutParams(-1, -1));
                        _dSWebViewActivity.Window.SetDecorFitsSystemWindows(false);
                    }
                    else
                    {
                        _originalVisibility = _dSWebViewActivity.Window.DecorView.WindowVisibility;
                        (_dSWebViewActivity.Window.DecorView as FrameLayout).AddView(_customView, new FrameLayout.LayoutParams(-1, -1));
                        _dSWebViewActivity.Window.DecorView.Visibility = ViewStates.Visible;
                    }
                    _originalOrientation = _dSWebViewActivity.RequestedOrientation;
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public override void OnHideCustomView()
            {
                base.OnHideCustomView();

                try
                {
                    (_dSWebViewActivity.Window.DecorView as FrameLayout).RemoveView(_customView);
                    _customView = null;

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
                    {
                        _dSWebViewActivity.Window.SetDecorFitsSystemWindows(true);
                    }
                    else
                    {
                        _dSWebViewActivity.Window.DecorView.Visibility = _originalVisibility;
                    }
                    _dSWebViewActivity.RequestedOrientation = _originalOrientation;
                    if (_customViewCallback != null)
                    {
                        _customViewCallback.OnCustomViewHidden();
                    }
                    _customViewCallback = null;
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
        }

        public class MyTNBWebViewClient : WebViewClient
        {
            private DSWebViewActivity mActivity;
            private ProgressBar progressBar;
            private Boolean transref = false;

            public MyTNBWebViewClient(DSWebViewActivity mActivity)
            {
                this.mActivity = mActivity;
            }

            public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, string url)
            {
                bool shouldOverride = false;
                if (ConnectionUtils.HasInternetConnection(mActivity))
                {
                    var act = this.mActivity as AppCompatActivity;
                    var actionBar = act.SupportActionBar;

                    Log.Debug("[DEBUG]", "MyTNBWebViewClient url: " + url.ToString());
                    if (url.ToLower().Contains(DigitalSignatureConstants.DS_ACT_BACK_TO_APP))
                    {
                        mActivity.LeaveOnClick();
                        shouldOverride = true;
                    }
                    else if (url.ToLower().Contains(DigitalSignatureConstants.DS_ACT_BACK_TO_HOME))
                    {
                        mActivity.OnShowDashboard();
                        shouldOverride = true;
                    }

                    if (url.ToString().ToLower().Contains(DigitalSignatureConstants.DS_EKYC_SUCCESS))
                    {
                        actionBar.Hide();

                        DynatraceHelper.OnTrack(DynatraceConstants.DS.Screens.Microsite.Landing_Success);
                    }
                    else if (url.ToString().ToLower().Contains(DigitalSignatureConstants.DS_EKYC_ERROR))
                    {
                        actionBar.Hide();

                        DynatraceHelper.OnTrack(DynatraceConstants.DS.Screens.Microsite.Landing_Error);
                    }
                }
                return shouldOverride;
            }

            public override void OnPageStarted(Android.Webkit.WebView view, string url, Android.Graphics.Bitmap favicon)
            {
                try
                {
                    var act = this.mActivity as AppCompatActivity;
                    var actionBar = act.SupportActionBar;

                    base.OnPageStarted(view, url, favicon);
                    Log.Debug("[DEBUG]", "OnPageStarted url: " + url.ToString());

                    view.LoadUrl("javascript:(function f() { window['__NVW_WEBVIEW__'] = { isAndroid: true } } )()");

                    if (url.ToString().ToLower().Contains(DigitalSignatureConstants.DS_EKYC_START))
                    {
                        actionBar.Show();
                    }
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public override void OnPageFinished(Android.Webkit.WebView view, string url)
            {
                try
                {
                    var act = this.mActivity as AppCompatActivity;
                    var actionBar = act.SupportActionBar;

                    Log.Debug("[DEBUG]", "OnPageFinished url: " + url.ToString());

                    if (url.ToString().ToLower().Contains("transref"))
                    {
                        transref = true;
                    }

                    if ((url.ToString().ToLower().Contains(DigitalSignatureConstants.DS_EKYC_START)) && (transref = true))
                    {
                        transref = false;
                        DynatraceHelper.OnTrack(DynatraceConstants.DS.CTAs.Microsite.Try_Again);
                    }
                    else if (url.ToString().ToLower().Contains(DigitalSignatureConstants.DS_EKYC_START))
                    {
                        actionBar.Show();
                    }
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public override bool OnRenderProcessGone(Android.Webkit.WebView view, RenderProcessGoneDetail detail)
            {
                return true;
            }

            public override void OnReceivedError(Android.Webkit.WebView view, ClientError errorCode, string description, string failingUrl)
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

            public override void OnLoadResource(Android.Webkit.WebView view, string url)
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
