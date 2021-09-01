
using System;
using System.Collections.Generic;
using System.Linq;
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
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.OverVoltageClaimSuccessPage.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Org.Json;
using Xamarin.Essentials;


namespace myTNB_Android.Src.OverVoltageClaim.Activity
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
        [BindView(Resource.Id.webView)]
        WebView webView;

        [BindView(Resource.Id.txtstep1of2)]
        TextView txtstep1of2;

        MyTNBAppToolTipBuilder leaveDialog;
        private string accNo = null;

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
                    }
               
                }
                SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimTitle"));
                SetUI();
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                
                //Enabled Javascript in Websettings  

            }
            catch (Exception ex)
            {

            } 
        }

        public override void OnBackPressed()
        {
            //bool SetOnBack = false;
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
            else
            {
               leaveDialog = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
              .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "leaveDialouge"))
              .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "leaveDialougeMessage"))
              .SetCTALabel(Utility.GetLocalizedCommonLabel("cancel"))
              .SetSecondaryCTALabel(Utility.GetLocalizedCommonLabel("yes"))
              .SetSecondaryCTAaction(() => { base.OnBackPressed(); })
              .Build();
              leaveDialog.Show();
              
            }
           // ;
        }

        public async void SetUI()
        {
            try
            {
                var AppVersion = DeviceIdUtils.GetAppVersionName();              
                var OsVersion = "Android"+DeviceIdUtils.GetAndroidVersion();
                var DeviceModel = DeviceInfo.Model;
                var Manufacturer = DeviceInfo.Manufacturer;
                var data = new BaseRequest();
                var usin = data.usrInf;
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

                string domain = "http://mytnbwvovis.ap.ngrok.io/?CA="+accNo+ "&eid=" + usin.eid + "&appVersion=" + AppVersion + "&os=" + OsVersion + "&Manufacturer=" + Manufacturer + "&model=" + DeviceModel;
                //string domain = "http://mytnbwvovis.ap.ngrok.io/?CA=" + accNo; // WebView Live

                //string domain = "http://192.168.1.157:3000/?CA="+accNo; // WebView Local
                // WebView Local
                //string domain = "http://192.168.1.157:3000/?CA="+accNo+ "&eid=" + usin.eid + "&appVersion=" + AppVersion + "&os=" + OsVersion + "&Manufacturer=" + Manufacturer + "&model=" + DeviceModel;

                //https://serene-rosalind-a35967.netlify.app/ //https://mytnbwvovis.ap.ngrok.io/  Live https://serene-rosalind-a35967.netlify.app/ //http://192.168.1.158:3000/ //https://mytnbwvovis.ap.ngrok.io/

                string url = domain;

                if (TextViewUtils.IsLargeFonts)
                {
                    url += "&large";
                }

                webView.LoadUrl(url); 
                await Task.Delay(0);
                //PassData();
                //HideProgressDialog();
            }
            catch (Exception ex)
            {

            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

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


                    //Intent OverVoltagClaim = new Intent(this, typeof(OverVoltageClaimSuccessPageActivity));
                    //OverVoltagClaim.PutExtra("SerialNumber", "ABC");
                    //StartActivity(OverVoltagClaim);
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
                        SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "tNC"));
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
                    SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "tNC"));
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

        internal void PassData()
        {
            try
            {
                var data = new BaseRequest();
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
        public override void OnProgressChanged(WebView view, int newProgress)
        {
            base.OnProgressChanged(view, newProgress);

            if (newProgress == 100)
            {
                this.overVoltageClaim.HideProgressDialog();
                overVoltageClaim.PassData();
            
            }
           
        }


    }

    public class DTOWebView
    {
        public string currentScreen { get; set; }
        public string nextScreen { get; set; }
        public string serviceNumber { get; set; }
    }
}
