using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

using Android.Views;
using Android.Webkit;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Runtime;

namespace myTNB_Android.Src.SmartMeterLearnMore.Activity
{
    [Activity(Label = "@string/smart_meter_learn_more_activity_title"
          , ScreenOrientation = ScreenOrientation.Portrait
          , Theme = "@style/Theme.SmartMeter")]
    public class SmartMeterLearnMoreActivity : BaseToolbarAppCompatActivity
    {

        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.webView)]
        private WebView webView;

        [BindView(Resource.Id.progressBar)]
        public ProgressBar mProgressBar;


        Weblink webLink;

        public override int ResourceId()
        {
            return Resource.Layout.SmartMeterLearnMoreView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            try
            {
                webLink = JsonConvert.DeserializeObject<Weblink>(Intent.Extras.GetString(Constants.SMART_METER_LINK));
                webView.Settings.JavaScriptEnabled = true;
                webView.SetWebViewClient(new SmartMeterLearnMoreWebClient(this, mProgressBar, webLink, webView));
                webView.LoadUrl(webLink.Url);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mErrorNoInternet;
        public void ShowErrorMessageNoInternet(string failingUrl)
        {
            if (mErrorNoInternet != null && mErrorNoInternet.IsShown)
            {
                mErrorNoInternet.Dismiss();
            }

            mErrorNoInternet = Snackbar.Make(rootView, GetString(Resource.String.smart_meter_learn_more_snackbar_error_no_internet), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.smart_meter_learn_more_snackbar_error_btn), delegate
            {
                webView.LoadUrl(failingUrl);
                mErrorNoInternet.Dismiss();
            });
            View v = mErrorNoInternet.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorNoInternet.Show();
        }

        public class SmartMeterLearnMoreWebClient : WebViewClient
        {
            private ProgressBar progressBar;
            private Weblink webLink;
            private SmartMeterLearnMoreActivity activity;
            private WebView webView;

            public SmartMeterLearnMoreWebClient(SmartMeterLearnMoreActivity activity, ProgressBar progressBar, Weblink webLink, WebView webView)
            {
                this.progressBar = progressBar;
                this.webLink = webLink;
                this.activity = activity;
                this.webView = webView;
            }

            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                if (ConnectionUtils.HasInternetConnection(activity))
                {
                    view.LoadUrl(url);
                }
                else
                {
                    activity.ShowErrorMessageNoInternet(url);
                }
                return true;
            }

            public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
            {
                if (ConnectionUtils.HasInternetConnection(activity))
                {
                    base.OnPageStarted(view, url, favicon);
                    progressBar.Visibility = ViewStates.Visible;
                    webView.Visibility = ViewStates.Gone;
                }
                else
                {
                    activity.ShowErrorMessageNoInternet(url);
                }
            }

            public override void OnPageFinished(WebView view, string url)
            {
                progressBar.Visibility = ViewStates.Gone;
                webView.Visibility = ViewStates.Visible;
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
    }
}