
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using CheeseBind;
using Dynatrace.Xamarin;
using Dynatrace.Xamarin.Binding.Android;
using Microsoft.Maui.ApplicationModel;
using myTNB.Mobile;
using myTNB.AndroidApp.Src.AppLaunch.Activity;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.Helper;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.OverVoltageClaim.Model;
using myTNB.AndroidApp.Src.OverVoltageClaimSuccessPage.Activity;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using Org.Json;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Devices;


namespace myTNB.AndroidApp.Src.OverVoltageClaim.Activity
{
    [Activity(Label = "@string/submitICFeedback_OverVoltageClaimTitle"
          , ScreenOrientation = ScreenOrientation.Portrait
          , WindowSoftInputMode = SoftInput.AdjustResize
          , Theme = "@style/Theme.FaultyStreetLamps")]
   
    public class OvervoltageClaim : BaseToolbarAppCompatActivity 
    {
        public bool IsINZeroStepTab = true;
        public bool IsTermAndConditions = false;
        public bool IsInTermAndConsitionStepTab = false;
        public bool IsCountryDropDown = false, IsStopTime = false;
        [BindView(Resource.Id.webView)]
        WebView webView;

        [BindView(Resource.Id.txtstep1of2)]
        TextView txtstep1of2;
        private static System.Timers.Timer aTimer;
        MyTNBAppToolTipBuilder leaveDialog;
        private string accNo = null;
        private string CANickname="";
        public override int ResourceId()
        {
            return Resource.Layout.OvervoltageClaim;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Android.OS.Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.ACCOUNT_NUMBER))
                    {
                        accNo = extras.GetString(Constants.ACCOUNT_NUMBER);
                        if (UserEntity.IsCurrentlyActive())
                        {
                            CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(accNo);
                            if (customerBillingAccount != null)
                            {
                                CANickname = customerBillingAccount.AccDesc;
                            }
                        }
                        else
                        {
                            //handle if acc from outside show only CA number 
                        }
                    }
               
                }
                //Dyanatrace
                var myAction = Agent.Instance.EnterAction(Constants.TOUCH_ON_SUBMIT_OVERVOLTAGE_CLAIM);  // DYNA
                myAction.ReportValue("session_id", LaunchViewActivity.DynatraceSessionUUID);
                myAction.ReportValue("ca_number", accNo);
                myAction.LeaveAction();

                SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimTitle"));
                SetUI();
                Platform.Init(this, savedInstanceState);
                
                //Enabled Javascript in Websettings  

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            } 
        }

        public override void OnBackPressed()
        {           
            if(!IsINZeroStepTab)
            {
                webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#onBack').trigger('click'); },500); })();", null);
            }
            else if (IsTermAndConditions)
            {
                webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#onBackTermsAndConditions').trigger('click'); },500); })();", null);

            }
            else if (IsInTermAndConsitionStepTab)
            {
                webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#onBackTnbTerms').trigger('click'); },500); })();", null);
            }
            else if (IsCountryDropDown)
            {
                webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { $('#onBcakToClaim').trigger('click'); },500); })();", null);
                IsCountryDropDown = false;
            }
            else
            {
               leaveDialog = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
              .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "leaveThisPage"))
              .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "leaveThisPageDescription"))
              .SetCTALabel(Utility.GetLocalizedCommonLabel("cancel"))
              .SetSecondaryCTALabel(Utility.GetLocalizedCommonLabel("yes"))
              .SetSecondaryCTAaction(() => { base.OnBackPressed(); })
              .Build();
              leaveDialog.Show();
              
            }          
        }

        public async void SetUI()
        {
            try
            {
                TextViewUtils.SetMuseoSans300Typeface(txtstep1of2);
                TextViewUtils.SetTextSize12(txtstep1of2);
                var AppVersion = DeviceIdUtils.GetAppVersionName();              
                var OsVersion = "Android"+DeviceIdUtils.GetAndroidVersion();
                var DeviceModel = DeviceInfo.Model;
                var Manufacturer = DeviceInfo.Manufacturer;
                var data = new MyTNBService.Request.BaseRequest();
                var usin = data.usrInf;
                UserEntity user = UserEntity.GetActive();               
#if DEBUG
                //global::Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
#endif

                webView = FindViewById<WebView>(Resource.Id.webView);
                txtstep1of2 = FindViewById<TextView>(Resource.Id.txtstep1of2);
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

                string domain = MobileConstants.OvisWebviewBaseUrl + "/"; // WebView Live

                UrlUtility urlUtility = new UrlUtility();
                urlUtility.AddQueryParams("CA", accNo);
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
                urlUtility.AddQueryParams("CANickName", CANickname);
                
                if (TextViewUtils.IsLargeFonts)
                {
                    urlUtility.AddQueryParams("large", "true");
                }

                string url = urlUtility.EncodeURL(domain);

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

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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

        internal async Task ResponseFromWebviewAsync(string message)
            {
            try
            { 
                var data = JsonConvert.DeserializeObject<DTOWebView>(message); 
                if (data.currentScreen == "step0-screen" && data.nextScreen == "step1-screen")
                {
                    IsINZeroStepTab = false;
                    txtstep1of2.Visibility = ViewStates.Visible;
                    //txtstep1of2.Text = "Step 1 of 2";
                    txtstep1of2.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle1of2");

                }
                else if (data.currentScreen == "step1-screen" && data.nextScreen == "country-select")
                {
                    txtstep1of2.Visibility = ViewStates.Gone;
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "selectCountryTitle"));
                    IsCountryDropDown = true;
                    IsINZeroStepTab = true;
                }
                else if (data.currentScreen == "country-select" && data.nextScreen == "step1-screen")
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimTitle"));
                    IsCountryDropDown = false;
                    IsINZeroStepTab = false;
                    txtstep1of2.Visibility = ViewStates.Visible;
                }

                else if (data.currentScreen == "step1-screen" && data.nextScreen == "step0-screen")
                {
                    IsINZeroStepTab = true;
                    txtstep1of2.Visibility = ViewStates.Gone;
                    //txtstep1of2.Text = "Step 1 of 2";
                    txtstep1of2.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle1of2");
                }
                else if (data.currentScreen == "step1-screen" && data.nextScreen == "step2-screen")
                {
                    IsINZeroStepTab = false;
                    txtstep1of2.Visibility = ViewStates.Visible;
                    //txtstep1of2.Text = "Step 2 of 2";
                    txtstep1of2.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle2of2");

                    try
                    {
                        var location = await Geolocation.GetLastKnownLocationAsync();

                        if (location != null)
                        {
                            Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                            webView.EvaluateJavascript("javascript:(function() { setTimeout(function() { setLocation(" + location.Latitude + "," + location.Longitude + ") },100); })();", null);
                        }
                    }
                    catch (FeatureNotSupportedException fnsEx)
                    {
                        // Handle not supported on device exception
                    }
                    catch (FeatureNotEnabledException fneEx)
                    {
                        // Handle not enabled on device exception
                    }
                    catch (PermissionException pEx)
                    {
                        // Handle permission exception
                    }
                    catch (Exception ex)
                    {
                        // Unable to get location
                    }
                }
                else if (data.currentScreen == "step2-screen" && data.nextScreen == "step1-screen")
                {
                    IsINZeroStepTab = false;
                    txtstep1of2.Visibility = ViewStates.Visible;
                    //txtstep1of2.Text = "Step 1 of 2";
                    txtstep1of2.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle1of2");
                }
                else if (data.currentScreen == "step2-screen" && data.nextScreen == "submit-screen")
                    {
                    ShowProgressDialog();
                    IsINZeroStepTab = false;
                    txtstep1of2.Visibility = ViewStates.Visible;
                    //txtstep1of2.Text = "Step 2 of 2";
                    txtstep1of2.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle2of2");
                    var serviceNumber = data.serviceNumber;
                    Intent OverVoltagClaim = new Intent(this, typeof(OverVoltageClaimSuccessPageActivity));
                    OverVoltagClaim.PutExtra("SerialNumber", serviceNumber.Trim());
                    OverVoltageClaimSuccessPageActivity.comeFromsubmitClaimPage = true;
                    StartActivity(OverVoltagClaim);
                    HideProgressDialog();
                }
                else if (data.currentScreen == "step2-screen" && data.nextScreen == "TermsAndConditions")
                {
                    // Go to terms and condition screen
                    try
                    {
                        IsTermAndConditions = true;
                        IsINZeroStepTab = true;
                        txtstep1of2.Visibility = ViewStates.Gone;
                        SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "termsAndConditionTitle"));
                    }
                    catch (Exception ex)
                    {

                    }

                }
                else if (data.currentScreen == "TermsAndConditions" && data.nextScreen == "step2-screen")
                {
                    //Termsandconditions back
                    try
                    {
                        SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimTitle"));
                        IsTermAndConditions = false;
                        IsINZeroStepTab = false;
                        txtstep1of2.Visibility = ViewStates.Visible;
                    }
                    catch (Exception ex)
                    {

                    }

                }
                else if (data.currentScreen == "TermsAndConditions" && data.nextScreen == "TnbTerms")
                {
                    //goto Termcondition main screen
                    try
                    {
                        SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "tnbTermUse"));
                        IsTermAndConditions = false;
                        IsInTermAndConsitionStepTab = true;
                    }
                    catch (Exception ex)
                    {

                    }

                }
                else if (data.currentScreen == "TermsAndConditions" && data.nextScreen == "privacy")
                {
                    //goto Privacy main screen
                    try
                    {
                        SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "privacyPolicyTitle"));
                        IsTermAndConditions = false;
                        IsInTermAndConsitionStepTab = true;
                    }
                    catch (Exception ex)
                    {

                    }

                }
                else if ((data.currentScreen == "TnbTerms" || data.currentScreen == "privacy") && data.nextScreen == "TermsAndConditions")
                {
                    //goto terms and condition screen
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "termsAndConditionTitle"));
                    IsTermAndConditions = true;
                    IsInTermAndConsitionStepTab = false;
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

    internal class WebViewClient :  WebChromeClient
    {
        OvervoltageClaim overVoltageClaim;
        private WebView webview;
        //Give the host application a chance to take over the control when a new URL is about to be loaded in the current WebView.  
         
        public  WebViewClient(OvervoltageClaim overVoltageClaim, WebView webview)
        {
            this.overVoltageClaim = overVoltageClaim;
            this.webview = webview;
           
        }

        public override bool OnJsAlert(WebView view, string url, string message, JsResult result)
            {
            //return base.OnJsAlert(view, url, message, result);
            result.Cancel();
            overVoltageClaim.ResponseFromWebviewAsync(message);

            return true;

        }
        public override void OnReceivedTitle(WebView view, string title)
        {
            base.OnReceivedTitle(view, title);
            if (title == "Webpage not available")
            {
                overVoltageClaim.showpopup();
            }
        }
        public override void OnProgressChanged(WebView view, int newProgress)
        {
            base.OnProgressChanged(view, newProgress);

            if (newProgress == 100)
            {
                overVoltageClaim.IsStopTime = true;
                this.overVoltageClaim.HideProgressDialog();               
            
            }           
        }
    }   
}
