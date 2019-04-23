using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class BrowserViewController : UIViewController
    {
        public BrowserViewController(IntPtr handle) : base(handle)
        {
        }

        UIWebView _webView;

        public string NavigationTitle = string.Empty;
        public string URL = string.Empty;
        public bool IsDelegateNeeded = false;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetNavigationItems();
            SetSubviews();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        void SetNavigationItems()
        {
            NavigationItem.Title = NavigationTitle;
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle("Back-White")
                , UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        void SetSubviews()
        {
            _webView = new UIWebView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));
            if (IsDelegateNeeded)
            {
                _webView.Delegate = new WebViewDelegate(View);
            }
            _webView.LoadRequest(new NSUrlRequest(new NSUrl(URL)));
            View.AddSubview(_webView);
        }
    }
}