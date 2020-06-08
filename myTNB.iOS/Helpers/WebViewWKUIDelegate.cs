using System;
using Foundation;
using ObjCRuntime;
using UIKit;
using WebKit;

namespace myTNB
{
    public class WebWKUIDelegate : WKUIDelegate
    {
        private UIView View;
        private UIViewController Controller;

        public WebWKUIDelegate(UIView view, UIViewController viewController)
        {
            View = view;
            Controller = viewController;
        }

        public override WKWebView CreateWebView(WKWebView webView, WKWebViewConfiguration configuration, WKNavigationAction navigationAction, WKWindowFeatures windowFeatures)
        {
            var url = navigationAction.Request.Url;
            if (url != null && navigationAction.TargetFrame == null)
            {
                webView.LoadRequest(navigationAction.Request);
            }

            return null;
        }

        [Export("webView:runJavaScriptAlertPanelWithMessage:initiatedByFrame:completionHandler:")]
        public void RunJavaScriptAlertPanel(WKWebView webView, string message, WKFrameInfo frame, Action completionHandler)
        {
            var okCancelAlertController = UIAlertController.Create(webView.Url?.Host, message, UIAlertControllerStyle.Alert);
            okCancelAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, alert => completionHandler()));
            //okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alert => completionHandler()));
            Controller.PresentViewController(okCancelAlertController, true, null);

        }

        [Export("webView:runJavaScriptConfirmPanelWithMessage:initiatedByFrame:completionHandler:")]
        public override void RunJavaScriptConfirmPanel(WKWebView webView, string message, WKFrameInfo frame, Action<bool> completionHandler)
        {
            var okCancelAlertController = UIAlertController.Create(webView.Url?.Host, message, UIAlertControllerStyle.Alert);
            okCancelAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, alert => completionHandler(true)));
            //okCancelAlertController.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, completionHandler(true)));
            Controller.PresentViewController(okCancelAlertController, true, null);

        }

    }
}