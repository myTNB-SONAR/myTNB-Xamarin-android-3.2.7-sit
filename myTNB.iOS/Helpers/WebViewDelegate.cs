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
            if (request != null)
            {
                if (request.ToString().IsValid())
                {
                    if (request.ToString().Contains("rating"))
                    {
                        string rateString = default(string);
                        string transId = default(string);
                        var url = request.Url;
                        var paramsStr = url?.Host;

                        var parameters = paramsStr?.Split('&');
                        if (parameters != null)
                        {
                            if (parameters.Length > 0)
                            {
                                foreach (var pair in parameters)
                                {
                                    var item = pair?.Split('=');
                                    if (item != null)
                                    {
                                        if (item.Length == 2)
                                        {
                                            var key = item[0];
                                            if (key == "rating")
                                            {
                                                rateString = item[1];
                                            }
                                            else if (key == "transid")
                                            {
                                                transId = item[1];
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        UIStoryboard storyBoard = UIStoryboard.FromName("Rating", null);
                        if (storyBoard.InstantiateViewController("RatingViewController") is RatingViewController viewController)
                        {
                            //viewController.Rating = !string.IsNullOrEmpty(rateString) ? int.Parse(rateString) : 0;
                            viewController.Rating = 0;
                            viewController.TransId = transId;
                            UINavigationController navController = new UINavigationController(viewController);
                            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                            Controller?.PresentViewController(navController, true, null);
                        }
                        loadingOverlay?.Hide();
                    }
                    if (request.ToString().Contains("intent://intent/#Intent;")
                       && Controller != null)
                    {
                        Controller?.DismissViewController(true, null);
                        loadingOverlay?.Hide();
                    }

                    if (request.ToString().Contains("mytnbapp://action=dashboard")
                        && Controller != null)
                    {
                        loadingOverlay?.Hide();
                        DataManager.DataManager.SharedInstance.SummaryNeedsRefresh = true;
                        ViewHelper.DismissControllersAndSelectTab(Controller, 0, true, true);
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
                            viewController.DetailedInfoNumber = transID;
                            viewController.isCCFlow = true;
                            viewController.showAllReceipts = true;
                            var navController = new UINavigationController(viewController);
                            var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                            var topVc = AppDelegate.GetTopViewController(baseRootVc);
                            topVc?.NavigationController?.PushViewController(viewController, true);
                            //Controller.NavigationController.PushViewController(viewController, true);
                        }
                        loadingOverlay?.Hide();
                    }

                    if (request.ToString().Contains("mytnbapp://action=payoptions")
                        && Controller != null)
                    {
                        loadingOverlay?.Hide();

                        var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;

                        foreach (var vc in Controller?.NavigationController.ViewControllers)
                        {
                            if (vc is SelectPaymentMethodViewController)
                            {
                                if (Controller?.NavigationController != null)
                                {
                                    if (Controller?.NavigationController?.NavigationBarHidden == true)
                                    {
                                        Controller.NavigationController.NavigationBarHidden = false;
                                    }
                                }
                                Controller?.NavigationController?.PopToViewController(vc, false);
                                break;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public override void LoadStarted(UIWebView webView)
        {

            if (loadingOverlay == null)
            {
                loadingOverlay = new LoadingOverlay(View.Bounds);
            }

            if (!loadingOverlay.IsDescendantOfView(View))
            {
                foreach (UIView view in View.Subviews)
                {
                    if (view.Tag == TNBGlobal.Tags.LoadingOverlay)
                    {
                        view.RemoveFromSuperview();
                        break;
                    }
                }
                View.AddSubview(loadingOverlay);
            }
        }

        public override void LoadingFinished(UIWebView webView)
        {
            var webUrl = webView?.Request?.Url ?? default(NSUrl);

            if (webUrl != null)
            {
                if ((webUrl.ToString().Contains("/paystatusreceipt.aspx")
                     || webUrl.ToString().Contains("/payMultiStatusReceipt.aspx")) && !webUrl.ToString().Contains("RETURN_URL"))
                {
                    var makePaymentVC = Controller as MakePaymentViewController;
                    if (makePaymentVC != null)
                    {
                        makePaymentVC.NavigationController?.SetNavigationBarHidden(true, false);
                        makePaymentVC._webView.ScrollView.ScrollEnabled = true;
                        makePaymentVC._webView.ScrollView.Bounces = false;
                        makePaymentVC.SetStatusBarHiddenForFullScreen(false);
                    }
                }
            }

            loadingOverlay?.Hide();
        }
    }
}