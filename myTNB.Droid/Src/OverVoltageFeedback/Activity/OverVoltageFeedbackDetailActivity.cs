using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.FeedbackDetails.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.OverVoltageFeedback.Activity
{
    [Activity(Label = "OverVoltageFeedbackDetailActivity", ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Others")]
    public class OverVoltageFeedbackDetailActivity : BaseToolbarAppCompatActivity, FeedbackDetailsContract.Others.IView
    {


        [BindView(Resource.Id.webView)]
        WebView webView;
        string ClaimId;

        SubmittedFeedbackDetails submittedFeedback;
        private FeedbackDetailsContract.Others.IUserActionsListener userActionsListener;

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.OverVoltageFeedbackDetail;
        }

        public void SetPresenter(FeedbackDetailsContract.Others.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }


        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }


        public void ShowImages(List<AttachedImage> list)
        {
            try
            {

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowInputData(string feedbackId, string feedbackStatus, string feedbackCode, string dateTime, string feedbackType, string feedback)
        {
            try
            {

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                string selectedFeedback = UserSessions.GetSelectedFeedback(Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(this));
                submittedFeedback = JsonConvert.DeserializeObject<SubmittedFeedbackDetails>(selectedFeedback);
                ClaimId = Intent.GetStringExtra("ClaimId");
                if (Intent.HasExtra("TITLE") && !string.IsNullOrEmpty(Intent.GetStringExtra("TITLE")))
                {
                    SetToolBarTitle(Intent.GetStringExtra("TITLE"));
                }

                SetUI();

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private async void SetUI()
        {
            try
            {
                webView = FindViewById<WebView>(Resource.Id.webView);

                WebSettings websettings = webView.Settings;
                //SetWebViewClient with an instance of WebViewClientClass  
                websettings.JavaScriptEnabled = true;
                websettings.LoadWithOverviewMode = true;
                websettings.UseWideViewPort = true;
                websettings.AllowFileAccess = true;
                websettings.AllowContentAccess = true;
                websettings.AllowFileAccessFromFileURLs = true;
                websettings.AllowUniversalAccessFromFileURLs = true;
                websettings.DomStorageEnabled = true;
                websettings.SetAppCacheEnabled(true);
                websettings.JavaScriptEnabled = true; //for set the javascript
                websettings.JavaScriptCanOpenWindowsAutomatically = true;
                // webView.SetWebViewClient(new WebViewClient());
                ShowProgressDialog();
                webView.SetWebChromeClient(new WebViewClient(this, webView) { });
                webView.LoadUrl("https://serene-rosalind-a35967.netlify.app/claimPage/" + ClaimId); //Live https://mytnbwvovis.ap.ngrok.io/claimPage/" + ClaimId// http://192.168.1.158:3000/claimPage/b1683610-34e6-424e-86fd-fce3ae3ab0b //338d6d22-4f04-4065-b7b1-3cb97542faa6 //https://serene-rosalind-a35967.netlify.app/claimPage/" + ClaimId
                await Task.Delay(0);
                //HideProgressDialog();
            }
            catch (Exception ex)
            {

            }
        }
        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Feedback Details");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }
        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
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
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

    }

    internal class WebViewClient : WebChromeClient
    {
        OverVoltageFeedbackDetailActivity _OverVoltageFeedbackDetailActivity;
        private WebView webview;
        //Give the host application a chance to take over the control when a new URL is about to be loaded in the current WebView.  

        public WebViewClient(OverVoltageFeedbackDetailActivity _OverVoltageFeedbackDetailActivity, WebView webview)
        {
            this._OverVoltageFeedbackDetailActivity = _OverVoltageFeedbackDetailActivity;
            this.webview = webview;

        }
        //public override bool OnJsAlert(WebView view, string url, string message, JsResult result)
        //{
        //    //return base.OnJsAlert(view, url, message, result);
        //    result.Cancel();
        //    _FeedbackDetailsOthersActivity.ResponseFromWebviewAsync(message);

        //    return true;

        //}
        public override void OnProgressChanged(WebView view, int newProgress)
        {
            base.OnProgressChanged(view, newProgress);

            if (newProgress == 100)
            {
                this._OverVoltageFeedbackDetailActivity.HideProgressDialog();

            }

        }


    }
}