using Foundation;
using UIKit;

namespace myTNB
{
    public class BasePageViewRootController
    {
        public UIViewController that;
        public BasePageViewRootController(UIViewController controller)
        {
            that = controller;
        }
        public ModelController ModelController { set; get; }

        public UIPageViewController PageViewController { set; get; }

        public virtual void OnViewDidLoad() { }

        public virtual void OnViewDidLayoutSubViews() { }

        [Export("pageViewController:spineLocationForInterfaceOrientation:")]
        public virtual UIPageViewControllerSpineLocation GetSpineLocation(UIPageViewController pageViewController, UIInterfaceOrientation orientation)
        {
            return new UIPageViewControllerSpineLocation();
        }
    }
}