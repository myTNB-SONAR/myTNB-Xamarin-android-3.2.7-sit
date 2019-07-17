using System;
using Foundation;
using UIKit;

namespace myTNB
{
    public class GenericPageViewRootController
    {
        public UIViewController that;
        public GenericPageViewRootController(UIViewController controller)
        {
            that = controller;
        }
        public ModelController ModelController
        {
            get; set;
        }

        public UIPageViewController PageViewController
        {
            get; set;
        }

        public virtual void OnViewDidLoad()
        {

        }

        public virtual void OnViewDidLayoutSubViews()
        {

        }

        [Export("pageViewController:spineLocationForInterfaceOrientation:")]
        public virtual UIPageViewControllerSpineLocation GetSpineLocation(UIPageViewController pageViewController, UIInterfaceOrientation orientation)
        {
            return new UIPageViewControllerSpineLocation();
        }
    }
}
