using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
using myTNB.AndroidApp.Src.AppointmentDetailSet.Activity;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.FeedbackDetails.MVP;
using myTNB.AndroidApp.Src.FeedBackSubmittedSucess.Activity;
using myTNB.AndroidApp.Src.OverVoltageClaimSuccessPage.Activity;
using myTNB.AndroidApp.Src.PaymentInfoSunmittedSuccess.Activity;
using myTNB.AndroidApp.Src.SelectSubmittedFeedback.Activity;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using BaseRequest = myTNB.AndroidApp.Src.MyTNBService.Request.BaseRequest;

using myTNB.AndroidApp.Src.AppLaunch.Activity;
using myTNB.AndroidApp.Src.myTNBMenu.Activity;
using myTNB.AndroidApp.Src.Helper;
using myTNB.AndroidApp.Src.OverVoltageFeedback.Model;
using myTNB.Mobile;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;

namespace myTNB.AndroidApp.Src.OverVoltageFeedback.Activity
{
    [Activity(Label = "OverVoltageFeedbackDetailActivity"
          , ScreenOrientation = ScreenOrientation.Portrait
          , WindowSoftInputMode = SoftInput.AdjustResize
          , Theme = "@style/Theme.Others")]

    public class OverVoltageFeedbackDetailActivity : BaseToolbarAppCompatActivity
    {
        [BindView(Resource.Id.webView)]
        WebView webView;

        [BindView(Resource.Id.txtstep1of2)]
        TextView txtstep1of2;

        public bool FcmPushNotificationFlagFromBackground;
        public static bool FcmPushNotificationFlagFromForkground;
        private static System.Timers.Timer aTimer;
        MyTNBAppToolTipBuilder leaveDialog;
        string ClaimId;
        string TempTitle,TempStepperTitle;
        DTOWebView data;
        private string accNo = null;
        public static bool isRescheduleappointment;
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
        public bool IsStopTime = false;
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            isCapture = false;
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
                    TempTitle = "Overvoltage Claim";
                }
                setAppointmentFlag = Convert.ToBoolean(Intent.GetStringExtra("setAppointmentFlag"));              
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
                TextViewUtils.SetMuseoSans300Typeface(txtstep1of2);
                TextViewUtils.SetTextSize12(txtstep1of2);
                //device info
                var AppVersion = DeviceIdUtils.GetAppVersionName();
                var OsVersion = "Android" + DeviceIdUtils.GetAndroidVersion();
                var DeviceModel = DeviceInfo.Model;
                var Manufacturer = DeviceInfo.Manufacturer;
                var data = new BaseRequest();
                var usin = data.usrInf;
                UserEntity user = UserEntity.GetActive();
                
                string domain = MobileConstants.OvisWebviewBaseUrl + "/claimPage/" +ClaimId; //WebView Live

                UrlUtility urlUtility = new UrlUtility();
                urlUtility.AddQueryParams("eid", usin.eid);
                urlUtility.AddQueryParams("appVersion", AppVersion);
                urlUtility.AddQueryParams("os", OsVersion);
                urlUtility.AddQueryParams("Manufacturer", Manufacturer);
                urlUtility.AddQueryParams("model", DeviceModel);
                urlUtility.AddQueryParams("session_id", LaunchViewActivity.DynatraceSessionUUID);
                urlUtility.AddQueryParams("lang", usin.lang);
                urlUtility.AddQueryParams("IDCN", user.IdentificationNo);
                urlUtility.AddQueryParams("userID", user.UserID);
                urlUtility.AddQueryParams("name", user.DisplayName);
                urlUtility.AddQueryParams("sec_auth_k1", usin.sec_auth_k1);
                urlUtility.AddQueryParams("mobileNo", user.MobileNo);

                if (proccedToPaymentFlag)
                {                   
                    urlUtility.AddQueryParams("paymentInfo", "true");                   
                    proccedToPaymentFlag = false;
                }
                else if (setAppointmentFlag)
                {                    
                    urlUtility.AddQueryParams("setAppointment", "true");                   
                    setAppointmentFlag = false;
                }               
                else
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimTitle"));
                    TempTitle = "Overvoltage Claim";
                }
                string url = urlUtility.EncodeURL(domain);

                if (TextViewUtils.IsLargeFonts)
                {
                    url += "&large";
                }


                #if DEBUG
                //global::Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
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
                webView.LoadUrl(url);
                await Task.Delay(0);
                SetTimer();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        public void SetTimer()
        {
            aTimer = new System.Timers.Timer(25000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        public void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (!IsStopTime)
            {
                HideProgressDialog();
                RunOnUiThread(() => { showpopup(); });
                aTimer.Stop();
            }
            aTimer.Stop();
        }
        public void showpopup()
        {
            leaveDialog = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
              .SetTitle(Utility.GetLocalizedLabel("Error", "defaultErrorTitle"))
              .SetMessage(Utility.GetLocalizedLabel("Error", "defaultErrorMessage"))
              .SetCTALabel(Utility.GetLocalizedLabel("Common", "ok"))
              .SetCTAaction(() => { base.OnBackPressed(); })
              .Build();
            leaveDialog.Show();
        }
        //File upload
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);          
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
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "setAppointmentTitle")); 
                     TempTitle = "Set Appointment";
                }
                else if(proccedToPaymentFlag)
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "enterPaymentDetailsTitle"));
                    TempTitle = "Enter Payment Details";
                }
                //TempTitle = Intent.GetStringExtra("TITLE");

                if (!isCapture)
                {
                    SetUI();
                }
                else if (data.title == "Update Payment Details" || TempTitle == "Update Payment Details")
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "updatePaymentDetailsTitle"));
                    TempTitle = "Update Payment Details";
                }
                else
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "enterPaymentDetailsTitle"));
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
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "setAppointmentTitle"));
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
                        if (TempTitle == "Reschedule Appointment")
                        {
                            isRescheduleappointment = true;
                        }
                        else
                        {
                            isRescheduleappointment = false;
                        }                       
                        Intent setAppointment = new Intent(this, typeof(AppointmentSetActivity));
                        setAppointment.PutExtra("Sernumbr", data.srNumber);
                        setAppointment.PutExtra("ApptDate", data.appointmentDate);
                        setAppointment.PutExtra("TechName", data.technicianName);
                        setAppointment.PutExtra("IncdAdd", data.incidentAddress);
                        //setAppointment.PutExtra("isRescheduleappointment", isRescheduleappointment);                      
                        StartActivity(setAppointment);
                    }

                }
                else if (!string.IsNullOrEmpty(data.crStatus))
                {
                    FcmPushNotificationFlagFromBackground = LaunchViewActivity.FcmPushNotificationFlagFromBackground;                   
                    if (data.crStatus != "NULL" && !string.IsNullOrEmpty(data.crStatusCode))// && !string.IsNullOrEmpty(data.crStatusCode)
                    {
                        SelectSubmittedFeedbackActivity.status = data.crStatus;//"Cancelled";
                        SelectSubmittedFeedbackActivity.crStatusCode =data.crStatusCode;
                    }
                    if (FcmPushNotificationFlagFromBackground == true )
                    {                        
                        Intent intent = new Intent(this, typeof(DashboardHomeActivity));
                        intent.SetFlags(ActivityFlags.ClearTop);
                        StartActivity(intent);
                        LaunchViewActivity.FcmPushNotificationFlagFromBackground = false;                     
                    }                   
                    else
                    {
                        base.OnBackPressed();
                    }
                    //NavigationController.SetNavigationBarHidden(false, false);                  
                }
                else if (data.title == "Compensation Agreement")
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "compensationAgreementTitle"));
                    TempTitle = "Compensation Agreement";

                }
                else if (data.title == "Negotiation Request")
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "negotiationRequestTitle"));
                    TempTitle = "Negotiation Request";
                }
                else if (data.title == "Enter Payment Details")
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "enterPaymentDetailsTitle"));
                    TempTitle = "Enter Payment Details";
                    //TempStepperTitle = "Enter Payment Details";
                }
                else if (data.title == "Update Payment Details")
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "updatePaymentDetailsTitle"));
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
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimTitle"));
                    TempTitle = "Overvoltage Claim";
                }
                else if (data.currentScreen == "1" && data.nextScreen == "2" && data.title == "Update Payment Details")
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "updatePaymentDetailsTitle"));
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
                else if (data.currentScreen == "0" && data.nextScreen == "overvoltageclaim")
                {
                    txtstep1of2.Visibility = ViewStates.Visible;
                    txtstep1of2.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle2of2");
                }
                else if (data.title == "Rate Your Experience")
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "rateYourExperienceTitle"));
                    TempTitle = "Rate Your Experience";

                }
                else if (data.title == "overvoltageclaim")
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimTitle"));
                    TempTitle = "Overvoltage Claim";
                }
                else if (!string.IsNullOrEmpty(data.claimId))
                {
                    Intent FeedBackSubmittedSuccess = new Intent(this, typeof(FeedBackSubmittedSuccessActivity));
                    FeedBackSubmittedSuccess.PutExtra("ClaimId", data.claimId);
                    StartActivity(FeedBackSubmittedSuccess);
                }
                else if (data.title == "Ex-gratia Agreement")
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "exGratiaAgreementTitle"));
                    TempTitle = "Ex-gratia Agreement";
                }
                else if (data.title == "reschedule-appointment")
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "rescheduleAppointmentTitle"));
                    TempTitle = "Reschedule Appointment";
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }            
        }

        public override void OnBackPressed()
        {
            try
            {
                if (TempTitle == "Set Appointment" || TempTitle == "Reschedule Appointment")
                {
                    webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#OnBackAppointment').trigger('click'); },500); })();", null);
                }
                else if (TempTitle == "Compensation Agreement")
                {
                    webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#OnBackCompensation').trigger('click'); },500); })();", null);
                }
                else if (TempTitle == "Ex-gratia Agreement")
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
                    OverVoltageClaimSuccessPageActivity.FormOverVoltageFeedbackDetailActivity = true;
                    webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#getCurrentStatusForMobileApp').trigger('click'); },500); })();", null);
                   
                }
                else if (data == null)
                {
                    base.OnBackPressed();
                }
                else if (data != null)
                {                   
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
        public override void OnReceivedTitle(WebView view, string title)
        {
            base.OnReceivedTitle(view, title);
            if (title == "Webpage not available")
            {
                _OverVoltageFeedbackDetailActivity.showpopup();
            }
        }
        public override void OnProgressChanged(WebView view, int newProgress)
        {
            base.OnProgressChanged(view, newProgress);

            if (newProgress == 100)
            {
                _OverVoltageFeedbackDetailActivity.IsStopTime = true;
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
}