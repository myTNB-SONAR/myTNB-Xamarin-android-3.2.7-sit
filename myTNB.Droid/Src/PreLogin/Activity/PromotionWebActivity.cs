using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

using Android.Views;
using Android.Webkit;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using System;

namespace myTNB_Android.Src.PreLogin.Activity
{
    [Activity(Label = "Promotions"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.AddAccount")]
    public class PromotionWebActivity : BaseToolbarAppCompatActivity
    {


        [BindView(Resource.Id.webView)]
        private static WebView webView;

        [BindView(Resource.Id.progressBar)]
        public ProgressBar mProgressBar;

        [BindView(Resource.Id.rootView)]
        public static FrameLayout baseView;

        private static Snackbar mErrorNoInternet;

        private Snackbar mErrorMessageSnackBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                string webLink = Intent.Extras.GetString(Constants.PROMOTIONS_LINK);

                webView = FindViewById<WebView>(Resource.Id.webView);
                baseView = FindViewById<FrameLayout>(Resource.Id.rootView);
                mProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

                webView.Settings.JavaScriptEnabled = (true);
                //webView.SetWebChromeClient(new WebChromeClient());
                webView.SetWebViewClient(new MyTNBWebViewClient(this, mProgressBar));

                webView.LoadUrl(webLink);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public class MyTNBWebViewClient : WebViewClient
        {

            public Android.App.Activity mActivity;
            public ProgressBar progressBar;
            private bool isRedirected = false;

            public MyTNBWebViewClient(Android.App.Activity mActivity, ProgressBar progress)
            {
                this.mActivity = mActivity;
                this.progressBar = progress;
            }

            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                if (ConnectionUtils.HasInternetConnection(mActivity))
                {
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
            return Resource.Layout.PromotionWebView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override string ToolbarTitle()
        {
            return GetString(Resource.String.menu_promotions);
        }

        public override void OnBackPressed()
        {
            Finish();
        }
    }
}