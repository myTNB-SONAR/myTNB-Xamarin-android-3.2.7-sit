using Foundation;
using System;
using UIKit;
using myTNB.Model;
using System.Collections.Generic;
using CoreGraphics;

namespace myTNB
{
    public partial class MakePaymentViewController : UIViewController
    {
        public RequestPayBillResponseModel _requestPayBillResponseModel;
        public CardModel _card;
        public UIWebView _webView;
        string _url = string.Empty;
        public bool _saveCardIsChecked = false;
        public bool _isNewCard = false;
        public int _platform = 2;
        public string _paymentMode = "CC";
        public bool isFromFPXPayment = false;
        public string _cardCVV = String.Empty;

        public MakePaymentViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            appDelegate._makePaymentVC = this;

            AddBackButton();
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {

                    if (!isFromFPXPayment)
                    {
                        if (NetworkUtility.isReachable)
                        {
                            GetPaymentURL();
                            if (_paymentMode == "CC")
                            {
                                SetSubviews();
                            }
                            else
                            {
                                DismissViewController(true, null);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No Network");
                            var alert = UIAlertController.Create("No Data Connection", "Please check your data connection and try again.", UIAlertControllerStyle.Alert);
                            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                            PresentViewController(alert, animated: true, completionHandler: null);
                        }
                    }
                    else
                    {
                        Console.WriteLine("From FPX");
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
            _webView = new UIWebView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));
            _webView.BackgroundColor = UIColor.White;
            _webView.Delegate = new WebViewDelegate(View, this);
            string nsURL = _url;
            _webView.LoadRequest(new NSUrlRequest(new Uri(_url)));
            View.AddSubview(_webView);
        }

        internal void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                NavigationController.PopViewController(true);
            });
            NavigationItem.LeftBarButtonItem = btnBack;

            var okCancelAlertController = UIAlertController.Create("Cancel Payment?", "Do you want to cancel ", UIAlertControllerStyle.Alert);

            //Add Actions
            okCancelAlertController.AddAction(UIAlertAction.Create("Okay", UIAlertActionStyle.Default, alert => Console.WriteLine("Okay was clicked")));
            okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));
        }

        internal void GetPaymentURL()
        {
            ServiceManager serviceManager = new ServiceManager();

            if (_isNewCard == true && _saveCardIsChecked == false && _paymentMode == "CC")
            {
                Dictionary<string, string> requestParams = new Dictionary<string, string>(){
                    { "MERCHANTID" , _requestPayBillResponseModel.d.data.payMerchantID},
                    { "MERCHANT_TRANID" , _requestPayBillResponseModel.d.data.payMerchant_transID},
                    { "PAYMENT_METHOD" , _requestPayBillResponseModel.d.data.payMethod},
                    { "CURRENCYCODE" , _requestPayBillResponseModel.d.data.payCurrencyCode},
                    { "AMOUNT" , _requestPayBillResponseModel.d.data.payAmount},
                    { "CUSTNAME" , _requestPayBillResponseModel.d.data.payCustName},
                    { "CUSTEMAIL" , _requestPayBillResponseModel.d.data.payCustEmail},
                    { "RETURN_URL" , _requestPayBillResponseModel.d.data.payReturnUrl},
                    { "SIGNATURE" , _requestPayBillResponseModel.d.data.paySign},
                    { "MPARAM1" , _requestPayBillResponseModel.d.data.payMParam},
                    { "TRANSACTIONTYPE", "1"},
                    { "CARDNO" ,_card.CardNo},
                    { "CARDNAME" , _card.CardName},
                    { "CARDTYPE" , _card.CardType},
                    { "EXPIRYMONTH" , _card.ExpiryMonth},
                    { "EXPIRYYEAR" , _card.ExpiryYear},
                    { "CARDCVC" , _card.CardCVV},
                    { "DESCRIPTION" , _requestPayBillResponseModel.d.data.payProdDesc}
            };
                _url = serviceManager.GetPaymentURL(requestParams, _requestPayBillResponseModel.d.data.action);

            }
            else if (_isNewCard == true && _saveCardIsChecked == true && _paymentMode == "CC")
            {
                Dictionary<string, string> requestParams = new Dictionary<string, string>(){
                    { "MERCHANTID" , _requestPayBillResponseModel.d.data.payMerchantID},
                    { "MERCHANT_TRANID" , _requestPayBillResponseModel.d.data.payMerchant_transID},
                    { "PAYMENT_METHOD" , _requestPayBillResponseModel.d.data.payMethod},
                    { "CURRENCYCODE" , _requestPayBillResponseModel.d.data.payCurrencyCode},
                    { "AMOUNT" , _requestPayBillResponseModel.d.data.payAmount},
                    { "CUSTNAME" , _requestPayBillResponseModel.d.data.payCustName},
                    { "CUSTEMAIL" , _requestPayBillResponseModel.d.data.payCustEmail},
                    { "DESCRIPTION" , _requestPayBillResponseModel.d.data.payProdDesc},
                    { "RETURN_URL" , _requestPayBillResponseModel.d.data.payReturnUrl},
                    { "SIGNATURE" , _requestPayBillResponseModel.d.data.paySign},
                    { "MPARAM1" , _requestPayBillResponseModel.d.data.payMParam},
                    { "TRANSACTIONTYPE", "1"},
                    { "CARDNO" ,_card.CardNo},
                    { "CARDNAME" , _card.CardName},
                    { "CARDTYPE" , _card.CardType},
                    { "EXPIRYMONTH" , _card.ExpiryMonth},
                    { "EXPIRYYEAR" , _card.ExpiryYear},
                    { "CARDCVC" , _card.CardCVV},
                    { "PYMT_IND" , "tokenization"},
                    { "PYMT_CRITERIA" , "registration"}
            };
                _url = serviceManager.GetPaymentURL(requestParams, _requestPayBillResponseModel.d.data.action);
            }
            else if (_isNewCard == false && _paymentMode == "CC")
            {
                Dictionary<string, string> requestParams = new Dictionary<string, string>(){
                    { "MERCHANTID" , _requestPayBillResponseModel.d.data.payMerchantID},
                    { "MERCHANT_TRANID" , _requestPayBillResponseModel.d.data.payMerchant_transID},
                    { "PAYMENT_METHOD" , _requestPayBillResponseModel.d.data.payMethod},
                    { "CURRENCYCODE" , _requestPayBillResponseModel.d.data.payCurrencyCode},
                    { "AMOUNT" , _requestPayBillResponseModel.d.data.payAmount},
                    { "CUSTNAME" , _requestPayBillResponseModel.d.data.payCustName},
                    { "CUSTEMAIL" , _requestPayBillResponseModel.d.data.payCustEmail},
                    { "DESCRIPTION" , _requestPayBillResponseModel.d.data.payProdDesc},
                    { "RETURN_URL" , _requestPayBillResponseModel.d.data.payReturnUrl},
                    { "SIGNATURE" , _requestPayBillResponseModel.d.data.paySign},
                    { "MPARAM1" , _requestPayBillResponseModel.d.data.payMParam},
                    { "TRANSACTIONTYPE", "1"},
                    { "PYMT_IND" , "tokenization"},
                    { "PYMT_CRITERIA" , "payment"},
                    //{ "CARDNO" ,_card.CardNo},
                    //{ "CARDNAME" , _card.CardName},
                    //{ "CARDTYPE" , _card.CardType},
                    //{ "EXPIRYMONTH" , _card.ExpiryMonth},
                    //{ "EXPIRYYEAR" , _card.ExpiryYear},
                    { "CARDCVC" , _cardCVV},
                    { "PYMT_TOKEN", _requestPayBillResponseModel.d.data.tokenizedHashCodeCC}

            };
                _url = serviceManager.GetPaymentURL(requestParams, _requestPayBillResponseModel.d.data.action);
            }
            else if (_paymentMode == "FPX")
            {
                string param3 = _requestPayBillResponseModel.d.data.payAccounts == null
                                                            ? "0" : "1";
                Dictionary<string, string> requestParams = new Dictionary<string, string>(){
                    { "Param1" , "3"},
                    {"Param2", _requestPayBillResponseModel.d.data.payMerchant_transID},
                    {"Param3", param3}
                };
                var tempURL = TNBGlobal.GetPaymentURL();
                _url = serviceManager.GetPaymentURL(requestParams, tempURL);
                UIApplication.SharedApplication.OpenUrl(new NSUrl(_url));
            }
        }
    }
}