using Android.App;
using Android.Content;
using Android.Net.Http;
using Android.Net.Wifi;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using DynatraceAndroid;
using Google.Android.Material.Snackbar;
using Java.IO;
using Java.Lang;
using Java.Net;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Rating.Activity;
using myTNB_Android.Src.Rating.Model;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.ViewReceipt.Activity;
using Newtonsoft.Json;
using System;
using System.Web;

namespace myTNB_Android.Src.MultipleAccountPayment.Fragment
{
    public class MPPaymentWebViewFragment : AndroidX.Fragment.App.Fragment
    {
        private string PYMT_IND = "tokenization";
        private string PYMT_CRITERIA_REGISTRATION = "registration";
        private string PYMT_CRITERIA_PAYMENT = "payment";
        private string apiKeyID;

        private string action;
        private string merchantId;
        private string merchantTransId;
        private string currencyCode;
        private string payAm;

        private string custEmail;
        private string custName;
        private string des;
        private string returnURL;
        private string signature;
        private string mparam1;
        private string payMethod;
        private string transType;
        private string accNum;

        private string custPhone;

        private string cardNo;
        private string cardExpM;
        private string cardExpY;
        private string cardCvv;
        private string cardType;
        private bool saveCard;
        private bool isRegisteredCard;

        string tokenizedHashCodeCC;

        private static WebView mWebView;
        private ProgressBar mProgressBar;
        private static FrameLayout mainView;

        private PaymentActivity paymentActivity;

        private static Snackbar mErrorMessageSnackBar;

        private SummaryDashBordRequest summaryDashBoardRequest = null;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            CookieSyncManager.CreateInstance(Activity);
            Android.Webkit.CookieManager.Instance.SetAcceptCookie(true);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View rootView = inflater.Inflate(Resource.Layout.PaymentWebView, container, false);
            try
            {
                mainView = rootView.FindViewById<FrameLayout>(Resource.Id.rootView);
                var metrics = Resources.DisplayMetrics;
                var widthInDp = ConvertPixelsToDp(metrics.WidthPixels);
                int heightInDp = metrics.HeightPixels;
                summaryDashBoardRequest = JsonConvert.DeserializeObject<SummaryDashBordRequest>(Arguments.GetString("SummaryDashBoardRequest"));
                paymentActivity = ((PaymentActivity)Activity);
                if (Arguments.ContainsKey("html_fpx"))
                {
                    paymentActivity.SetToolBarTitle(Utility.GetLocalizedLabel("MakePayment", "onlineBanking"));
                }
                else
                {
                    paymentActivity.SetToolBarTitle(Utility.GetLocalizedLabel("MakePayment", "enterOTP"));
                }

                mWebView = rootView.FindViewById<WebView>(Resource.Id.web_view);
                mProgressBar = rootView.FindViewById<ProgressBar>(Resource.Id.progressBar);
                mProgressBar.Visibility = ViewStates.Gone;

                //mWebView.LayoutParameters.Height = heightInDp + 200;
                mWebView.SetWebChromeClient(new WebChromeClient());
                mWebView.Settings.SetPluginState(WebSettings.PluginState.On);
                mWebView.SetWebViewClient(new MyTNBWebViewClient(paymentActivity, mProgressBar, summaryDashBoardRequest));
                mWebView.VerticalScrollBarEnabled = true;

                WebSettings settings = mWebView.Settings;
                settings.JavaScriptEnabled = true;
                settings.DomStorageEnabled = true;
                settings.LoadWithOverviewMode = true;
                settings.BuiltInZoomControls = false;

                System.Console.WriteLine("[DEBUG] TextZoom 1: " + settings.TextZoom);
                settings.TextZoom = TextViewUtils.IsLargeFonts ? 130 : 100;
                System.Console.WriteLine("[DEBUG] TextZoom 2: " + settings.TextZoom);

                if (Arguments.ContainsKey("html_fpx"))
                {
                    mWebView.LoadUrl(Arguments.GetString("html_fpx"));
                }
                else
                {
                    action = Arguments.GetString("action");
                    merchantId = Arguments.GetString("merchantId");//"MYTNB_V2_UAT";
                    merchantTransId = Arguments.GetString("merchantTransId");
                    currencyCode = Arguments.GetString("currencyCode");
                    payAm = Arguments.GetString("payAm");

                    custEmail = Arguments.GetString("custEmail");
                    custName = Arguments.GetString("custName");
                    des = Arguments.GetString("des");
                    returnURL = Arguments.GetString("returnURL");
                    signature = Arguments.GetString("signature");

                    mparam1 = Arguments.GetString("mparam1");
                    transType = Arguments.GetString("transType");
                    payMethod = Arguments.GetString("payMethod");
                    accNum = Arguments.GetString("accNum");
                    custPhone = Arguments.GetString("custPhone");

                    transType = Arguments.GetString("transType");
                    tokenizedHashCodeCC = Arguments.GetString("tokenizedHashCodeCC");

                    isRegisteredCard = Arguments.GetBoolean("registeredCard", false);
                    cardCvv = Arguments.GetString("cardCvv"); // -- CVV Enabled --
                    if (!isRegisteredCard)
                    {
                        cardNo = Arguments.GetString("cardNo");
                        cardExpM = Arguments.GetString("cardExpM");
                        cardExpY = "20" + Arguments.GetString("cardExpY");
                        cardCvv = Arguments.GetString("cardCvv");
                        cardType = Arguments.GetString("cardType");
                        saveCard = Arguments.GetBoolean("saveCard", false);
                    }

                    string data = "";

                    if (isRegisteredCard)
                    {
                        data = "PAYMENT_METHOD=" + URLEncoder.Encode(payMethod, "UTF-8")
                                    + "&TRANSACTIONTYPE=" + URLEncoder.Encode(transType, "UTF-8")
                                    + "&MERCHANTID=" + URLEncoder.Encode(merchantId, "UTF-8")
                                    + "&MERCHANT_TRANID=" + URLEncoder.Encode(merchantTransId, "UTF-8")
                                    + "&PYMT_IND=" + URLEncoder.Encode(PYMT_IND, "UTF-8")
                                    + "&PYMT_CRITERIA=" + URLEncoder.Encode(PYMT_CRITERIA_PAYMENT, "UTF-8")
                                    + "&CURRENCYCODE=" + URLEncoder.Encode(currencyCode, "UTF-8")
                                    + "&AMOUNT=" + URLEncoder.Encode(payAm, "UTF-8")
                                    + "&MPARAM1=" + URLEncoder.Encode(mparam1, "UTF-8")
                                    + "&SIGNATURE=" + URLEncoder.Encode(signature, "UTF-8")
                                    + "&CUSTNAME=" + URLEncoder.Encode(custName, "UTF-8")
                                    + "&CUSTEMAIL=" + URLEncoder.Encode(custEmail, "UTF-8")
                                    + "&CUSTPHONE=" + URLEncoder.Encode(custPhone, "UTF-8")
                                    + "&PYMT_TOKEN=" + URLEncoder.Encode(tokenizedHashCodeCC, "UTF-8")
                                    //+ "&SHOPPER_IP=" + URLEncoder.Encode(GetDeviceIPAddress())
                                    + "&DESCRIPTION=" + URLEncoder.Encode(des, "UTF-8")
                                    //+ "&RESPONSE_TYPE=" + URLEncoder.Encode("1") // 1 – Return response via browser redirection, using HTTP GET method
                                    + "&CARDCVC=" + URLEncoder.Encode(cardCvv, "UTF-8") // -- CVV Enabled --
                                    + "&RETURN_URL=" + URLEncoder.Encode(returnURL, "UTF-8")
                                    + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper();
                    }
                    else
                    {
                        string paymentCriteria = saveCard ? PYMT_CRITERIA_REGISTRATION : PYMT_CRITERIA_PAYMENT;
                        if (saveCard)
                        {
                            data = "PAYMENT_METHOD=" + URLEncoder.Encode(payMethod, "UTF-8")
                                        + "&TRANSACTIONTYPE=" + URLEncoder.Encode(transType, "UTF-8")
                                        + "&MERCHANTID=" + URLEncoder.Encode(merchantId, "UTF-8")
                                        + "&MERCHANT_TRANID=" + URLEncoder.Encode(merchantTransId, "UTF-8")
                                        + "&CURRENCYCODE=" + URLEncoder.Encode(currencyCode, "UTF-8")
                                        + "&AMOUNT=" + URLEncoder.Encode(payAm, "UTF-8")
                                        + "&MPARAM1=" + URLEncoder.Encode(mparam1, "UTF-8")
                                        + "&SIGNATURE=" + URLEncoder.Encode(signature, "UTF-8")
                                        + "&CUSTNAME=" + URLEncoder.Encode(custName, "UTF-8")
                                        + "&CUSTEMAIL=" + URLEncoder.Encode(custEmail, "UTF-8")
                                        + "&CUSTPHONE=" + URLEncoder.Encode(custPhone, "UTF-8")
                                        + "&PYMT_IND=" + URLEncoder.Encode(PYMT_IND, "UTF-8")
                                        + "&PYMT_CRITERIA=" + URLEncoder.Encode(paymentCriteria, "UTF-8")
                                        //+ "&SHOPPER_IP=" + URLEncoder.Encode(GetDeviceIPAddress())
                                        + "&DESCRIPTION=" + URLEncoder.Encode(des, "UTF-8")
                                        // + "&RESPONSE_TYPE=" + URLEncoder.Encode("1") // 1 – Return response via browser redirection, using HTTP GET method
                                        + "&RETURN_URL=" + URLEncoder.Encode(returnURL, "UTF-8")
                                        + "&CARDNO=" + URLEncoder.Encode(cardNo, "UTF-8")
                                        + "&CARDNAME=" + URLEncoder.Encode(custName, "UTF-8")
                                        + "&CARDTYPE=" + URLEncoder.Encode(cardType, "UTF-8")
                                        + "&EXPIRYMONTH=" + URLEncoder.Encode(cardExpM, "UTF-8")
                                        + "&EXPIRYYEAR=" + URLEncoder.Encode(cardExpY, "UTF-8")
                                        + "&CARDCVC=" + URLEncoder.Encode(cardCvv, "UTF-8")
                                        + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper();
                        }
                        else
                        {
                            data = "PAYMENT_METHOD=" + URLEncoder.Encode(payMethod, "UTF-8")
                                       + "&TRANSACTIONTYPE=" + URLEncoder.Encode(transType, "UTF-8")
                                       + "&MERCHANTID=" + URLEncoder.Encode(merchantId, "UTF-8")
                                       + "&MERCHANT_TRANID=" + URLEncoder.Encode(merchantTransId, "UTF-8")
                                       + "&CURRENCYCODE=" + URLEncoder.Encode(currencyCode, "UTF-8")
                                       + "&AMOUNT=" + URLEncoder.Encode(payAm, "UTF-8")
                                       + "&MPARAM1=" + URLEncoder.Encode(mparam1, "UTF-8")
                                       + "&SIGNATURE=" + URLEncoder.Encode(signature, "UTF-8")
                                       + "&CUSTNAME=" + URLEncoder.Encode(custName, "UTF-8")
                                       + "&CUSTEMAIL=" + URLEncoder.Encode(custEmail, "UTF-8")
                                       + "&CUSTPHONE=" + URLEncoder.Encode(custPhone, "UTF-8")
                                       //+ "&SHOPPER_IP=" + URLEncoder.Encode(GetDeviceIPAddress())
                                       + "&DESCRIPTION=" + URLEncoder.Encode(des, "UTF-8")
                                       // + "&RESPONSE_TYPE=" + URLEncoder.Encode("1") // 1 – Return response via browser redirection, using HTTP GET method
                                       + "&RETURN_URL=" + URLEncoder.Encode(returnURL, "UTF-8")
                                       + "&CARDNO=" + URLEncoder.Encode(cardNo, "UTF-8")
                                       + "&CARDNAME=" + URLEncoder.Encode(custName, "UTF-8")
                                       + "&CARDTYPE=" + URLEncoder.Encode(cardType, "UTF-8")
                                       + "&EXPIRYMONTH=" + URLEncoder.Encode(cardExpM, "UTF-8")
                                       + "&EXPIRYYEAR=" + URLEncoder.Encode(cardExpY, "UTF-8")
                                       + "&CARDCVC=" + URLEncoder.Encode(cardCvv, "UTF-8")
                                       + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper();
                        }
                    }
                    mWebView.PostUrl(action, GetBytes(data, "base64"));
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            try
            {
                Android.Content.Res.Configuration configuration = Resources.Configuration;
                configuration.FontScale = 1.3F;
                DisplayMetrics metrics = Resources.DisplayMetrics;
                metrics.ScaledDensity = configuration.FontScale * metrics.Density;
                Resources.UpdateConfiguration(configuration, metrics);
            }
            catch (Java.Lang.Exception javaEx)
            {
                System.Console.WriteLine("[DEBUG] configuration.DensityDpi Java Exception: " + javaEx.Message);
            }
            catch (System.Exception sysEx)
            {
                System.Console.WriteLine("[DEBUG] configuration.DensityDpi System Exception: " + sysEx.Message);
            }
            return rootView;
        }

        public override void OnResume()
        {
            if (Arguments.ContainsKey("html_fpx"))
            {
                paymentActivity.SetToolBarTitle(Utility.GetLocalizedLabel("MakePayment", "onlineBanking"));
            }
            else
            {
                paymentActivity.SetToolBarTitle(Utility.GetLocalizedLabel("MakePayment", "enterOTP"));
            }
            base.OnResume();
        }

        public class MyTNBWebViewClient : WebViewClient
        {

            public PaymentActivity mActivity;
            public ProgressBar progressBar;
            private bool isRedirected = false;
            private SummaryDashBordRequest summaryDashBoardRequest = null;


            public MyTNBWebViewClient(PaymentActivity mActivity, ProgressBar progress, SummaryDashBordRequest summaryDashBoardRequest)
            {
                this.mActivity = mActivity;
                this.progressBar = progress;
                this.summaryDashBoardRequest = summaryDashBoardRequest;
            }

            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                if (!ConnectionUtils.HasInternetConnection(mActivity))
                {
                    view.StopLoading();
                    return false;
                }
                else
                {
                    if (url.ToLower().Contains("mytnbapp://receipt"))// && !isRedirected)
                    {
                        isRedirected = true;
                        string merchantTransId = url.Substring(url.LastIndexOf("=") + 1);//GetQueryString(url, "transid");
                        Intent payment_activity = new Intent(mActivity, typeof(ViewReceiptMultiAccountNewDesignActivty));
                        payment_activity.PutExtra("SELECTED_ACCOUNT_NUMBER", "");
                        payment_activity.PutExtra("DETAILED_INFO_NUMBER", merchantTransId);
                        payment_activity.PutExtra("IS_OWNED_ACCOUNT", true);
                        payment_activity.PutExtra("IS_SHOW_ALL_RECEIPT", true);
                        mActivity.StartActivity(payment_activity);
                        //((PaymentActivity)this.mActivity).SetResult(Result.Ok);
                        //((PaymentActivity)this.mActivity).Finish();
                    }
                    else if (url.ToLower().Contains("mytnbapp://payoptions") && !isRedirected)
                    {
                        isRedirected = true;
                        ((PaymentActivity)this.mActivity).ClearBackStack();
                        ((PaymentActivity)this.mActivity).OnLoadMainFragment();
                        ((PaymentActivity)this.mActivity).ShowToolBar();

                    }
                    else if (url.ToLower().Contains("mytnbapp://payment/"))
                    {
                        try
                        {
                            IDTXAction WEBVIEW_PAYMENT_FINISH_DASHBOARD = DynatraceAndroid.Dynatrace.EnterAction(Constants.WEBVIEW_PAYMENT_FINISH_DASHBOARD);  // DYNA
                            WEBVIEW_PAYMENT_FINISH_DASHBOARD.LeaveAction();
                        }
                        catch (System.Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                        isRedirected = true;
                        /* This call inject JavaScript into the page which just finished loading. */
                        //((PaymentActivity)this.mActivity).SetResult(Result.Ok);
                        //((PaymentActivity)this.mActivity).Finish();
                        Intent DashboardIntent = new Intent(mActivity, typeof(DashboardHomeActivity));
                        MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                        HomeMenuUtils.ResetAll();
                        DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                        mActivity.StartActivity(DashboardIntent);
                        //view.loadUrl("javascript:window.HTMLOUT.processHTML('<html>'+document.getElementsByTagName('html')[0].innerHTML+'</html>');");
                    }
                    else if (url.ToLower().Contains("mytnbapp://transid"))// && !isRedirected)
                    {
                        //isRedirected = true;
                        /* This call inject JavaScript into the page which just finished loading. */
                        int ratings = int.Parse(url.Substring(url.LastIndexOf("=") + 1));//GetQueryString(url, "transid");
                        int lastIndexOfMerchantID = (url.IndexOf("&") - 1) - url.IndexOf("=");
                        string merchantTransId = url.Substring(url.IndexOf("=") + 1, lastIndexOfMerchantID);
                        Intent payment_activity = new Intent(mActivity, typeof(RatingActivity));
                        payment_activity.PutExtra(Constants.QUESTION_ID_CATEGORY, ((int)QuestionCategoryID.Payment));
                        payment_activity.PutExtra(Constants.SELECTED_RATING, ratings);
                        payment_activity.PutExtra(Constants.MERCHANT_TRANS_ID, merchantTransId);
                        mActivity.StartActivity(payment_activity);
                        //((PaymentActivity)this.mActivity).SetResult(Result.Ok);
                        //((PaymentActivity)this.mActivity).Finish();
                    }
                    else if (url.Contains("mytnbapp://action=setAppointment"))
                    {
                        isRedirected = true;
                        mActivity.OnSetAppointment();
                    }
                    else if (url.Contains("mytnbapp://action=startDigitalBilling"))
                    {
                        isRedirected = true;
                        mActivity.OnManageBillDelivery();
                    }
                    else
                    {
                        view.LoadUrl(url);
                    }
                    return true;
                }
            }

            public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
            {
                try
                {
                    if (ConnectionUtils.HasInternetConnection(mActivity))
                    {
                        base.OnPageStarted(view, url, favicon);
                        progressBar.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        ShowErrorMessage(url);
                    }

                    IDTXAction DynAction = DynatraceAndroid.Dynatrace.EnterAction(Constants.WEBVIEW_PAYMENT);  // DYNA
                    DynAction.LeaveAction();

                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public override void OnPageFinished(WebView view, string url)
            {
                try
                {
                    if (url.ToLower().Contains("statusreceipt.aspx") && url.ToLower().Contains("approved") || url.ToLower().Contains("paystatusreceipt"))
                    {
                        try
                        {
                            IDTXAction WEBVIEW_PAYMENT_SUCCESS = DynatraceAndroid.Dynatrace.EnterAction(Constants.WEBVIEW_PAYMENT_SUCCESS);  // DYNA
                            WEBVIEW_PAYMENT_SUCCESS.LeaveAction();
                        }
                        catch (System.Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }

                        ((PaymentActivity)mActivity).SetPaymentReceiptFlag(true, summaryDashBoardRequest);
                        //((PaymentActivity)mActivity).SetToolBarTitle("Success");
                        ((PaymentActivity)mActivity).HideToolBar();
                        progressBar.Visibility = ViewStates.Gone;
                    }
                    else if (url.ToLower().Contains("statusreceipt.aspx") || url.ToLower().Contains("paystatusreceipt") && url.ToLower().Contains("failed"))
                    {
                        try
                        {
                            IDTXAction WEBVIEW_PAYMENT_FAIL = DynatraceAndroid.Dynatrace.EnterAction(Constants.WEBVIEW_PAYMENT_FAIL);  // DYNA
                            WEBVIEW_PAYMENT_FAIL.LeaveAction();
                        }
                        catch (System.Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                        progressBar.Visibility = ViewStates.Gone;
                        ((PaymentActivity)mActivity).SetPaymentReceiptFlag(false, null);
                        //((PaymentActivity)mActivity).SetToolBarTitle("Unsuccessful");
                        ((PaymentActivity)mActivity).HideToolBar();
                    }
                    else if (url.ToLower().Contains("mytnbapp://payment/") && !isRedirected)
                    {
                        try
                        {
                            IDTXAction WEBVIEW_PAYMENT_FINISH_DASHBOARD = DynatraceAndroid.Dynatrace.EnterAction(Constants.WEBVIEW_PAYMENT_FINISH_DASHBOARD);  // DYNA
                            WEBVIEW_PAYMENT_FINISH_DASHBOARD.LeaveAction();
                        }
                        catch (System.Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                        isRedirected = true;
                        progressBar.Visibility = ViewStates.Gone;
                        MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                        HomeMenuUtils.ResetAll();
                        Intent DashboardIntent = new Intent(mActivity, typeof(DashboardHomeActivity));
                        DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                        mActivity.StartActivity(DashboardIntent);
                    }
                    else if (url.Contains("mytnbapp://action=setAppointment") && !isRedirected)
                    {
                        mActivity.OnSetAppointment();
                    }
                    else
                    {
                        progressBar.Visibility = ViewStates.Gone;
                    }
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public override bool OnRenderProcessGone(WebView view, RenderProcessGoneDetail detail)
            {
                return true;
            }

            public override void OnReceivedError(WebView view, ClientError errorCode, string description, string failingUrl)
            {
                try
                {
                    string message = "Please check your internet connection.";
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
                        ShowErrorMessage(failingUrl);
                    }
                    else
                    {
                        ShowErrorMessage(failingUrl);
                    }

                    //Toast.makeText(PaymentWebViewActivity.this,message,Toast.LENGTH_LONG).show();
                    if (!ConnectionUtils.HasInternetConnection(mActivity))
                    {
                        mWebView.StopLoading();
                    }
                    else
                    {
                        mWebView.LoadUrl("");
                    }
                }
                catch (System.Exception e)
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

        public string GetDeviceIPAddress()
        {
            WifiManager manager = (WifiManager)Activity.GetSystemService(Service.WifiService);
            int ip = manager.ConnectionInfo.IpAddress;

            string ipaddress = Android.Text.Format.Formatter.FormatIpAddress(ip);
            return ipaddress;
        }

        public static string GetQueryString(string url, string key)
        {
            var uri = new Uri(url.Replace("&", "?"));
            var newQueryString = HttpUtility.ParseQueryString(uri.Query);
            string query_string = newQueryString[key].ToString();
            return query_string;
        }

        private int ConvertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
            return dp;
        }

        public static void ShowErrorMessage(string failingUrl)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }

            mErrorMessageSnackBar = Snackbar.Make(mainView, Utility.GetLocalizedErrorLabel("noDataConnectionMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedLabel("Common", "tryAgain"), delegate
            {
                if (!failingUrl.ToLower().Contains("statusreceipt.aspx") || !failingUrl.ToLower().Contains("paystatusreceipt"))
                {
                    mWebView.LoadUrl(failingUrl);
                }
                mErrorMessageSnackBar.Dismiss();
            });
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
        }

        public byte[] GetBytes(string data, string charset)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new IllegalArgumentException("data may not be null");
            }

            if (string.IsNullOrEmpty(charset))
            {
                throw new IllegalArgumentException("charset may not be null or empty");
            }

            Java.Lang.String ToBeEncoded = new Java.Lang.String(data);

            try
            {
                return ToBeEncoded.GetBytes(charset);
            }
            catch (UnsupportedEncodingException e)
            {
                Utility.LoggingNonFatalError(e);
                return ToBeEncoded.GetBytes();
            }
        }
    }
}