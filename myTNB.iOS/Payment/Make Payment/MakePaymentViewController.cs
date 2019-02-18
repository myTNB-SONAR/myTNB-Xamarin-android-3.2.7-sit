using Foundation;
using System;
using UIKit;
using myTNB.Model;
using System.Collections.Generic;
using CoreGraphics;
using myTNB.Extensions;
using System.Threading.Tasks;

namespace myTNB
{
    public partial class MakePaymentViewController : UIViewController
    {
        public RequestPayBillResponseModel _requestPayBillResponseModel;
        public CardModel _card;
        public UIWebView _webView;
        public UIView _barView;
        string _url = string.Empty;
        public bool _saveCardIsChecked = false;
        public bool _isNewCard = false;
        public int _platform = 2;
        public string _paymentMode = "CC";
        public string _cardCVV = String.Empty;

        public MakePaymentViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            appDelegate._makePaymentVC = this;

            NavigationItem.Title = (_paymentMode == "CC") ? "PaymentNavTitleCC".Translate() : "PaymentNavTitleFPX".Translate();

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
                        Console.WriteLine("No Network");
                        var alert = UIAlertController.Create("ErrNoNetworkTitle".Translate(), "ErrNoNetworkMsg".Translate(), UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                        PresentViewController(alert, animated: true, completionHandler: null);
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
                _webView = new UIWebView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));
                _webView.BackgroundColor = UIColor.White;
                _webView.Delegate = new WebViewDelegate(View, this);

                try
                {
                    _webView.LoadRequest(new NSUrlRequest(new Uri(_url)));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }

                View.AddSubview(_webView);

                var statusBarHeight = UIApplication.SharedApplication.StatusBarFrame.Size.Height;
                _barView = new UIView
                {
                    Frame = new CGRect(0, 0, View.Frame.Width, statusBarHeight),
                    BackgroundColor = myTNBColor.GradientPurpleDarkElement(),
                    Hidden = true
                };
                View.AddSubview(_barView);
            }
        }

        internal void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                var okCancelAlertController = UIAlertController.Create("PaymentAlertTitle".Translate(), "PaymentAlertMsg".Translate(), UIAlertControllerStyle.Alert);
                okCancelAlertController.AddAction(UIAlertAction.Create("Abort".Translate(), UIAlertActionStyle.Default, alert => NavigationController?.PopViewController(true)));
                okCancelAlertController.AddAction(UIAlertAction.Create("Cancel".Translate(), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));
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

                if (_isNewCard == true && _saveCardIsChecked == false && _paymentMode == "CC")
                {
                    Dictionary<string, string> requestParams = new Dictionary<string, string>(){
                    { "MERCHANTID" , _requestPayBillResponseModel?.d?.data?.payMerchantID},
                    { "MERCHANT_TRANID" , _requestPayBillResponseModel?.d?.data?.payMerchant_transID},
                    { "PAYMENT_METHOD" , _requestPayBillResponseModel?.d?.data?.payMethod},
                    { "CURRENCYCODE" , _requestPayBillResponseModel?.d?.data?.payCurrencyCode},
                    { "AMOUNT" , _requestPayBillResponseModel?.d?.data?.payAmount},
                    { "CUSTNAME" , _requestPayBillResponseModel?.d?.data?.payCustName},
                    { "CUSTEMAIL" , _requestPayBillResponseModel?.d?.data?.payCustEmail},
                    { "RETURN_URL" , _requestPayBillResponseModel?.d?.data?.payReturnUrl},
                    { "SIGNATURE" , _requestPayBillResponseModel?.d?.data?.paySign},
                    { "MPARAM1" , _requestPayBillResponseModel?.d?.data?.payMParam},
                    { "TRANSACTIONTYPE", "1"},
                    { "CARDNO" ,_card?.CardNo},
                    { "CARDNAME" , _card?.CardName},
                    { "CARDTYPE" , _card?.CardType},
                    { "EXPIRYMONTH" , _card?.ExpiryMonth},
                    { "EXPIRYYEAR" , _card?.ExpiryYear},
                    { "CARDCVC" , _card?.CardCVV},
                    { "DESCRIPTION" , _requestPayBillResponseModel?.d?.data?.payProdDesc}
                    };
                    _url = serviceManager.GetPaymentURL(requestParams, _requestPayBillResponseModel?.d?.data?.action);

                }
                else if (_isNewCard == true && _saveCardIsChecked == true && _paymentMode == "CC")
                {
                    Dictionary<string, string> requestParams = new Dictionary<string, string>(){
                    { "MERCHANTID" , _requestPayBillResponseModel?.d?.data?.payMerchantID},
                    { "MERCHANT_TRANID" , _requestPayBillResponseModel?.d?.data?.payMerchant_transID},
                    { "PAYMENT_METHOD" , _requestPayBillResponseModel?.d?.data?.payMethod},
                    { "CURRENCYCODE" , _requestPayBillResponseModel?.d?.data?.payCurrencyCode},
                    { "AMOUNT" , _requestPayBillResponseModel?.d?.data?.payAmount},
                    { "CUSTNAME" , _requestPayBillResponseModel?.d?.data?.payCustName},
                    { "CUSTEMAIL" , _requestPayBillResponseModel?.d?.data?.payCustEmail},
                    { "DESCRIPTION" , _requestPayBillResponseModel?.d?.data?.payProdDesc},
                    { "RETURN_URL" , _requestPayBillResponseModel?.d?.data?.payReturnUrl},
                    { "SIGNATURE" , _requestPayBillResponseModel?.d?.data?.paySign},
                    { "MPARAM1" , _requestPayBillResponseModel?.d?.data?.payMParam},
                    { "TRANSACTIONTYPE", "1"},
                    { "CARDNO" ,_card?.CardNo},
                    { "CARDNAME" , _card?.CardName},
                    { "CARDTYPE" , _card?.CardType},
                    { "EXPIRYMONTH" , _card?.ExpiryMonth},
                    { "EXPIRYYEAR" , _card?.ExpiryYear},
                    { "CARDCVC" , _card?.CardCVV},
                    { "PYMT_IND" , "tokenization"},
                    { "PYMT_CRITERIA" , "registration"}
                    };
                    _url = serviceManager.GetPaymentURL(requestParams, _requestPayBillResponseModel?.d?.data?.action);
                }
                else if (_isNewCard == false && _paymentMode == "CC")
                {
                    Dictionary<string, string> requestParams = new Dictionary<string, string>(){
                    { "MERCHANTID" , _requestPayBillResponseModel?.d?.data?.payMerchantID},
                    { "MERCHANT_TRANID" , _requestPayBillResponseModel?.d?.data?.payMerchant_transID},
                    { "PAYMENT_METHOD" , _requestPayBillResponseModel?.d?.data?.payMethod},
                    { "CURRENCYCODE" , _requestPayBillResponseModel?.d?.data?.payCurrencyCode},
                    { "AMOUNT" , _requestPayBillResponseModel?.d?.data?.payAmount},
                    { "CUSTNAME" , _requestPayBillResponseModel?.d?.data?.payCustName},
                    { "CUSTEMAIL" , _requestPayBillResponseModel?.d?.data?.payCustEmail},
                    { "DESCRIPTION" , _requestPayBillResponseModel?.d?.data?.payProdDesc},
                    { "RETURN_URL" , _requestPayBillResponseModel?.d?.data?.payReturnUrl},
                    { "SIGNATURE" , _requestPayBillResponseModel?.d?.data?.paySign},
                    { "MPARAM1" , _requestPayBillResponseModel?.d?.data?.payMParam},
                    { "TRANSACTIONTYPE", "1"},
                    { "PYMT_IND" , "tokenization"},
                    { "PYMT_CRITERIA" , "payment"},
                    { "CARDCVC" , _cardCVV},
                    { "PYMT_TOKEN", _requestPayBillResponseModel?.d?.data?.tokenizedHashCodeCC}
                    };
                    _url = serviceManager.GetPaymentURL(requestParams, _requestPayBillResponseModel?.d?.data?.action);
                }
                else if (_paymentMode == "FPX")
                {
                    string param3 = _requestPayBillResponseModel?.d?.data?.payAccounts == null
                                                                ? "0" : "1";
                    Dictionary<string, string> requestParams = new Dictionary<string, string>(){
                    { "Param1" , "3"},
                    {"Param2", _requestPayBillResponseModel?.d?.data?.payMerchant_transID},
                    {"Param3", param3}
                    };
                    var tempURL = TNBGlobal.GetPaymentURL();
                    _url = serviceManager.GetPaymentURL(requestParams, tempURL);
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
                                            , UIScreen.MainScreen.Bounds.Width
                                            , UIScreen.MainScreen.Bounds.Height + statusBarHeight);
            }
            else
            {
                _webView.Frame = new CGRect(0, statusBarHeight * -1
                                            , UIScreen.MainScreen.Bounds.Width
                                            , UIScreen.MainScreen.Bounds.Height + statusBarHeight + 40);
            }

            _barView.Hidden = isHidden;
        }


    }
}