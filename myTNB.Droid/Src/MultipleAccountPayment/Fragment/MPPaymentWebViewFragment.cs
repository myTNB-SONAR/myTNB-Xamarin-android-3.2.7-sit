using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Webkit;
using Org.Apache.Http.Util;
using Java.Net;
using Android.Net.Wifi;
using myTNB_Android.Src.ViewReceipt.Activity;
using System.Web;
using Newtonsoft.Json;
using myTNB_Android.Src.PaymentSuccessExperienceRating.Activity;
using myTNB_Android.Src.Utils;
using Android.Support.Design.Widget;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using Android.Net.Http;
using myTNB_Android.Src.Rating.Activity;
using myTNB_Android.Src.Rating.Model;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Base.Api;

namespace myTNB_Android.Src.MultipleAccountPayment.Fragment
{
    public class MPPaymentWebViewFragment : Android.App.Fragment
    {
        private string TOOL_BAR_TITLE = "Enter OTP";
        private string TOOL_BAR_TITLE_FPX = "Online Banking";
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
            mainView = rootView.FindViewById<FrameLayout>(Resource.Id.rootView);
            var metrics = Resources.DisplayMetrics;
            var widthInDp = ConvertPixelsToDp(metrics.WidthPixels);
            int heightInDp = metrics.HeightPixels;
            summaryDashBoardRequest = JsonConvert.DeserializeObject<SummaryDashBordRequest>(Arguments.GetString("SummaryDashBoardRequest"));
            paymentActivity = ((PaymentActivity)Activity);
            if (Arguments.ContainsKey("html_fpx"))
            {
                paymentActivity.SetToolBarTitle(TOOL_BAR_TITLE_FPX);
            }
            else
            {
                paymentActivity.SetToolBarTitle(TOOL_BAR_TITLE);
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


                //string url = "https://pguat.tnb.com.my/payment/PaymentInterface.jsp";

                //String url = "https://pguat.tnb.com.my/payment/PaymentWindowResponsive.jsp";

                string data = "";

                if (isRegisteredCard)
                {
                    data = "PAYMENT_METHOD=" + URLEncoder.Encode(payMethod)
                                + "&TRANSACTIONTYPE=" + URLEncoder.Encode(transType)
                                + "&MERCHANTID=" + URLEncoder.Encode(merchantId)
                                + "&MERCHANT_TRANID=" + URLEncoder.Encode(merchantTransId)
                                + "&PYMT_IND=" + URLEncoder.Encode(PYMT_IND)
                                + "&PYMT_CRITERIA=" + URLEncoder.Encode(PYMT_CRITERIA_PAYMENT)
                                + "&CURRENCYCODE=" + URLEncoder.Encode(currencyCode)
                                + "&AMOUNT=" + URLEncoder.Encode(payAm)
                                + "&MPARAM1=" + URLEncoder.Encode(mparam1)
                                + "&SIGNATURE=" + URLEncoder.Encode(signature)
                                + "&CUSTNAME=" + URLEncoder.Encode(custName)
                                + "&CUSTEMAIL=" + URLEncoder.Encode(custEmail)
                                + "&CUSTPHONE=" + URLEncoder.Encode(custPhone)
                                + "&PYMT_TOKEN=" + URLEncoder.Encode(tokenizedHashCodeCC)
                                //+ "&SHOPPER_IP=" + URLEncoder.Encode(GetDeviceIPAddress())
                                + "&DESCRIPTION=" + URLEncoder.Encode(des)
                                //+ "&RESPONSE_TYPE=" + URLEncoder.Encode("1") // 1 – Return response via browser redirection, using HTTP GET method
                                + "&CARDCVC=" + URLEncoder.Encode(cardCvv) // -- CVV Enabled --
                                + "&RETURN_URL=" + URLEncoder.Encode(returnURL);
                }
                else
                {
                    string paymentCriteria = saveCard ? PYMT_CRITERIA_REGISTRATION : PYMT_CRITERIA_PAYMENT;
                    if (saveCard)
                    {
                        data = "PAYMENT_METHOD=" + URLEncoder.Encode(payMethod)
                                    + "&TRANSACTIONTYPE=" + URLEncoder.Encode(transType)
                                    + "&MERCHANTID=" + URLEncoder.Encode(merchantId)
                                    + "&MERCHANT_TRANID=" + URLEncoder.Encode(merchantTransId)
                                    + "&CURRENCYCODE=" + URLEncoder.Encode(currencyCode)
                                    + "&AMOUNT=" + URLEncoder.Encode(payAm)
                                    + "&MPARAM1=" + URLEncoder.Encode(mparam1)
                                    + "&SIGNATURE=" + URLEncoder.Encode(signature)
                                    + "&CUSTNAME=" + URLEncoder.Encode(custName)
                                    + "&CUSTEMAIL=" + URLEncoder.Encode(custEmail)
                                    + "&CUSTPHONE=" + URLEncoder.Encode(custPhone)
                                    + "&PYMT_IND=" + URLEncoder.Encode(PYMT_IND)
                                    + "&PYMT_CRITERIA=" + URLEncoder.Encode(paymentCriteria)
                                    //+ "&SHOPPER_IP=" + URLEncoder.Encode(GetDeviceIPAddress())
                                    + "&DESCRIPTION=" + URLEncoder.Encode(des)
                                    // + "&RESPONSE_TYPE=" + URLEncoder.Encode("1") // 1 – Return response via browser redirection, using HTTP GET method
                                    + "&RETURN_URL=" + URLEncoder.Encode(returnURL)
                                    + "&CARDNO=" + URLEncoder.Encode(cardNo)
                                    + "&CARDNAME=" + URLEncoder.Encode(custName)
                                    + "&CARDTYPE=" + URLEncoder.Encode(cardType)
                                    + "&EXPIRYMONTH=" + URLEncoder.Encode(cardExpM)
                                    + "&EXPIRYYEAR=" + URLEncoder.Encode(cardExpY)
                                    + "&CARDCVC=" + URLEncoder.Encode(cardCvv);
                    }
                    else
                    {
                        data = "PAYMENT_METHOD=" + URLEncoder.Encode(payMethod)
                                   + "&TRANSACTIONTYPE=" + URLEncoder.Encode(transType)
                                   + "&MERCHANTID=" + URLEncoder.Encode(merchantId)
                                   + "&MERCHANT_TRANID=" + URLEncoder.Encode(merchantTransId)
                                   + "&CURRENCYCODE=" + URLEncoder.Encode(currencyCode)
                                   + "&AMOUNT=" + URLEncoder.Encode(payAm)
                                   + "&MPARAM1=" + URLEncoder.Encode(mparam1)
                                   + "&SIGNATURE=" + URLEncoder.Encode(signature)
                                   + "&CUSTNAME=" + URLEncoder.Encode(custName)
                                   + "&CUSTEMAIL=" + URLEncoder.Encode(custEmail)
                                   + "&CUSTPHONE=" + URLEncoder.Encode(custPhone)
                                   //+ "&SHOPPER_IP=" + URLEncoder.Encode(GetDeviceIPAddress())
                                   + "&DESCRIPTION=" + URLEncoder.Encode(des)
                                   // + "&RESPONSE_TYPE=" + URLEncoder.Encode("1") // 1 – Return response via browser redirection, using HTTP GET method
                                   + "&RETURN_URL=" + URLEncoder.Encode(returnURL)
                                   + "&CARDNO=" + URLEncoder.Encode(cardNo)
                                   + "&CARDNAME=" + URLEncoder.Encode(custName)
                                   + "&CARDTYPE=" + URLEncoder.Encode(cardType)
                                   + "&EXPIRYMONTH=" + URLEncoder.Encode(cardExpM)
                                   + "&EXPIRYYEAR=" + URLEncoder.Encode(cardExpY)
                                   + "&CARDCVC=" + URLEncoder.Encode(cardCvv);
                    }
                }
                mWebView.PostUrl(action, EncodingUtils.GetBytes(data, "base64"));
            }
            
            return rootView;
            
        }

        public override void OnResume()
        {
            if (Arguments.ContainsKey("html_fpx"))
            {
                paymentActivity.SetToolBarTitle(TOOL_BAR_TITLE_FPX);
            }
            else
            {
                paymentActivity.SetToolBarTitle(TOOL_BAR_TITLE);
            }
            base.OnResume();
        }

       


        public class MyTNBWebViewClient : WebViewClient
        {

            public Android.App.Activity mActivity;
            public ProgressBar progressBar;
            private bool isRedirected = false;
            private SummaryDashBordRequest summaryDashBoardRequest = null;

            public MyTNBWebViewClient(Android.App.Activity mActivity, ProgressBar progress, SummaryDashBordRequest summaryDashBoardRequest)
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
                        payment_activity.PutExtra("merchantTransId", merchantTransId);
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
                    else if (url.ToLower().Contains("mytnbapp://payment/") && !isRedirected)
                    {
                        isRedirected = true;
                        /* This call inject JavaScript into the page which just finished loading. */
                        //((PaymentActivity)this.mActivity).SetResult(Result.Ok);
                        //((PaymentActivity)this.mActivity).Finish();
                        Intent DashboardIntent = new Intent(mActivity, typeof(DashboardActivity));
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
                    else
                    {
                        view.LoadUrl(url);
                    }
                    return true;
                }
                
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
                    ShowErrorMessage(url);
                }
            }

            public override void OnPageFinished(WebView view, string url)
            {
                
                if (url.ToLower().Contains("statusreceipt.aspx") && url.ToLower().Contains("approved") || url.ToLower().Contains("paystatusreceipt"))
                {
                     
                    ((PaymentActivity)mActivity).SetPaymentReceiptFlag(true, summaryDashBoardRequest);
                    //((PaymentActivity)mActivity).SetToolBarTitle("Success");
                    ((PaymentActivity)mActivity).HideToolBar();
                    progressBar.Visibility = ViewStates.Gone;
                }
                else if (url.ToLower().Contains("statusreceipt.aspx") || url.ToLower().Contains("paystatusreceipt") && url.ToLower().Contains("failed"))
                {
                    progressBar.Visibility = ViewStates.Gone;
                    ((PaymentActivity)mActivity).SetPaymentReceiptFlag(false, null);
                    //((PaymentActivity)mActivity).SetToolBarTitle("Unsuccessful");
                    ((PaymentActivity)mActivity).HideToolBar();

                }else if (url.ToLower().Contains("mytnbapp://payment/"))
                {
                    progressBar.Visibility = ViewStates.Gone;
                    Intent DashboardIntent = new Intent(mActivity, typeof(DashboardActivity));
                    DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    mActivity.StartActivity(DashboardIntent);
                } else {
                    progressBar.Visibility = ViewStates.Gone;
                }

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
            WifiManager manager = (WifiManager) Activity.GetSystemService(Service.WifiService);
            int ip = manager.ConnectionInfo.IpAddress;

            string ipaddress = Android.Text.Format.Formatter.FormatIpAddress(ip);
            return ipaddress;
        }

        public static string GetQueryString(string url, string key)
        {
            string query_string = string.Empty;

            var uri = new Uri(url.Replace("&","?"));
            var newQueryString = HttpUtility.ParseQueryString(uri.Query);
            query_string = newQueryString[key].ToString();

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

            mErrorMessageSnackBar = Snackbar.Make(mainView, "Please check your internet connection.", Snackbar.LengthIndefinite)
            .SetAction("Try Again", delegate {
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
    }
}