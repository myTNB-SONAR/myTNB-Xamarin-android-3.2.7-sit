using Foundation;
using UIKit;
using CoreGraphics;
using WebKit;

namespace myTNB
{
    public partial class BrowserViewController : CustomUIViewController
    {
        private WKWebView _webView;

        public string NavigationTitle = string.Empty;
        public string URL = string.Empty;
        public bool IsDelegateNeeded;

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

        private void SetNavigationItems()
        {
            NavigationItem.Title = NavigationTitle.IsValid() ? NavigationTitle : string.Empty;
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(Constants.IMG_Back)
                , UIBarButtonItemStyle.Done, (sender, e) =>
                {
                    DismissViewController(true, null);
                });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        private void SetSubviews()
        {
            _webView = new WKWebView(new CGRect(0, 0, ViewWidth, ViewHeight), new WKWebViewConfiguration());
            if (IsDelegateNeeded)
            {
                _webView.NavigationDelegate = new WebWKNavigationDelegate(View);
            }
            _webView.LoadRequest(new NSUrlRequest(new NSUrl(URL)));
            View.AddSubview(_webView);
        }
    }
}