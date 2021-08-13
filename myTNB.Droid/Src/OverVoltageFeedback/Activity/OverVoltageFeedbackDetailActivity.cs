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
using myTNB_Android.Src.AppointmentDetailSet.Activity;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.FeedbackDetails.MVP;
using myTNB_Android.Src.OverVoltageClaimSuccessPage.Activity;
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
        string TempTitle;
        DTOWebView data;

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
                //SetUI();

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
                webView.LoadUrl("http://mytnbwvovis.ap.ngrok.io/claimPage/" + ClaimId);//http://192.168.1.158:3000 //Live https://mytnbwvovis.ap.ngrok.io/claimPage/" + ClaimId// http://192.168.1.158:3000/claimPage/b1683610-34e6-424e-86fd-fce3ae3ab0b //338d6d22-4f04-4065-b7b1-3cb97542faa6 //https://serene-rosalind-a35967.netlify.app/claimPage/" + ClaimId
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
                SetToolBarTitle(Intent.GetStringExtra("TITLE"));
                SetUI();
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

        internal void ResponseFromWebviewAsync(string message)
        {
            data = JsonConvert.DeserializeObject<DTOWebView>(message);
            TempTitle = data.title;
            if (data.title == "Set Appointment")
            {
                SetToolBarTitle("Set Appointment");
            }
            if (data.srNumber != null)
            {
                if (data.title == "CancelAppointment")
                {
                    Intent canclAppointment = new Intent(this, typeof(OverVoltageClaimSuccessPageActivity));
                    canclAppointment.PutExtra("Sernumbr", data.srNumber);
                    canclAppointment.PutExtra("AppointmentFlag", "True");
                    StartActivity(canclAppointment);
                }
                else if (data.title == "CancelEnquiry")
                {
                    Intent canclEnquiry = new Intent(this, typeof(OverVoltageClaimSuccessPageActivity));
                    canclEnquiry.PutExtra("Sernumbr", data.srNumber);
                    canclEnquiry.PutExtra("EnuiryFlag", "True");
                    StartActivity(canclEnquiry);
                }
                else if (data.title == "onAgree")
                {
                    Intent OnAgree = new Intent(this, typeof(OverVoltageClaimSuccessPageActivity));
                    OnAgree.PutExtra("Sernumbr", data.srNumber);
                    OnAgree.PutExtra("AgreeFlag", "True");
                    StartActivity(OnAgree);
                }
                else if (data.title == "onDisagree")
                {

                    Intent OnDisAgree = new Intent(this, typeof(OverVoltageClaimSuccessPageActivity));
                    OnDisAgree.PutExtra("Sernumbr", data.srNumber);
                    OnDisAgree.PutExtra("DisAgreeFlag", "True");
                    StartActivity(OnDisAgree);
                }
                else
                {
                    Intent setAppointment = new Intent(this, typeof(AppointmentSetActivity));
                    setAppointment.PutExtra("Sernumbr", data.srNumber);
                    setAppointment.PutExtra("ApptDate", data.appointmentDate);
                    setAppointment.PutExtra("TechName", data.technicianName);
                    setAppointment.PutExtra("IncdAdd", data.incidentAddress);
                    StartActivity(setAppointment);
                }

            }
            else if (data.title == "Compensation Agreement")
            {
                SetToolBarTitle("Compensation Agreement");
                TempTitle= "Compensation Agreement";

            }
            else if (data.title == "Negotiation Request")
            {
                SetToolBarTitle("Negotiation Request");
                TempTitle = "Negotiation Request";
            }
            else if (data.title == "overvoltageclaim")
            {
                SetToolBarTitle("Overvoltage Claim");
            }
            
        }
        public override void OnBackPressed()
        {
            if (TempTitle == "Set Appointment")
            {

                webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#OnBackAppointment').trigger('click'); },500); })();", null);
            }
            else if (TempTitle == "Compensation Agreement")
            {
                webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#OnBackCompensation').trigger('click'); },500); })();", null);
            }
            else if (TempTitle == "Negotiation Request")
            {
                webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#OnBackNegotiation').trigger('click'); },500); })();", null);
            }
            else
            {
                base.OnBackPressed();
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
        public override bool OnJsAlert(WebView view, string url, string message, JsResult result)
        {
            //return base.OnJsAlert(view, url, message, result);
            result.Cancel();
            _OverVoltageFeedbackDetailActivity.ResponseFromWebviewAsync(message);

            return true;

        }
        public override void OnProgressChanged(WebView view, int newProgress)
        {
            base.OnProgressChanged(view, newProgress);

            if (newProgress == 100)
            {
                this._OverVoltageFeedbackDetailActivity.HideProgressDialog();

            }

        }


    }
    public class DTOWebView
    {
        public string srNumber { get; set; }
        public string appointmentDate { get; set; }
        public string technicianName { get; set; }
        public string incidentAddress { get; set; }
        public string title { get; set; }
        public string claimId { get; set; }
        public string currentScreen { get; set; }
        public string nextScreen { get; set; }
        public string totalAmount { get; set; }
        public string message { get; set; }
    }
}