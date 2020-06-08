using Foundation;
using System;
using UIKit;
using myTNB.Model;
using System.Collections.Generic;
using CoreGraphics;
using System.Threading.Tasks;
using System.Diagnostics;
using myTNB.Payment;

using WebKit;

namespace myTNB
{
    public partial class MakePaymentViewController : CustomUIViewController
    {
        public GetPaymentTransactionIdResponseModel _paymentTransactionIDResponseModel;
        public CardModel _card;
        public WKWebView _webView;
        public UIView _barView;
        string _url = string.Empty;
        public bool _saveCardIsChecked = false;
        public bool _isNewCard = false;
        public int _platform = 2;
        public string _paymentMode = "CC";
        public string _cardCVV = string.Empty;

        public MakePaymentViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = PaymentConstants.Pagename_MakePayment;
            base.ViewDidLoad();
            AppDelegate appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            if (appDelegate != null)
            {
                appDelegate._makePaymentVC = this;
            }

            NavigationItem.Title = GetI18NValue(_paymentMode == "CC" ? PaymentConstants.I18N_EnterOTP : PaymentConstants.I18N_OnlineBanking);

            AddBackButton();
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        await GetPaymentURL().ContinueWith(task =>
                        {
                            InvokeOnMainThread(SetSubviews);
                        });
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        internal void SetSubviews()
        {
            if (!string.IsNullOrEmpty(_url))
            {

                var preferences = new WKPreferences();
                preferences.JavaScriptEnabled = true;
                var configuration = new WKWebViewConfiguration();
                configuration.Preferences = preferences;

                _webView = new WKWebView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height), configuration);
                _webView.BackgroundColor = UIColor.White;
                _webView.NavigationDelegate = new WebWKNavigationDelegate(View, this);
                _webView.UIDelegate = new WebWKUIDelegate(View, this);

                try
                {
                    _webView.LoadRequest(new NSUrlRequest(new Uri(_url)));
                }
                catch (MonoTouchException m) { Debug.WriteLine("Error: " + m.Message); }
                catch (Exception e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                }

                View.AddSubview(_webView);

                nfloat statusBarHeight = UIApplication.SharedApplication.StatusBarFrame.Size.Height;
                _barView = new UIView
                {
                    Frame = new CGRect(0, 0, View.Frame.Width, statusBarHeight),
                    BackgroundColor = MyTNBColor.GradientPurpleDarkElement,
                    Hidden = true
                };
                View.AddSubview(_barView);
            }
        }

        internal void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                UIAlertController okCancelAlertController = UIAlertController.Create(GetI18NValue(PaymentConstants.I18N_AbortTitle)
                    , GetI18NValue(PaymentConstants.I18N_AbortMessage)
                    , UIAlertControllerStyle.Alert);
                okCancelAlertController.AddAction(UIAlertAction.Create(GetCommonI18NValue(Constants.Common_Yes)
                    , UIAlertActionStyle.Cancel, alert => NavigationController?.PopViewController(true)));
                okCancelAlertController.AddAction(UIAlertAction.Create(GetCommonI18NValue(Constants.Common_No)
                    , UIAlertActionStyle.Default, alert => Debug.WriteLine("Cancel was clicked")));
                okCancelAlertController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(okCancelAlertController, animated: true, completionHandler: null);
            });
            if (NavigationItem != null)
            {
                NavigationItem.LeftBarButtonItem = btnBack;
            }
        }

        /// <summary>
        /// Gets the payment URL.
        /// </summary>
        /// <returns>The payment URL.</returns>
        internal Task GetPaymentURL()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                if (_paymentMode == "FPX")
                {
                    string param3 = _paymentTransactionIDResponseModel?.d?.data?.payAccounts == null ? "0" : "1";
                    Dictionary<string, string> requestParams = new Dictionary<string, string>{
                    {"Param1", "3"},
                    {"Param2", _paymentTransactionIDResponseModel?.d?.data?.payMerchant_transID},
                    {"Param3", param3} ,
                    {"lang", TNBGlobal.APP_LANGUAGE}};
                    _url = serviceManager.GetPaymentURL(requestParams, _paymentTransactionIDResponseModel?.d?.data?.action);
                }
                else
                {
                    Dictionary<string, string> requestParams = new Dictionary<string, string>{
                    {"MERCHANTID", _paymentTransactionIDResponseModel?.d?.data?.payMerchantID},
                    {"MERCHANT_TRANID", _paymentTransactionIDResponseModel?.d?.data?.payMerchant_transID},
                    {"PAYMENT_METHOD", _paymentTransactionIDResponseModel?.d?.data?.payMethod},
                    {"CURRENCYCODE", _paymentTransactionIDResponseModel?.d?.data?.payCurrencyCode},
                    {"AMOUNT", _paymentTransactionIDResponseModel?.d?.data?.payAmount},
                    {"CUSTNAME", _paymentTransactionIDResponseModel?.d?.data?.payCustName},
                    {"CUSTEMAIL", _paymentTransactionIDResponseModel?.d?.data?.payCustEmail},
                    {"RETURN_URL", _paymentTransactionIDResponseModel?.d?.data?.payReturnUrl},
                    {"SIGNATURE", _paymentTransactionIDResponseModel?.d?.data?.paySign},
                    {"MPARAM1", _paymentTransactionIDResponseModel?.d?.data?.payMParam},
                    {"DESCRIPTION", _paymentTransactionIDResponseModel?.d?.data?.payProdDesc},
                    {"TRANSACTIONTYPE", "1"},
                    {"lang", TNBGlobal.APP_LANGUAGE}};
                    if (_isNewCard)
                    {
                        requestParams.Add("CARDNO", _card?.CardNo);
                        requestParams.Add("CARDNAME", _card?.CardName);
                        requestParams.Add("CARDTYPE", _card?.CardType);
                        requestParams.Add("EXPIRYMONTH", _card?.ExpiryMonth);
                        requestParams.Add("EXPIRYYEAR", _card?.ExpiryYear);
                        requestParams.Add("CARDCVC", _card?.CardCVV);
                        if (_saveCardIsChecked)
                        {
                            requestParams.Add("PYMT_IND", "tokenization");
                            requestParams.Add("PYMT_CRITERIA", "registration");
                        }
                    }
                    else
                    {
                        requestParams.Add("PYMT_IND", "tokenization");
                        requestParams.Add("PYMT_CRITERIA", "payment");
                        requestParams.Add("CARDCVC", _cardCVV);
                        requestParams.Add("PYMT_TOKEN", _paymentTransactionIDResponseModel?.d?.data?.tokenizedHashCodeCC);
                    }
                    _url = serviceManager.GetPaymentURL(requestParams, _paymentTransactionIDResponseModel?.d?.data?.action);
                }
            });
        }

        /// <summary>
        /// Sets the status bar hidden for full screen.
        /// </summary>
        /// <param name="isHidden">If set to <c>true</c> is hidden.</param>
        public void SetStatusBarHiddenForFullScreen(bool isHidden)
        {
            var statusBarHeight = _barView.Frame.Height;

            if (!DeviceHelper.IsIphoneXUpResolution())
            {
                _webView.Frame = new CGRect(0, statusBarHeight * -1
                    , UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height + statusBarHeight);
            }
            else
            {
                _webView.Frame = new CGRect(0, statusBarHeight * -1
                    , UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height + statusBarHeight + 40);
            }
            _barView.Hidden = isHidden;
        }
    }
}