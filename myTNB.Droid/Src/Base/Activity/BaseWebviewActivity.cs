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
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB.AndroidApp.Src.Utils;
using System;

namespace myTNB.AndroidApp.Src.Base.Activity
{
    [Activity(Label = "In-App Browser"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.AddAccount")]
    public class BaseWebviewActivity : BaseToolbarAppCompatActivity
    {
        [BindView(Resource.Id.webView)]
        private static WebView webView;

        [BindView(Resource.Id.progressBar)]
        public ProgressBar mProgressBar;

        [BindView(Resource.Id.rootView)]
        public static FrameLayout baseView;

        private static Snackbar mErrorNoInternet;

        private Snackbar mErrorMessageSnackBar;

        private string HeaderTitle = "";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                webView = FindViewById<WebView>(Resource.Id.webView);
                baseView = FindViewById<FrameLayout>(Resource.Id.rootView);
                mProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

                webView.Settings.JavaScriptEnabled = (true);

                Bundle extra = Intent.Extras;

                string webLink = "";

                if (extra != null)
                {
                    if (extra.ContainsKey("action") && extra.GetString("action") == "contractorRating")
                    {
                        //Todo: Update Back Arrow to X
                    }
                    if (extra.ContainsKey(Constants.IN_APP_LINK))
                    {
                        webLink = extra.GetString(Constants.IN_APP_LINK);
                    }
                    else
                    {
                        this.Finish();
                        return;
                    }

                    if (extra.ContainsKey(Constants.IN_APP_TITLE))
                    {
                        HeaderTitle = extra.GetString(Constants.IN_APP_TITLE);
                        SetToolBarTitle(extra.GetString(Constants.IN_APP_TITLE));
                    }
                }
                else
                {
                    this.Finish();
                    return;
                }

                if (webLink.Contains(".jpeg") || webLink.Contains(".jpg") || webLink.Contains(".png"))
                {
                    webView.Settings.BuiltInZoomControls = true;
                    webView.Settings.SetSupportZoom(true);
                }

                //webView.SetWebChromeClient(new WebChromeClient());
                webView.SetWebViewClient(new MyTNBWebViewClient(this, mProgressBar, webLink, HeaderTitle));

                webView.LoadUrl(webLink);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_AddAccountLarge
                : Resource.Style.Theme_AddAccount);
        }

        protected override void OnStart()
        {
            base.OnStart();

            try
            {
                SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
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
                FirebaseAnalyticsUtils.SetScreenName(this, "In-App Browser");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public class MyTNBWebViewClient : WebViewClient
        {

            public BaseWebviewActivity mActivity;
            public ProgressBar progressBar;
            private string webLink;
            private string HeaderTitle;

            public MyTNBWebViewClient(BaseWebviewActivity mActivity, ProgressBar progress, string mWebLink, string mHeaderTitle)
            {
                this.mActivity = mActivity;
                this.progressBar = progress;
                this.webLink = mWebLink;
                HeaderTitle = mHeaderTitle;
            }

            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                if (ConnectionUtils.HasInternetConnection(mActivity))
                {
                    if (url.Contains("mytnbapp://action=contractorRatingCompleted")
                        || url.Contains("contractorRatingCompleted"))
                    {
                        mActivity.SetResult(Result.Ok);
                        mActivity.Finish();
                    }
                    view.LoadUrl(url);
                }
                else
                {
                    ShowErrorMessageNoInternet(url);
                }
                return true;
            }

            public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
            {
                if (ConnectionUtils.HasInternetConnection(mActivity))
                {
                    base.OnPageStarted(view, url, favicon);
                    progressBar.Visibility = ViewStates.Visible;
                }
                else
                {
                    ShowErrorMessageNoInternet(url);
                }
            }

            public override void OnPageFinished(WebView view, string url)
            {
                progressBar.Visibility = ViewStates.Gone;
                /*if (string.IsNullOrEmpty(HeaderTitle))
                {
                    mActivity.SetToolBarTitle(view.Title);
                }*/
            }

            public override bool OnRenderProcessGone(WebView view, RenderProcessGoneDetail detail)
            {
                return true;
            }

            public override void OnReceivedError(WebView view, ClientError errorCode, string description, string failingUrl)
            {
                try
                {
                    ShowErrorMessageNoInternet(webLink);
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public override void OnReceivedSslError(WebView view, SslErrorHandler handler, SslError error)
            {
                //base.OnReceivedSslError(view, handler, error);
                handler.Proceed();
            }

            public override void OnLoadResource(WebView view, string url)
            {
                //base.OnLoadResource(view, url);
                if (!ConnectionUtils.HasInternetConnection(mActivity))
                {
                    view.StopLoading();
                }
            }
        }

        public static void ShowErrorMessageNoInternet(string failingUrl)
        {
            if (mErrorNoInternet != null && mErrorNoInternet.IsShown)
            {
                mErrorNoInternet.Dismiss();
            }

            mErrorNoInternet = Snackbar.Make(baseView, Utility.GetLocalizedErrorLabel("noDataConnectionMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedLabel("Common", "tryAgain"), delegate
            {
                webView.LoadUrl(failingUrl);
                mErrorNoInternet.Dismiss();
            });
            View v = mErrorNoInternet.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorNoInternet.Show();
        }

        public override int ResourceId()
        {
            return Resource.Layout.BaseWebView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override void OnBackPressed()
        {
            SetResult(Result.Ok);
            Finish();
        }

        //  TODO: AndroidX Temporary Fix for Android 5,5.1 
        //  TODO: AndroidX Due to this: https://github.com/xamarin/AndroidX/issues/131
        public override AssetManager Assets =>
            (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop && Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.M)
            ? Resources.Assets : base.Assets;
    }
}
