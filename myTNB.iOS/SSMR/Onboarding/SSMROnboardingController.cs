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
        private UIButton _btnStart;
        public override void OnViewDidLoad()
        {
            PageName = SSMRConstants.Pagename_SSMRWalkthrough;
            base.OnViewDidLoad();
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
                            pageViewRect.Height -= DeviceHelper.IsIphoneXUpResolution() ? 80 : 60;
                            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                                pageViewRect = new CGRect(pageViewRect.X + 20, pageViewRect.Y + 20, pageViewRect.Width - 40, pageViewRect.Height - 40);
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
            _viewBottomContainer = new UIView(new CGRect(26, baseHeight - 38, that.View.Frame.Width - 52, 20));
            UIView viewDontShow = new UIView(new CGRect(0, 0, _viewBottomContainer.Frame.Width / 2, 20));
            UIImageView imgViewCheckBox = new UIImageView(new CGRect(0, 0, 20, 20))
            {
                Image = UIImage.FromBundle(SSMRAccounts.IsHideOnboarding
                    ? SSMRConstants.IMG_Mark : SSMRConstants.IMG_Unmark)
            };
            viewDontShow.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                SSMRAccounts.IsHideOnboarding = !SSMRAccounts.IsHideOnboarding;
                imgViewCheckBox.Image = UIImage.FromBundle(SSMRAccounts.IsHideOnboarding
                    ? SSMRConstants.IMG_Mark : SSMRConstants.IMG_Unmark);
            }));
            UILabel lblDontShow = new UILabel(new CGRect(30, 3, 24, 14))
            {
                TextColor = MyTNBColor.BrownGrey,
                Font = MyTNBFont.MuseoSans12_500,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(SSMRConstants.I18N_DontShow)
            };
            viewDontShow.AddSubviews(new UIView[] { imgViewCheckBox, lblDontShow });
            ResizeView(ref viewDontShow, ref lblDontShow, that.View.Frame.Width / 2, 20, true);

            UIView viewSkip = new UIView(new CGRect(0, 3, 50, 14));
            UILabel lblSkip = new UILabel(new CGRect(0, 0, 24, 14))
            {
                TextColor = MyTNBColor.BrownGrey,
                Font = MyTNBFont.MuseoSans12_500,
                TextAlignment = UITextAlignment.Right,
                Text = GetI18NValue(SSMRConstants.I18N_Skip)
            };
            ResizeView(ref viewSkip, ref lblSkip, 50, 14);
            viewSkip.AddSubview(lblSkip);

            _btnStart = CustomUIButton.GetUIButton(new CGRect(16, baseHeight - 64, that.View.Frame.Width - 32, DeviceHelper.GetScaledHeight(48))
               , GetI18NValue(SSMRConstants.I18N_StartApplication));
            _btnStart.Enabled = true;
            _btnStart.BackgroundColor = MyTNBColor.FreshGreen;
            _btnStart.Hidden = true;
            _btnStart.TouchUpInside += (sender, e) =>
            {
                DisplaySSMRApplicationForm();
            };
            viewSkip.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                DisplaySSMRApplicationForm();
            }));

            _viewBottomContainer.AddSubviews(new UIView[] { viewDontShow, viewSkip });
            that.View.AddSubviews(new UIView[] { _viewBottomContainer, _btnStart });
        }

        private void OnUpdateMainWidgets(int index)
        {
            int dataCount = SSMRModelController.pageData.Count;
            _viewBottomContainer.Hidden = index == dataCount - 1;
            _btnStart.Hidden = index != dataCount - 1;
        }

        private void ResizeView(ref UIView view, ref UILabel lbl, nfloat maxWidth, nfloat maxHeight, bool isOriginalX = false)
        {
            CGSize size = GetLabelSize(lbl, maxWidth, maxHeight);
            lbl.Frame = new CGRect(lbl.Frame.X, lbl.Frame.Y, size.Width, size.Height);
            view.Frame = new CGRect(isOriginalX ? view.Frame.X : _viewBottomContainer.Frame.Width - size.Width
                , view.Frame.Y, size.Width + lbl.Frame.X, size.Height);
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