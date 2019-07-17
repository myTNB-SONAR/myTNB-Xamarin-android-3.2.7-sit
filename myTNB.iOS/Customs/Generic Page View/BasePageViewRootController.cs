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
        public UIPageViewControllerSpineLocation GetSpineLocation(UIPageViewController pageViewController, UIInterfaceOrientation orientation)
        {
            UIViewController currentViewController;
            UIViewController[] viewControllers;

            if (orientation == UIInterfaceOrientation.Portrait || orientation == UIInterfaceOrientation.PortraitUpsideDown || UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                // In portrait orientation or on iPhone: Set the spine position to "min" and the page view controller's view controllers array to contain just one view controller.
                // Setting the spine position to 'UIPageViewControllerSpineLocation.Mid' in landscape orientation sets the doubleSided property to true, so set it to false here.
                currentViewController = pageViewController.ViewControllers[0];
                viewControllers = new[] { currentViewController };
                pageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, true, null);

                pageViewController.DoubleSided = false;

                return UIPageViewControllerSpineLocation.Min;
            }

            // In landscape orientation: Set set the spine location to "mid" and the page view controller's view controllers array to contain two view controllers.
            // If the current page is even, set it to contain the current and next view controllers; if it is odd, set the array to contain the previous and current view controllers.
            currentViewController = pageViewController.ViewControllers[0];

            int index = ModelController.IndexOf((GenericPageDataViewController)currentViewController);
            if (index == 0 || index % 2 == 0)
            {
                var nextViewController = ModelController.GetNextViewController(pageViewController, currentViewController);
                viewControllers = new[] { currentViewController, nextViewController };
            }
            else
            {
                var previousViewController = ModelController.GetPreviousViewController(pageViewController, currentViewController);
                viewControllers = new[] { previousViewController, currentViewController };
            }

            pageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, true, null);
            return UIPageViewControllerSpineLocation.Mid;
        }
    }
}