using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;

using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Net;
using myTNB_Android.Src.MakePayment.Activity;
using myTNB_Android.Src.PaymentSuccessExperienceRating.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.ViewReceipt.Activity;
using Org.Apache.Http.Util;
using System;
using System.Web;

namespace myTNB_Android.Src.MakePayment.Fragment
{
    public class PaymentWebViewFragment : AndroidX.Fragment.App.Fragment 
    {
        private string TOOL_BAR_TITLE = "Enter OTP";
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

        private MakePaymentActivity makePaymentActivity;

        private static Snackbar mErrorMessageSnackBar;

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

                makePaymentActivity = ((MakePaymentActivity)Activity);
                makePaymentActivity.SetToolBarTitle(TOOL_BAR_TITLE);
                mWebView = rootView.FindViewById<WebView>(Resource.Id.web_view);
                mProgressBar = rootView.FindViewById<ProgressBar>(Resource.Id.progressBar);
                mProgressBar.Visibility = ViewStates.Gone;

                mWebView.LayoutParameters.Height = heightInDp - 85;
                mWebView.SetWebChromeClient(new WebChromeClient());
                mWebView.Settings.SetPluginState(WebSettings.PluginState.On);
                mWebView.SetWebViewClient(new MyTNBWebViewClient(makePaymentActivity, mProgressBar));

                WebSettings settings = mWebView.Settings;
                settings.JavaScriptEnabled = true;
                settings.DomStorageEnabled = true;
                settings.LoadWithOverviewMode = true;
                settings.BuiltInZoomControls = true;

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
                cardCvv = Arguments.GetString("cardCvv");
                if (!isRegisteredCard)
                {
                    cardNo = Arguments.GetString("cardNo");
                    cardExpM = Arguments.GetString("cardExpM");
                    cardExpY = "20" + Arguments.GetString("cardExpY");
                    //cardCvv = Arguments.GetString("cardCvv");
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
                                + "&CARDCVC=" + URLEncoder.Encode(cardCvv)
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
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return rootView;

        }

        public override void OnResume()
        {
            ((MakePaymentActivity)Activity).SetToolBarTitle(TOOL_BAR_TITLE);
            base.OnResume();
        }

        public class MyTNBWebViewClient : WebViewClient
        {

            public Android.App.Activity mActivity;
            public ProgressBar progressBar;
            private bool isRedirected = false;

            public MyTNBWebViewClient(Android.App.Activity mActivity, ProgressBar progress)
            {
                this.mActivity = mActivity;
                this.progressBar = progress;
            }

            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                if (ConnectionUtils.HasInternetConnection(mActivity))
                {
                    if (url.ToLower().Contains("mytnbapp://receipt") && !isRedirected)
                    {
                        isRedirected = true;
                        string merchantTransId = url.Substring(url.LastIndexOf("=") + 1);//GetQueryString(url, "transid");
                        Intent payment_activity = new Intent(mActivity, typeof(ViewReceiptActivity));
                        payment_activity.PutExtra("merchantTransId", merchantTransId);
                        mActivity.StartActivity(payment_activity);
                        ((MakePaymentActivity)this.mActivity).SetResult(Result.Ok);
                        ((MakePaymentActivity)this.mActivity).Finish();
                    }
                    else if (url.ToLower().Contains("mytnbapp://payoptions") && !isRedirected)
                    {
                        isRedirected = true;
                        ((MakePaymentActivity)this.mActivity).ClearBackStack();
                        ((MakePaymentActivity)this.mActivity).OnLoadMainFragment();

                    }
                    else if (url.ToLower().Contains("mytnbapp://payment/") && !isRedirected)
                    {
                        isRedirected = true;
                        /* This call inject JavaScript into the page which just finished loading. */
                        ((MakePaymentActivity)this.mActivity).SetResult(Result.Ok);
                        ((MakePaymentActivity)this.mActivity).Finish();
                        //view.loadUrl("javascript:window.HTMLOUT.processHTML('<html>'+document.getElementsByTagName('html')[0].innerHTML+'</html>');");
                    }
                    else if (url.ToLower().Contains("mytnbapp://rating") && !isRedirected)
                    {
                        isRedirected = true;
                        /* This call inject JavaScript into the page which just finished loading. */
                        int ratings = int.Parse(url.Substring(url.LastIndexOf("=") + 1));//GetQueryString(url, "transid");
                        Intent payment_activity = new Intent(mActivity, typeof(PaymentSuccessExperienceRatingActivity));
                        payment_activity.PutExtra(Constants.SELECTED_RATING, ratings);
                        mActivity.StartActivity(payment_activity);
                        ((MakePaymentActivity)this.mActivity).SetResult(Result.Ok);
                        ((MakePaymentActivity)this.mActivity).Finish();
                    }
                    else
                    {
                        view.LoadUrl(url);
                    }
                }
                return true;
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
                progressBar.Visibility = ViewStates.Gone;
                if (url.ToLower().Contains("paystatusreceipt.aspx") && url.ToLower().Contains("approved"))
                {
                    MakePaymentActivity.SetPaymentReceiptFlag(true);
                    ((MakePaymentActivity)mActivity).SetToolBarTitle("Success");
                }
                else if (url.ToLower().Contains("paystatusreceipt.aspx") && url.ToLower().Contains("failed"))
                {
                    MakePaymentActivity.SetPaymentReceiptFlag(true);
                    ((MakePaymentActivity)mActivity).SetToolBarTitle("Unsuccessful");
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
                mWebView.LoadUrl("");
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
            string query_string = string.Empty;

            var uri = new Uri(url.Replace("&", "?"));
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

            mErrorMessageSnackBar = Snackbar.Make(mainView, Utility.GetLocalizedErrorLabel("noDataConnectionMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedLabel("Common", "tryAgain"), delegate
            {
                mWebView.LoadUrl(failingUrl);
                mErrorMessageSnackBar.Dismiss();
            });
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
        }
    }
}