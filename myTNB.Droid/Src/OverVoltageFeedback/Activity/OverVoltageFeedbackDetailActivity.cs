using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.AppointmentDetailSet.Activity;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FeedbackDetails.MVP;
using myTNB_Android.Src.FeedBackSubmittedSucess.Activity;
using myTNB_Android.Src.OverVoltageClaimSuccessPage.Activity;
using myTNB_Android.Src.PaymentInfoSunmittedSuccess.Activity;
using myTNB_Android.Src.SelectSubmittedFeedback.Activity;
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

        [BindView(Resource.Id.txtstep1of2)]
        TextView txtstep1of2;

        string ClaimId;
        string TempTitle,TempStepperTitle;
        DTOWebView data;
        private string accNo = null;

        bool IsfromPaymentInfoSubmittedSucces = false;
        bool IsfromFeedBackSubmittedSucces = false;
        bool IsfromSetAppointmentSucces = false;
        public static bool proccedToPaymentFlag = false;
        public static bool backFromAppointmentFlag = false;
        bool setAppointmentFlag = false;
        SubmittedFeedbackDetails submittedFeedback;
        private FeedbackDetailsContract.Others.IUserActionsListener userActionsListener;
        //File upload
        public static bool isCapture = false;
        private Action<int, Result, Intent> resultCallbackvalue;

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
                Android.OS.Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.ACCOUNT_NUMBER))
                    {
                        accNo = extras.GetString(Constants.ACCOUNT_NUMBER);
                    }

                }
                string selectedFeedback = UserSessions.GetSelectedFeedback(Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(this));
                submittedFeedback = JsonConvert.DeserializeObject<SubmittedFeedbackDetails>(selectedFeedback);
                ClaimId = Intent.GetStringExtra("ClaimId");
                if (Intent.HasExtra("TITLE") && !string.IsNullOrEmpty(Intent.GetStringExtra("TITLE")))
                {
                    SetToolBarTitle(Intent.GetStringExtra("TITLE"));
                    TempTitle = Intent.GetStringExtra("TITLE");
                }
                setAppointmentFlag = Convert.ToBoolean(Intent.GetStringExtra("setAppointmentFlag"));
                //proccedToPaymentFlag = Convert.ToBoolean(Intent.GetStringExtra("proccedToPaymentFlag"));
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
                // WebView Live
                var domain = "http://mytnbwvovis.ap.ngrok.io/";

                // WebView Local
                //var domain = "http://192.168.1.158:3000/";

                String EndPoint = "";
                
                
                
                if (proccedToPaymentFlag)
                {
                    EndPoint = "?paymentInfo=true";
                    proccedToPaymentFlag = false;
                }
                else if (setAppointmentFlag)
                {
                    EndPoint = "?setAppointment=true";
                    setAppointmentFlag = false;
                }
                //else if (backFromAppointmentFlag)
                //{
                //    EndPoint = "";
                //    SetToolBarTitle("Overvoltage Claim");

                //}
                else
                {
                    EndPoint = "";
                    SetToolBarTitle("Overvoltage Claim");
                    TempTitle = "Overvoltage Claim";
                }
                var url = domain + "claimPage/" + ClaimId + EndPoint;

                #if DEBUG
                global::Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
                #endif

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
                webView.LoadUrl(url); //"https://mytnbwvovis.ap.ngrok.io/claimPage/" + ClaimId  //Live https://mytnbwvovis.ap.ngrok.io/claimPage/" + ClaimId// http://192.168.1.158:3000/claimPage/b1683610-34e6-424e-86fd-fce3ae3ab0b //338d6d22-4f04-4065-b7b1-3cb97542faa6 //https://serene-rosalind-a35967.netlify.app/claimPage/" + ClaimId
                await Task.Delay(0);
                //HideProgressDialog();
                //File upload
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) != (int)Permission.Granted)
                {
                    RequestPermissions(new string[] { Manifest.Permission.Camera }, 0);
                }


                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
                {
                    RequestPermissions(new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage }, 0);
                }

            }
            catch (Exception ex)
            {

            }
        }
        //File upload
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void StartsActivity(Intent intent, int requestCode, Action<int, Result, Intent> resultCallback)
        {
            this.resultCallbackvalue = resultCallback;
            isCapture = true;
            StartActivityForResult(intent, requestCode);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (this.resultCallbackvalue != null)
            {
                this.resultCallbackvalue(requestCode, resultCode, data);
                this.resultCallbackvalue = null;
            }
        }
        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                ClaimId = Intent.GetStringExtra("ClaimId");
                //SetToolBarTitle(Intent.GetStringExtra("TITLE"));
                IsfromFeedBackSubmittedSucces = Convert.ToBoolean(Intent.GetStringExtra("IsfromPaymentInfoSubmittedSucces"));
                IsfromPaymentInfoSubmittedSucces = Convert.ToBoolean(Intent.GetStringExtra("IsfromPaymentInfoSubmittedSucces"));
                IsfromSetAppointmentSucces = Convert.ToBoolean(Intent.GetStringExtra("IsfromSetAppointmentSucces"));
                if (IsfromFeedBackSubmittedSucces == true)
                {
                    txtstep1of2.Visibility = ViewStates.Gone;
                }
                else if (IsfromPaymentInfoSubmittedSucces == true)
                {
                    txtstep1of2.Visibility = ViewStates.Gone;
                }
                else if (IsfromSetAppointmentSucces)
                {
                    SetToolBarTitle("Set Appointment");
                    TempTitle = "Set Appointment";
                }
                else if(proccedToPaymentFlag)
                {
                    SetToolBarTitle("Enter Payment Details");
                    TempTitle = "Enter Payment Details";
                }
                TempTitle = Intent.GetStringExtra("TITLE");

                if (!isCapture)
                {
                    SetUI();
                }
                else
                {
                    SetToolBarTitle("Enter Payment Details");
                    TempTitle = "Enter Payment Details";
                    isCapture = false;
                }

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
            try
            {
                data = JsonConvert.DeserializeObject<DTOWebView>(message);
                //TempTitle = data.title;
                if (data.title == "Set Appointment")
                {
                    SetToolBarTitle("Set Appointment");
                    TempTitle = "Set Appointment";
                }                

                if (data.srNumber != null)
                {
                    if (data.title == "CancelAppointment")
                    {
                        TempTitle = "Set Appointment";
                        Intent canclAppointment = new Intent(this, typeof(OverVoltageClaimSuccessPageActivity));
                        canclAppointment.PutExtra("Sernumbr", data.srNumber);
                        canclAppointment.PutExtra("AppointmentFlag", "True");
                        canclAppointment.PutExtra("ClaimId", ClaimId);
                        StartActivity(canclAppointment);
                    }
                    else if (data.title == "CancelEnquiry")
                    {
                        TempTitle = "CancelEnquiry";
                        Intent canclEnquiry = new Intent(this, typeof(OverVoltageClaimSuccessPageActivity));
                        canclEnquiry.PutExtra("Sernumbr", data.srNumber);
                        canclEnquiry.PutExtra("EnuiryFlag", "True");
                        StartActivity(canclEnquiry);
                    }
                    else if (data.title == "onAgree")
                    {
                        TempTitle = "onAgree";
                        Intent OnAgree = new Intent(this, typeof(OverVoltageClaimSuccessPageActivity));
                        OnAgree.PutExtra("Sernumbr", data.srNumber);
                        OnAgree.PutExtra("AgreeFlag", "True");
                        OnAgree.PutExtra("ClaimId", ClaimId);
                        StartActivity(OnAgree);
                    }
                    else if (data.title == "onDisagree")
                    {
                        TempTitle = "onDisagree";
                        Intent OnDisAgree = new Intent(this, typeof(OverVoltageClaimSuccessPageActivity));
                        OnDisAgree.PutExtra("Sernumbr", data.srNumber);
                        OnDisAgree.PutExtra("DisAgreeFlag", "True");
                        StartActivity(OnDisAgree);
                    }
                    else if (data.currentScreen == "submit" && TempTitle == "Enter Payment Details")
                    {
                        Intent paymentInfoSubmitted = new Intent(this, typeof(PaymentInfoSunmittedSuccessActivity));
                        paymentInfoSubmitted.PutExtra("Sernumbr", data.srNumber);
                        paymentInfoSubmitted.PutExtra("TotalAmt", data.totalAmount);
                        paymentInfoSubmitted.PutExtra("ClaimId", data.claimId);
                        StartActivity(paymentInfoSubmitted);
                    }
                    else if (data.currentScreen == "submit" && TempTitle == "Update Payment Details")
                    {
                        Intent paymentInfoSubmitted = new Intent(this, typeof(PaymentInfoSunmittedSuccessActivity));
                        paymentInfoSubmitted.PutExtra("Sernumbr", data.srNumber);
                        paymentInfoSubmitted.PutExtra("TotalAmt", data.totalAmount);
                        paymentInfoSubmitted.PutExtra("ClaimId", data.claimId);
                        StartActivity(paymentInfoSubmitted);
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
                else if (!string.IsNullOrEmpty(data.crStatus))
                {
                    if (data.crStatus != "NULL")
                    {
                        SelectSubmittedFeedbackActivity.status = data.crStatus;//"Cancelled";
                    }
                    //NavigationController.SetNavigationBarHidden(false, false);
                    base.OnBackPressed();
                }
                else if (data.title == "Compensation Agreement")
                {
                    SetToolBarTitle("Compensation Agreement");
                    TempTitle = "Compensation Agreement";

                }
                else if (data.title == "Negotiation Request")
                {
                    SetToolBarTitle("Negotiation Request");
                    TempTitle = "Negotiation Request";
                }
                else if (data.title == "Enter Payment Details")
                {
                    SetToolBarTitle("Enter Payment Details");
                    TempTitle = "Enter Payment Details";
                    //TempStepperTitle = "Enter Payment Details";
                }
                else if (data.title == "Update Payment Details")
                {
                    SetToolBarTitle("Update Payment Details");
                    TempTitle = "Update Payment Details";
                    txtstep1of2.Visibility = ViewStates.Visible;
                    txtstep1of2.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle1of2");

                }
                else if (data.currentScreen == "1" && data.nextScreen == "2" && TempTitle == "Enter Payment Details")
                {
                    txtstep1of2.Visibility = ViewStates.Visible;
                    txtstep1of2.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle1of2");
                }
                else if (data.currentScreen == "1" && data.nextScreen == "overvoltageclaim")
                {
                    txtstep1of2.Visibility = ViewStates.Gone;
                    SetToolBarTitle("Overvoltage Claim");
                    TempTitle = "Overvoltage Claim";
                }
                else if (data.currentScreen == "1" && data.nextScreen == "2" && data.title == "Update Payment Details")
                {
                    SetToolBarTitle("Update Payment Details");
                    txtstep1of2.Visibility = ViewStates.Visible;
                    txtstep1of2.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle1of2");
                }
                else if (data.currentScreen == "2" && data.nextScreen == "submit")
                {
                    txtstep1of2.Visibility = ViewStates.Visible;
                    txtstep1of2.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle2of2");
                }
                else if (data.currentScreen == "2" && data.nextScreen == "1")
                {
                    txtstep1of2.Visibility = ViewStates.Visible;
                    txtstep1of2.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle1of2");
                }
                else if (data.currentScreen == "0" && data.nextScreen == "overvoltageclaim")
                {
                    string toolbartitle = Intent.GetStringExtra("TITLE");
                    SetToolBarTitle(Intent.GetStringExtra("TITLE"));
                    //base.OnBackPressed();
                }
                //back to start screen
                else if (data.currentScreen == "1" && data.nextScreen == "0")
                {
                    txtstep1of2.Visibility = ViewStates.Gone;
                }
                //else if (data.currentScreen == "1" && data.nextScreen == "2")
                //{
                //    txtstep1of2.Visibility = ViewStates.Gone;
                //}
                else if (data.currentScreen == "0" && data.nextScreen == "overvoltageclaim")
                {
                    txtstep1of2.Visibility = ViewStates.Visible;
                    txtstep1of2.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle2of2");
                }
                else if (data.title == "Rate Your Experience")
                {
                    SetToolBarTitle("Rate Your Experience");
                    TempTitle = "Rate Your Experience";

                }
                else if (data.title == "overvoltageclaim")
                {
                    SetToolBarTitle("Overvoltage Claim");
                    TempTitle = "Overvoltage Claim";
                }
                else if (!string.IsNullOrEmpty(data.claimId))
                {
                    Intent FeedBackSubmittedSuccess = new Intent(this, typeof(FeedBackSubmittedSuccessActivity));
                    FeedBackSubmittedSuccess.PutExtra("ClaimId", data.claimId);
                    StartActivity(FeedBackSubmittedSuccess);
                }
            }
            catch (Exception ex)
            {

            }
            


        }
        public override void OnBackPressed()
        {
            try
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
                else if (TempTitle == "Overvoltage Claim")
                {
                   //base.OnBackPressed();
                   webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#getCurrentStatusForMobileApp').trigger('click'); },500); })();", null);
                   
                }
                else if (data == null)
                {
                    base.OnBackPressed();
                }
                else if (data != null)
                {
                    //if (data.title == "Update Payment Details")
                    //{
                    //    webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#onBackUpdatePaymentStep').trigger('click'); },500); })();", null);
                    //}

                    if (data.currentScreen == "0" && data.nextScreen == "1")
                    {
                        webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#onBackPaymnet').trigger('click'); },500); })();", null);
                    }
                    else if (data.title == "CancelEnquiry")
                    {
                        base.OnBackPressed();
                    }
                    else if (data.currentScreen == "1" && data.nextScreen == "2" && TempTitle == "Enter Payment Details")
                    {
                        webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#onBackPaymentStep').trigger('click'); },500); })();", null);
                    }
                    else if (data.currentScreen == "1" && data.nextScreen == "2" && TempTitle == "Update Payment Details")
                    {
                        webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#onBackUpdatePaymentStep').trigger('click'); },500); })();", null);
                    }
                    else if (data.currentScreen == "2" && data.nextScreen == "submit" && TempTitle == "Enter Payment Details")
                    {
                        webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#onBackPaymentStep').trigger('click'); },500); })();", null);
                    }
                    else if (data.currentScreen == "2" && data.nextScreen == "submit" && TempTitle == "Update Payment Details")
                    {
                        webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#onBackUpdatePayment').trigger('click'); },500); })();", null);
                    }
                    else if (data.currentScreen == "2" && data.nextScreen == "1" && TempTitle == "Enter Payment Details")
                    {
                        webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#onBackPaymentStep').trigger('click'); },500); })();", null);
                    }
                    else if (data.currentScreen == "2" && data.nextScreen == "1" && TempTitle == "Update Payment Details")
                    {
                        webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#onBackUpdatePaymentStep').trigger('click'); },500); })();", null);
                    }
                    else if (data.currentScreen == "1" && data.nextScreen == "0")
                    {
                        webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#onBackPaymnet').trigger('click'); },500); })();", null);
                    }
                    else if (data.title == "Rate Your Experience")
                    {
                        webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#OnBackRatingPage').trigger('click'); },500); })();", null);

                    }
                    else if (data.currentScreen == "1" && data.nextScreen == "0")
                    {
                        base.OnBackPressed();
                    }
                    else if (data.currentScreen == "0" && data.nextScreen == "overvoltageclaim")
                    {
                        base.OnBackPressed();
                    }
                    else if (data.title == "overvoltageclaim" )
                    {
                        if (TempTitle == "Overvoltage Claim")
                        {
                            //base.OnBackPressed();
                            webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#getCurrentStatusForMobileApp').trigger('click'); },500); })();", null);
                        }
                        //base.OnBackPressed();

                    }

                }

                else
                {
                    base.OnBackPressed();
                }
            }
            catch (Exception ex)
            {

            }


        }
        internal void PassData()
        {
            try
            {
                var data = new MyTNBService.Request.BaseRequest();
                var usin = data.usrInf;
                var ac = accNo.Trim();
                var datajson = JsonConvert.SerializeObject(usin);
                Console.WriteLine(datajson);

                UserEntity user = UserEntity.GetActive();
                //webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { getUserInfo('" + ac +"', '" + user.IdentificationNo + "', '" + user.UserID + "', '" + user.DisplayName + "','" + datajson + "') },100); })();", null);
                webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { getUserInfo('" + ac + "', '" + user.IdentificationNo + "', '" + user.UserID + "', '" + user.DisplayName + "','" + usin.eid + "','" + usin.lang + "','" + usin.sec_auth_k1 + "','" + Utility.GetLocalizedLabel("SubmitEnquiry", "defaultErrorMessage") + "', '" + user.MobileNo + "') },100); })();", null);


            }
            catch (Exception ex)
            {

            }
        }
    }

    internal class WebViewClient : WebChromeClient
    {
        OverVoltageFeedbackDetailActivity _OverVoltageFeedbackDetailActivity;
        private WebView webview;
        private static int filechooser = 1;
        private IValueCallback message;
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

            this._OverVoltageFeedbackDetailActivity.StartsActivity(contentSelectionIntent, filechooser, this.OnActivityResult);

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
                        //`enter code here`
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
        public string crStatus { get; set; }
}
}