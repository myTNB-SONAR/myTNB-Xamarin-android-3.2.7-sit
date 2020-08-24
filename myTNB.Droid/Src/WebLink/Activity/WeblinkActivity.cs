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
using Newtonsoft.Json;
using System;
using Weblink = myTNB_Android.Src.AppLaunch.Models.Weblink;

namespace myTNB_Android.Src.WebLink.Activity
{
    [Activity(Label = "@string/weblink_activity_title"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Weblink")]
    public class WeblinkActivity : BaseToolbarAppCompatActivity
    {

        Weblink weblink;

        [BindView(Resource.Id.webView)]
        static WebView webView;

        [BindView(Resource.Id.progressBar)]
        public ProgressBar mProgressBar;

        [BindView(Resource.Id.rootView)]
        public static FrameLayout baseView;

        private static Snackbar mErrorNoInternet;

        public override int ResourceId()
        {
            return Resource.Layout.WeblinkWebView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override string ToolbarTitle()
        {
            return weblink.Title;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            weblink = JsonConvert.DeserializeObject<Weblink>(Intent.Extras.GetString(Constants.SELECTED_WEBLINK));

            base.OnCreate(savedInstanceState);
            webView = FindViewById<WebView>(Resource.Id.webView);
            baseView = FindViewById<FrameLayout>(Resource.Id.rootView);
            mProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            // Create your application here
            webView.Settings.JavaScriptEnabled = (true);
            //webView.SetWebChromeClient(new WebChromeClient());
            webView.SetWebViewClient(new WeblinkClient(this, mProgressBar));

            webView.LoadUrl(weblink.Url);
        }


        public class WeblinkClient : WebViewClient
        {

            public Android.App.Activity mActivity;
            public ProgressBar progressBar;

            public WeblinkClient(Android.App.Activity mActivity, ProgressBar progress)
            {
                this.mActivity = mActivity;
                this.progressBar = progress;

            }

            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                //view.LoadUrl(url);
                return false;
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

            public override void OnReceivedError(WebView view, ClientError errorCode, string description, string failingUrl)
            {
                String message = "Please check your internet connection.";
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
                    ShowErrorMessageNoInternet(failingUrl);
                }
                else
                {
                    ShowErrorMessageNoInternet(failingUrl);
                }

                //Toast.makeText(PaymentWebViewActivity.this,message,Toast.LENGTH_LONG).show();
                webView.LoadUrl("");
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
    }
}