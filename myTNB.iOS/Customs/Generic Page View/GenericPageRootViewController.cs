using Foundation;
using System;
using UIKit;

namespace myTNB
{
    public partial class GenericPageRootViewController : UIViewController
    {
        protected GenericPageRootViewController(IntPtr handle) : base(handle) { }

        BasePageViewRootController _pageViewController;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _pageViewController = new OnboardingController(this);
            _pageViewController.OnViewDidLoad();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            _pageViewController.OnViewDidLayoutSubViews();
        }

        [Export("pageViewController:spineLocationForInterfaceOrientation:")]
        public UIPageViewControllerSpineLocation GetSpineLocation(UIPageViewController pageViewController, UIInterfaceOrientation orientation)
        {
            return _pageViewController.GetSpineLocation(pageViewController, orientation);
        }
    }
}