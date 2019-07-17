using Foundation;
using System;
using UIKit;

namespace myTNB
{
    public partial class GenericPageRootViewController : UIViewController
    {
        protected GenericPageRootViewController(IntPtr handle) : base(handle) { }

        private BasePageViewRootController _pageViewController;

        public GenericPageViewEnum.Type PageType;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (PageType == GenericPageViewEnum.Type.Onboarding)
            {
                _pageViewController = new OnboardingController(this);
            }
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