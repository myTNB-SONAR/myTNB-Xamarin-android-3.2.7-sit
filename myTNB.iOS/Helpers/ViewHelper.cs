using System;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using UIKit;
namespace myTNB
{
    public static class ViewHelper
    {
        public static CGRect UpdateFeedbackViewYCoord(float parentY, float margin, UIView view, float height)
        {
            var frame = new CGRect();
            frame = view.Frame;
            frame.Y = parentY + margin;
            frame.Height = height;
            return frame;
        }

        /// <summary>
        /// Sets the frame's y coordinate.
        /// </summary>
        /// <param name="adjView">View to be adjusted.</param>
        /// <param name="adjY">New y value.</param>
        public static void AdjustFrameSetY(UIView adjView, nfloat adjY)
        {
            var temp = adjView.Frame;
            temp.Y = adjY;
            adjView.Frame = temp;
        }

        /// <summary>
        /// Adjusts the frame's set x coordinate.
        /// </summary>
        /// <param name="adjView">Adj view.</param>
        /// <param name="adjX">Adj x.</param>
        public static void AdjustFrameSetX(UIView adjView, nfloat adjX)
        {
            var temp = adjView.Frame;
            temp.X = adjX;
            adjView.Frame = temp;
        }

        /// <summary>
        /// Sets the frame's height.
        /// </summary>
        /// <param name="adjView">Adj view.</param>
        /// <param name="adjHeight">Adj height.</param>
        public static void AdjustFrameSetHeight(UIView adjView, nfloat adjHeight)
        {
            var temp = adjView.Frame;
            temp.Height = adjHeight;
            adjView.Frame = temp;
        }

        /// <summary>
        /// Sets the frame's width
        /// </summary>
        /// <param name="adjView"></param>
        /// <param name="adjWidth"></param>
        public static void AdjustFrameSetWidth(UIView adjView, nfloat adjWidth)
        {
            var temp = adjView.Frame;
            temp.Width = adjWidth;
            adjView.Frame = temp;
        }

        /// <summary>
        /// Adjusts the height of the frame.
        /// </summary>
        /// <param name="adjView">Adj view.</param>
        /// <param name="adjY">Adj y.</param>
        public static void AdjustFrameHeight(UIView adjView, nfloat adjY)
        {
            var temp = adjView.Frame;
            temp.Height += adjY;
            adjView.Frame = temp;
        }

        /// <summary>
        /// Adjusts the width of the frame.
        /// </summary>
        /// <param name="adjView">Adj view.</param>
        /// <param name="adjWidth">Adj width.</param>
        public static void AdjustFrameWidth(UIView adjView, nfloat adjWidth)
        {
            var temp = adjView.Frame;
            temp.Width += adjWidth;
            adjView.Frame = temp;
        }

        /// <summary>
        /// Removes all subviews.
        /// </summary>
        /// <param name="parentView">Parent view.</param>
        public static void RemoveAllSubviews(UIView parentView)
        {
            foreach (UIView item in parentView)
            {
                item.RemoveFromSuperview();
            }
        }

        /// <summary>
        /// Dismisses the controllers and select tab.
        /// </summary>
        /// <param name="baseVc">Base vc.</param>
        /// <param name="tabIndexToSelect">Tab index to select.</param>
        /// <param name="willAnimateDismiss">If set to <c>true</c> will animate dismiss.</param>
        /// <param name="willPopToRootOnSelect">If set to <c>true</c> will pop to root on select.</param>
        /// <param name="dismissCompletionHandler">Dismiss completion handler.</param>
        public static HomeTabBarController DismissControllersAndSelectTab(UIViewController baseVc, int tabIndexToSelect, bool willAnimateDismiss,
                                                          bool willPopToRootOnSelect = false, Action dismissCompletionHandler = null)
        {
            try
            {
#if true
                var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                var topVc = AppDelegate.GetTopViewController(baseRootVc);

                HomeTabBarController tabBar = null;
                if (topVc != null && topVc?.ParentViewController is HomeTabBarController)
                {
                    tabBar = topVc?.ParentViewController as HomeTabBarController;
                }
                else
                {
                    var vc = baseVc?.PresentingViewController;
                    while (vc != null)
                    {
                        if (vc is HomeTabBarController)
                        {
                            vc?.DismissViewController(willAnimateDismiss, dismissCompletionHandler);
                            break;
                        }

                        if (vc is UINavigationController)
                        {
                            if (vc is UINavigationController nav)
                            {
                                nav?.PopToRootViewController(false);
                            }
                        }
                        else
                        {
                            vc?.DismissViewController(false, null);
                        }

                        vc = vc?.PresentingViewController;
                    }

                    tabBar = vc as HomeTabBarController;
                }

                if (tabBar != null)
                {
                    tabBar.SelectedIndex = (tabIndexToSelect > -1) ? tabIndexToSelect : 0;

                    if (willPopToRootOnSelect)
                    {
                        var selectedVc = tabBar.SelectedViewController;
                        if (selectedVc != null && selectedVc is UINavigationController)
                        {
                            if (selectedVc is UINavigationController nav)
                            {
                                nav?.PopToRootViewController(false);
                            }
                        }
                    }

                    return tabBar;
                }
#else
                var presentedVc = baseVc?.View?.Window?.RootViewController?.PresentedViewController;
                if (presentedVc is HomeTabBarController)
                {
                    var tabBar = presentedVc as HomeTabBarController;
                    if (tabIndexToSelect < tabBar.ViewControllers?.Length)
                    {
                        tabBar.SelectedIndex = tabIndexToSelect;
                    }
                    tabBar.DismissViewController(willAnimateDismiss, dismissCompletionHandler);
                }
#endif
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in DismissControllersAndSelectTab: " + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Opens the browser with URL.
        /// </summary>
        /// <param name="urlStr">URL string.</param>
        public static void OpenBrowserWithUrl(string urlStr)
        {
            if (!string.IsNullOrEmpty(urlStr))
            {
                NSUrl url = new NSUrl(urlStr);
                UIApplication.SharedApplication.OpenUrl(url);
            }
        }

        /// <summary>
        /// Gos to FAQ screen with identifier.
        /// </summary>
        /// <param name="faqId">FAQ identifier.</param>
        public static void GoToFAQScreenWithId(string faqId)
        {
            var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            var topVc = AppDelegate.GetTopViewController(baseRootVc);
            if (topVc != null)
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("FAQ", null);
                FAQViewController viewController =
                    storyBoard.InstantiateViewController("FAQViewController") as FAQViewController;
                viewController.faqId = faqId;
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                topVc.PresentViewController(navController, true, null);
            }
        }
    }
}
