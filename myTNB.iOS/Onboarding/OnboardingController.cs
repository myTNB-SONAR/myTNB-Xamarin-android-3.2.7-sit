using System;
using System.Diagnostics;
using Cirrious.FluentLayouts.Touch;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public class OnboardingController : GenericPageViewRootController
    {
        public OnboardingController(UIViewController controller) : base(controller) { }
        public override void OnViewDidLoad()
        {
            ModelController = new ModelController();
            ModelController.SetPageData().ContinueWith(task =>
            {
                that.InvokeOnMainThread(() =>
                {
                    try
                    {
                        // Configure the page view controller and add it as a child view controller.
                        PageViewController = new UIPageViewController(UIPageViewControllerTransitionStyle.Scroll, UIPageViewControllerNavigationOrientation.Horizontal, UIPageViewControllerSpineLocation.Min)
                        {
                            WeakDelegate = that
                        };
                        var startingViewController = ModelController?.GetViewController(0, that.Storyboard);
                        if (startingViewController != null)
                        {
                            var viewControllers = new UIViewController[] { startingViewController };
                            PageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, false, null);
                            PageViewController.WeakDataSource = ModelController;

                            that.AddChildViewController(PageViewController);
                            that.View.AddSubview(PageViewController.View);

                            // Set the page view controller's bounds using an inset rect so that self's view is visible around the edges of the pages.
                            var pageViewRect = that.View.Bounds;
                            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                                pageViewRect = new CGRect(pageViewRect.X + 20, pageViewRect.Y + 20, pageViewRect.Width - 40, pageViewRect.Height - 40);
                            PageViewController.View.Frame = pageViewRect;

                            PageViewController.DidMoveToParentViewController(that);

                            // Add the page view controller's gesture recognizers to the book view controller's view so that the gestures are started more easily.
                            that.View.GestureRecognizers = PageViewController.GestureRecognizers;


                            UIButton btnSkip = new UIButton(UIButtonType.Custom)
                            {
                                Frame = new CGRect(0f, 0f, 0f, 0f)
                            };
                            btnSkip.SetTitle("Onboarding_Skip".Translate(), UIControlState.Normal);
                            btnSkip.BackgroundColor = UIColor.Clear;
                            btnSkip.TitleLabel.Font = MyTNBFont.MuseoSans14;
                            btnSkip.TitleLabel.TextAlignment = UITextAlignment.Left;
                            that.View.AddSubview(btnSkip);
                            btnSkip.TouchUpInside += (sender, e) =>
                            {
                                OnSkip();
                            };

                            UIButton btnDone = new UIButton(UIButtonType.Custom)
                            {
                                Frame = new CGRect(0f, 0f, 0f, 0f)
                            };
                            btnDone.SetTitle("Common_Done".Translate(), UIControlState.Normal);
                            btnDone.BackgroundColor = UIColor.Clear;
                            btnDone.TitleLabel.Font = MyTNBFont.MuseoSans14;
                            btnDone.TitleLabel.TextAlignment = UITextAlignment.Right;
                            that.View.AddSubview(btnDone);
                            btnDone.TouchUpInside += (sender, e) =>
                            {
                                OnSkip();
                            };

                            btnDone.Hidden = true;

                            UIView viewNext = new UIView(new CGRect(0, 0, 24, 24));
                            UIImageView imgViewNext = new UIImageView(new CGRect(0, 0, 24, 24))
                            {
                                Image = UIImage.FromBundle("Next-White")
                            };
                            viewNext.AddSubview(imgViewNext);

                            UITapGestureRecognizer onNextTap = new UITapGestureRecognizer(() =>
                            {
                                if (ModelController?.pageData != null && ModelController?.pageData?.Count > 0)
                                {
                                    //Todo: Next function
                                    ModelController.isNextTapped = true;
                                    int index = ModelController.currentIndex + 1;
                                    btnSkip.Hidden = index == ModelController.pageData.Count - 1;
                                    btnDone.Hidden = index != ModelController.pageData.Count - 1;
                                    viewNext.Hidden = index == ModelController.pageData.Count - 1;
                                    var nextVC = ModelController.GetViewController(index, that.Storyboard);
                                    if (nextVC != null)
                                    {
                                        var vc = new UIViewController[] { nextVC };
                                        PageViewController.SetViewControllers(vc, UIPageViewControllerNavigationDirection.Forward, false, null);
                                    }
                                }
                            });
                            viewNext.AddGestureRecognizer(onNextTap);
                            that.View.AddSubview(viewNext);

                            that.View.AddConstraints(
                                btnSkip.AtLeftOf(that.View, 18),
                                btnSkip.AtBottomOf(that.View, 19),
                                btnSkip.Width().EqualTo(50),
                                btnSkip.Height().EqualTo(18),

                                btnDone.AtRightOf(that.View, 18),
                                btnDone.AtBottomOf(that.View, 19),
                                btnDone.Width().EqualTo(50),
                                btnDone.Height().EqualTo(18),

                                viewNext.AtRightOf(that.View, 19),
                                viewNext.AtBottomOf(that.View, 16),
                                viewNext.Width().EqualTo(24),
                                viewNext.Height().EqualTo(24)
                            );
                            that.View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

                            ModelController.btnDone = btnDone;
                            ModelController.btnSkip = btnSkip;
                            ModelController.viewNext = viewNext;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Error: " + e.Message);
                    }
                });
            });
        }

        public override void OnViewDidLayoutSubViews()
        {
            SetupSuperViewBackground();
        }

        [Export("pageViewController:spineLocationForInterfaceOrientation:")]
        public override UIPageViewControllerSpineLocation GetSpineLocation(UIPageViewController pageViewController, UIInterfaceOrientation orientation)
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

        private void SetupSuperViewBackground()
        {
            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;
            var gradientLayer = new CAGradientLayer();
            gradientLayer.Colors = new[] { startColor.CGColor, endColor.CGColor };
            gradientLayer.Locations = new NSNumber[] { 0, 1 };
            gradientLayer.Frame = that.View.Bounds;
            that.View.Layer.InsertSublayer(gradientLayer, 0);
        }

        private void OnSkip()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            sharedPreference.SetBool(true, "isWalkthroughDone");
            sharedPreference.Synchronize();
            UIStoryboard loginStoryboard = UIStoryboard.FromName("Login", null);
            UIViewController preLoginVC = (UIViewController)loginStoryboard.InstantiateViewController("PreloginViewController");
            preLoginVC.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            that.PresentViewController(preLoginVC, true, null);
        }
    }
}
