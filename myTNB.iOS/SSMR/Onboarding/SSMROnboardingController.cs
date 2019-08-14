using System;
using System.Diagnostics;
using CoreGraphics;
using UIKit;

namespace myTNB.SSMR
{
    public class SSMROnboardingController : BasePageViewRootController
    {
        public SSMROnboardingController(UIViewController controller) : base(controller) { }
        private UIView _viewBottomContainer;
        private CustomUIButtonV2 _btnStart;
        public override void OnViewDidLoad()
        {
            PageName = SSMRConstants.Pagename_SSMRWalkthrough;
            base.OnViewDidLoad();
            SSMRAccounts.IsHideOnboarding = true;
            SSMRModelController = new SSMROnboardingModelController();
            SSMRModelController.UpdateWidgets = OnUpdateMainWidgets;
            UIPageControl.UIPageControlAppearance appearance = UIPageControl.Appearance;
            appearance.CurrentPageIndicatorTintColor = MyTNBColor.WaterBlue;
            appearance.BackgroundColor = UIColor.Clear;
            appearance.PageIndicatorTintColor = MyTNBColor.VeryLightPinkTwo;
            SSMRModelController.SetPageData().ContinueWith(task =>
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
                        var startingViewController = SSMRModelController?.GetViewController(0, that.Storyboard);
                        if (startingViewController != null)
                        {
                            var viewControllers = new UIViewController[] { startingViewController };
                            PageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, false, null);
                            PageViewController.WeakDataSource = SSMRModelController;

                            that.AddChildViewController(PageViewController);
                            that.View.AddSubview(PageViewController.View);

                            // Set the page view controller's bounds using an inset rect so that self's view is visible around the edges of the pages.
                            CGRect pageViewRect = that.View.Bounds;
                            pageViewRect.Height = ScaleUtility.GetScaledHeight(488);//-= DeviceHelper.IsIphoneXUpResolution() ? 80 : 60;
                           // if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                             //   pageViewRect = new CGRect(pageViewRect.X + 20, pageViewRect.Y + 20, pageViewRect.Width - 40, pageViewRect.Height - 40);
                            PageViewController.View.Frame = pageViewRect;
                            PageViewController.DidMoveToParentViewController(that);

                            // Add the page view controller's gesture recognizers to the book view controller's view so that the gestures are started more easily.
                            that.View.GestureRecognizers = PageViewController.GestureRecognizers;
                        }
                        AddBack();
                        AddSubviews();
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

        private void SetupSuperViewBackground() { }

        private void AddBack()
        {
            nfloat yLoc = DeviceHelper.GetStatusBarHeight();
            if (that.NavigationController != null)
            {
                nfloat navHeight = that.NavigationController.NavigationBar.Frame.Height;
                yLoc += ((navHeight - 24) / 2);
            }
            UIView viewBack = new UIView(new CGRect(16, yLoc, 24, 24));
            viewBack.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                that.DismissViewController(true, null);
            }));

            UIImageView imgBack = new UIImageView(new CGRect(0, 0, 24, 24))
            {
                Image = UIImage.FromBundle(SSMRConstants.IMG_BackIcon)
            };
            viewBack.AddSubview(imgBack);
            that.View.AddSubview(viewBack);
        }

        private void AddSubviews()
        {
            nfloat baseHeight = that.View.Frame.Height;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                baseHeight -= 20;
            }
            _viewBottomContainer = new UIView(new CGRect(ScaleUtility.GetScaledWidth(24), baseHeight - ScaleUtility.GetScaledHeight(35)
                , that.View.Frame.Width - ScaleUtility.GetScaledWidth(48), ScaleUtility.GetScaledHeight(14)));

            UIView viewSkip = new UIView(new CGRect(_viewBottomContainer.Frame.Width - ScaleUtility.GetScaledWidth(50)
                , 0, ScaleUtility.GetScaledWidth(50), ScaleUtility.GetScaledHeight(14)));
            UILabel lblSkip = new UILabel(new CGRect(0, 0, ScaleUtility.GetScaledWidth(50), ScaleUtility.GetScaledHeight(14)))
            {
                TextColor = MyTNBColor.BrownGrey,
                Font = TNBFont.MuseoSans_12_500,
                TextAlignment = UITextAlignment.Right,
                Text = GetI18NValue(SSMRConstants.I18N_Skip)
            };
            viewSkip.AddSubview(lblSkip);

            _btnStart = new CustomUIButtonV2()
            {
                Frame = new CGRect(ScaleUtility.GetScaledWidth(16), baseHeight - ScaleUtility.GetScaledHeight(64)
                    , that.View.Frame.Width - ScaleUtility.GetScaledWidth(32), ScaleUtility.GetScaledHeight(48)),
                Enabled = true,
                BackgroundColor = MyTNBColor.FreshGreen,
                Hidden = true,
                Font = TNBFont.MuseoSans_16_500
            };
            _btnStart.SetTitle(GetI18NValue(SSMRConstants.I18N_StartApplication), UIControlState.Normal);

            _btnStart.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                DisplaySSMRApplicationForm();
            }));
            viewSkip.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                DisplaySSMRApplicationForm();
            }));

            _viewBottomContainer.AddSubviews(new UIView[] { viewSkip });
            that.View.AddSubviews(new UIView[] { _viewBottomContainer, _btnStart });
        }

        private void OnUpdateMainWidgets(int index)
        {
            int dataCount = SSMRModelController.pageData.Count;
            _viewBottomContainer.Hidden = index == dataCount - 1;
            _btnStart.Hidden = index != dataCount - 1;
        }

        private async void DisplaySSMRApplicationForm()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("SSMR", null);
            SSMRApplicationViewController viewController =
                storyBoard.InstantiateViewController("SSMRApplicationViewController") as SSMRApplicationViewController;
            viewController.IsApplication = true;
            that.NavigationController.PushViewController(viewController, true);
        }
    }
}