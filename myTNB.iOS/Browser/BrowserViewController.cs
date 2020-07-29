using Foundation;
using UIKit;
using CoreGraphics;
using WebKit;

namespace myTNB
{
    public partial class BrowserViewController : CustomUIViewController
    {
        private WKWebView _webView;
        private UIBarButtonItem _btnShare;

        public string NavigationTitle { set; private get; } = string.Empty;
        public string URL { set; private get; } = string.Empty;
        public bool IsDelegateNeeded { set; private get; }
        public bool IsShareableContent { set; private get; }
        public string ShareID { set; private get; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.White;
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
            //Todo: Configure to accept all kinds of shareable content
            if (IsShareableContent)
            {
                _btnShare = new UIBarButtonItem(UIImage.FromBundle(WhatsNewConstants.Img_ShareIcon), UIBarButtonItemStyle.Done, (sender, e) =>
                {
                    NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                    {
                        if (NetworkUtility.isReachable && ShareID.IsValid())
                        {
                            ActivityIndicator.Show();
                            BaseService baseService = new BaseService();
                            APIEnvironment env = TNBGlobal.IsProduction ? APIEnvironment.PROD : APIEnvironment.SIT;
                            string linkUrl = baseService.GetDomain(env) + "/whatsnew/redirect.aspx/wnid=" + ShareID;

                            var deeplinkUrl = string.Empty;
                            var components = CommonServices.GenerateLongURL(linkUrl);
                            components.GetShortenUrl((shortUrl, warnings, error) =>
                            {
                                if (error == null)
                                {
                                    deeplinkUrl = shortUrl.AbsoluteString;
                                }
                                else
                                {
                                    deeplinkUrl = linkUrl;
                                }
                                ShareAction(deeplinkUrl);
                                ActivityIndicator.Hide();
                            });
                        }
                        else
                        {
                            AlertHandler.DisplayNoDataAlert(this);
                        }
                    });
                });
                NavigationItem.RightBarButtonItem = _btnShare;
            }
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

        private void ShareAction(string deeplinkUrl)
        {
            if (deeplinkUrl.IsValid())
            {
                NSObject item = NSObject.FromObject(deeplinkUrl);
                NSObject[] activityItems = { item };
                UIActivity[] applicationActivities = null;
                UIActivityViewController activityController = new UIActivityViewController(activityItems, applicationActivities);
                UIBarButtonItem.AppearanceWhenContainedIn(new[] { typeof(UINavigationBar) }).TintColor = UIColor.White;
                activityController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(activityController, true, null);
            }
        }
    }
}