using System;
using System.Text.RegularExpressions;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public class WebViewDelegate : UIWebViewDelegate
    {
        LoadingOverlay loadingOverlay;
        UIView View;
        public WebViewDelegate(UIView view) => View = view;
        UIViewController Controller;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:myTNB.WebViewDelegate"/> class.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="controller">Controller.</param>
        public WebViewDelegate(UIView view, UIViewController controller)
        {
            View = view;
            Controller = controller;
        }

        public override bool ShouldStartLoad(UIWebView webView
                                             , NSUrlRequest request
                                             , UIWebViewNavigationType navigationType)
        {
            if (request.ToString().Contains("rating"))
            {
                string rateString = request.ToString().Split('=')[1];
                int rating = int.Parse(rateString);
                UIStoryboard storyBoard = UIStoryboard.FromName("Rating", null);
                RatingViewController viewController =
                    storyBoard.InstantiateViewController("RatingViewController") as RatingViewController;
                viewController.Rating = rating;
                var navController = new UINavigationController(viewController);
                Controller.PresentViewController(navController, true, null);
                loadingOverlay?.Hide();
            }
            if (request.ToString().Contains("intent://intent/#Intent;")
               && Controller != null)
            {
                Controller.DismissViewController(true, null);
                loadingOverlay?.Hide();
            }

            if (request.ToString().Contains("mytnbapp://action=dashboard")
                && Controller != null)
            {
                loadingOverlay?.Hide();
                DataManager.DataManager.SharedInstance.IsPaymentDone = true;
                Controller.DismissViewController(true, null);
            }

            if (request.ToString().Contains("mytnbapp://action=recipt&transid")
                && Controller != null)
            {
                string absoluteURL = request.ToString();
                Regex regex = new Regex("\\btransid.*\\b");
                Match match = regex.Match(absoluteURL);
                if (match.Success)
                {
                    string transID = match.Value.Replace("transid=", "");
                    UIStoryboard storyBoard = UIStoryboard.FromName("Receipt", null);
                    ReceiptViewController viewController =
                        storyBoard.InstantiateViewController("ReceiptViewController") as ReceiptViewController;
                    viewController.MerchatTransactionID = transID;
                    viewController.isCCFlow = true;
                    var navController = new UINavigationController(viewController);
                    Controller.NavigationController.PushViewController(viewController, true);
                }
                loadingOverlay?.Hide();
            }

            if (request.ToString().Contains("mytnbapp://action=payoptions")
                && Controller != null)
            {
                loadingOverlay?.Hide();

                var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;

                foreach (var vc in Controller.NavigationController.ViewControllers)
                {
                    if (vc is SelectPaymentMethodViewController)
                    {
                        if (Controller.NavigationController != null)
                        {
                            if (Controller.NavigationController.NavigationBarHidden == true)
                                Controller.NavigationController.NavigationBarHidden = false;
                        }
                        Controller.NavigationController.PopToViewController(vc, false);
                    }
                }
            }

            return true;
        }

        public override void LoadStarted(UIWebView webView)
        {
            Console.WriteLine("Started....");

            var webUrl = webView.Request.Url;

            if (loadingOverlay == null)
            {
                loadingOverlay = new LoadingOverlay(View.Bounds);
            }

            if (!loadingOverlay.IsDescendantOfView(View))
            {
                View.AddSubview(loadingOverlay);
            }
        }

        public override void LoadingFinished(UIWebView webView)
        {
            var webUrl = webView.Request.Url;

            if (webUrl.ToString().Contains("/paystatusreceipt.aspx?")
                || webUrl.ToString().Contains("/payMultiStatusReceipt.aspx?"))
            {
                var makePaymentVC = Controller as MakePaymentViewController;
                makePaymentVC.NavigationController.NavigationBarHidden = true;
                makePaymentVC._webView.ScrollView.ScrollEnabled = false;
                if (!DeviceHelper.IsIphoneX())
                    makePaymentVC._webView.Frame = new CGRect(0, -20
                                                              , UIScreen.MainScreen.Bounds.Width
                                                              , UIScreen.MainScreen.Bounds.Height + 20);
                else
                    makePaymentVC._webView.Frame = new CGRect(0, -44
                                                              , UIScreen.MainScreen.Bounds.Width
                                                              , UIScreen.MainScreen.Bounds.Height + 84);
            }

            loadingOverlay?.Hide();
        }
    }
}