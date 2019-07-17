using Foundation;
using System;
using UIKit;

namespace myTNB
{
    public partial class GenericPageRootViewController : UIViewController
    {
        protected GenericPageRootViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        GenericPageViewRootController test;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            test = new OnboardingController(this);
            test.OnViewDidLoad();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            test.OnViewDidLayoutSubViews();
        }

        [Export("pageViewController:spineLocationForInterfaceOrientation:")]
        public UIPageViewControllerSpineLocation GetSpineLocation(UIPageViewController pageViewController, UIInterfaceOrientation orientation)
        {
            return test.GetSpineLocation(pageViewController, orientation);
        }
    }
}