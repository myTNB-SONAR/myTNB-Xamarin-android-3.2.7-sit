using Foundation;
using System;
using UIKit;
using CoreGraphics;
using CoreAnimation;
using Cirrious.FluentLayouts.Touch;

namespace myTNB
{
    public partial class OnboardingRootViewController : UIViewController
    {
        public ModelController ModelController
        {
            get; private set;
        }

        public UIPageViewController PageViewController
        {
            get; private set;
        }

        protected OnboardingRootViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ModelController = new ModelController();
            ModelController.SetPageData();

            // Configure the page view controller and add it as a child view controller.
            PageViewController = new UIPageViewController(UIPageViewControllerTransitionStyle.Scroll, UIPageViewControllerNavigationOrientation.Horizontal, UIPageViewControllerSpineLocation.Min);
            PageViewController.WeakDelegate = this;
            var startingViewController = ModelController.GetViewController(0, Storyboard);
            var viewControllers = new UIViewController[] { startingViewController };
            PageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, false, null);

            PageViewController.WeakDataSource = ModelController;

            AddChildViewController(PageViewController);
            View.AddSubview(PageViewController.View);

            // Set the page view controller's bounds using an inset rect so that self's view is visible around the edges of the pages.
            var pageViewRect = View.Bounds;
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                pageViewRect = new CGRect(pageViewRect.X + 20, pageViewRect.Y + 20, pageViewRect.Width - 40, pageViewRect.Height - 40);
            PageViewController.View.Frame = pageViewRect;

            PageViewController.DidMoveToParentViewController(this);

            // Add the page view controller's gesture recognizers to the book view controller's view so that the gestures are started more easily.
            View.GestureRecognizers = PageViewController.GestureRecognizers;


            UIButton btnSkip = new UIButton(UIButtonType.Custom);
            btnSkip.Frame = new CGRect(0f, 0f, 0f, 0f);
            btnSkip.SetTitle("Skip", UIControlState.Normal);
            //btnSkip.Layer.CornerRadius = 5.0f;
            //btnSkip.Layer.BorderWidth = 1.0f;
            //btnSkip.Layer.BorderColor = UIColor.White.CGColor;
            btnSkip.BackgroundColor = UIColor.Clear;
            btnSkip.TitleLabel.Font = myTNBFont.MuseoSans14();
            btnSkip.TitleLabel.TextAlignment = UITextAlignment.Left;
            View.AddSubview(btnSkip);
            btnSkip.TouchUpInside += (sender, e) =>
            {
                OnSkip();
            };

            UIButton btnDone = new UIButton(UIButtonType.Custom);
            btnDone.Frame = new CGRect(0f, 0f, 0f, 0f);
            btnDone.SetTitle("Done", UIControlState.Normal);
            btnDone.BackgroundColor = UIColor.Clear;
            btnDone.TitleLabel.Font = myTNBFont.MuseoSans14();
            btnDone.TitleLabel.TextAlignment = UITextAlignment.Right;
            View.AddSubview(btnDone);
            btnDone.TouchUpInside += (sender, e) =>
            {
                OnSkip();
            };

            btnDone.Hidden = true;

            UIView viewNext = new UIView(new CGRect(0, 0, 24, 24));
            UIImageView imgViewNext = new UIImageView(new CGRect(0, 0, 24, 24));
            imgViewNext.Image = UIImage.FromBundle("Next-White");
            viewNext.AddSubview(imgViewNext);

            UITapGestureRecognizer onNextTap = new UITapGestureRecognizer(() =>
            {
                //Todo: Next function
                ModelController.isNextTapped = true;
                int index = ModelController.currentIndex + 1;
                btnSkip.Hidden = index == ModelController.pageData.Count - 1;
                btnDone.Hidden = index != ModelController.pageData.Count - 1;
                viewNext.Hidden = index == ModelController.pageData.Count - 1;
                var nextVC = ModelController.GetViewController(index, Storyboard);
                var vc = new UIViewController[] { nextVC };
                PageViewController.SetViewControllers(vc, UIPageViewControllerNavigationDirection.Forward, false, null);
            });
            viewNext.AddGestureRecognizer(onNextTap);
            View.AddSubview(viewNext);

            View.AddConstraints(
                btnSkip.AtLeftOf(View, 18),
                btnSkip.AtBottomOf(View, 19),
                btnSkip.Width().EqualTo(50),
                btnSkip.Height().EqualTo(18),

                btnDone.AtRightOf(View, 18),
                btnDone.AtBottomOf(View, 19),
                btnDone.Width().EqualTo(50),
                btnDone.Height().EqualTo(18),

                viewNext.AtRightOf(View, 19),
                viewNext.AtBottomOf(View, 16),
                viewNext.Width().EqualTo(24),
                viewNext.Height().EqualTo(24)
            );
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();


            ModelController.btnDone = btnDone;
            ModelController.btnSkip = btnSkip;
            ModelController.viewNext = viewNext;
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            SetupSuperViewBackground();
        }

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

            int index = ModelController.IndexOf((OnboardingDataViewController)currentViewController);
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

        internal void SetupSuperViewBackground()
        {
            var startColor = myTNBColor.GradientPurpleDarkElement();
            var endColor = myTNBColor.GradientPurpleLightElement();

            var gradientLayer = new CAGradientLayer();
            gradientLayer.Colors = new[] { startColor.CGColor, endColor.CGColor };
            gradientLayer.Locations = new NSNumber[] { 0, 1 };
            gradientLayer.Frame = View.Bounds;

            View.Layer.InsertSublayer(gradientLayer, 0);
        }

        internal void OnSkip()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            sharedPreference.SetBool(true, "isWalkthroughDone");
            sharedPreference.Synchronize();
            UIStoryboard loginStoryboard = UIStoryboard.FromName("Login", null);
            UIViewController preLoginVC = (UIViewController)loginStoryboard.InstantiateViewController("PreloginViewController");
            preLoginVC.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            this.PresentViewController(preLoginVC, true, null);
        }
    }
}