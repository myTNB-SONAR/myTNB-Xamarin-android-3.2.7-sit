using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using myTNB.SSMR;
using myTNB.SSMR.Onboarding;
using UIKit;

namespace myTNB
{
    public class SSMROnboardingController : BasePageViewRootController
    {
        public SSMROnboardingController(UIViewController controller) : base(controller) { }
        public override void OnViewDidLoad()
        {
            PageName = "SSMROnboarding";
            base.OnViewDidLoad();

            SSMRModelController = new SSMROnboardingModelController();
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

                            UIPageControl.UIPageControlAppearance appearance = UIPageControl.Appearance;
                            appearance.CurrentPageIndicatorTintColor = MyTNBColor.WaterBlue;
                            appearance.BackgroundColor = UIColor.Clear;
                            appearance.PageIndicatorTintColor = MyTNBColor.VeryLightPinkTwo;
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

        private void SetupSuperViewBackground()
        {

        }

        private void AddBack()
        {
            UIView viewBack = new UIView(new CGRect(16, DeviceHelper.GetStatusBarHeight(), 24, 24));
            viewBack.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                that.DismissViewController(true, null);
            }));

            UIImageView imgBack = new UIImageView(new CGRect(0, 0, 24, 24))
            {
                Image = UIImage.FromBundle("Back-White")
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
            UIView viewDontShow = new UIView(new CGRect(26, baseHeight - 38, that.View.Frame.Width / 2, 20));
            UIImageView imgViewCheckBox = new UIImageView(new CGRect(0, 0, 20, 20))
            {
                Image = UIImage.FromBundle(DataManager.DataManager.SharedInstance.DontShowSSMROnboarding
                    ? SSMRConstants.IMG_Mark : SSMRConstants.IMG_Unmark)
            };
            viewDontShow.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                DataManager.DataManager.SharedInstance.DontShowSSMROnboarding = !DataManager.DataManager.SharedInstance.DontShowSSMROnboarding;
                imgViewCheckBox.Image = UIImage.FromBundle(DataManager.DataManager.SharedInstance.DontShowSSMROnboarding
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

            UIView viewSkip = new UIView(new CGRect(0, baseHeight - 35, 50, 14));
            UILabel lblSkip = new UILabel(new CGRect(0, 0, 24, 14))
            {
                TextColor = MyTNBColor.BrownGrey,
                Font = MyTNBFont.MuseoSans12_500,
                TextAlignment = UITextAlignment.Right,
                Text = GetI18NValue(SSMRConstants.I18N_Skip)
            };
            ResizeView(ref viewSkip, ref lblSkip, 50, 14);
            viewSkip.AddSubview(lblSkip);

            UIButton btnStart = CustomUIButton.GetUIButton(new CGRect(16, baseHeight - 64, that.View.Frame.Width - 32, 48)
                , GetI18NValue(SSMRConstants.I18N_StartApplication));
            btnStart.Enabled = true;
            btnStart.BackgroundColor = MyTNBColor.FreshGreen;
            btnStart.Hidden = true;
            btnStart.TouchUpInside += (sender, e) =>
            {
            };
            viewSkip.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (SSMRModelController?.pageData != null && SSMRModelController?.pageData?.Count > 0)
                {
                    int lastIndex = SSMRModelController.pageData.Count - 1;
                    SSMRModelController.currentIndex = lastIndex;
                    SSMRModelController.isSkipTapped = true;
                    viewSkip.Hidden = true;
                    viewDontShow.Hidden = true;
                    btnStart.Hidden = false;
                    var nextVC = SSMRModelController.GetViewController(lastIndex, that.Storyboard);
                    if (nextVC != null)
                    {
                        var vc = new UIViewController[] { nextVC };
                        PageViewController.SetViewControllers(vc, UIPageViewControllerNavigationDirection.Forward, false, null);
                    }
                }
            }));
            that.View.AddSubviews(new UIView[] { viewDontShow, viewSkip, btnStart });
        }

        private void ResizeView(ref UIView view, ref UILabel lbl, nfloat maxWidth, nfloat maxHeight, bool isOriginalX = false)
        {
            CGSize size = GetLabelSize(lbl, maxWidth, maxHeight);
            lbl.Frame = new CGRect(lbl.Frame.X, lbl.Frame.Y, size.Width, size.Height);
            view.Frame = new CGRect(isOriginalX ? view.Frame.X : that.View.Frame.Width - 24 - size.Width
                , view.Frame.Y, size.Width + lbl.Frame.X, size.Height);
        }
    }
}
