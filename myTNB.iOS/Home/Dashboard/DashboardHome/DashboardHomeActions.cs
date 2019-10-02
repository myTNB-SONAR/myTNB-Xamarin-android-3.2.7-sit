using System;
using System.Collections.Generic;
using UIKit;

namespace myTNB.Home.Dashboard.DashboardHome
{
    public class DashboardHomeActions
    {
        private DashboardHomeViewController _controller;
        public DashboardHomeActions(DashboardHomeViewController controller)
        {
            _controller = controller;
        }

        internal Dictionary<string, Action> GetActionsDictionary()
        {
            return new Dictionary<string, Action>()
            {
                { "1001", On_1001_Action}
                , { "1002", On_1002_Action}
                , { "1003", On_1003_Action}
                , { "1004", On_1004_Action}
                , { "1005", On_1005_Action}
            };
        }

        private void On_1001_Action()
        {
            if (SSMRAccounts.IsHideOnboarding)
            {
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    _controller.InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            UIStoryboard storyBoard = UIStoryboard.FromName("SSMR", null);
                            SSMRReadingHistoryViewController viewController =
                                storyBoard.InstantiateViewController("SSMRReadingHistoryViewController") as SSMRReadingHistoryViewController;
                            if (viewController != null)
                            {
                                viewController.IsFromHome = true;
                                UINavigationController navController = new UINavigationController(viewController);
                                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                _controller.PresentViewController(navController, true, null);
                            }
                        }
                        else
                        {
                            _controller.DisplayNoDataAlert();
                        }
                    });
                });
            }
            else
            {
                UIStoryboard onboardingStoryboard = UIStoryboard.FromName("Onboarding", null);
                GenericPageRootViewController onboardingVC =
                    onboardingStoryboard.InstantiateViewController("GenericPageRootViewController") as GenericPageRootViewController;
                onboardingVC.PageType = GenericPageViewEnum.Type.SSMR;
                var navController = new UINavigationController(onboardingVC)
                {
                    ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                };
                _controller.PresentViewController(navController, true, null);
            }
        }

        private void On_1002_Action() { }

        private void On_1003_Action()
        {
            DataManager.DataManager.SharedInstance.IsPreloginFeedback = false;
            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
            FeedbackViewController feedbackVC = storyBoard.InstantiateViewController("FeedbackViewController") as FeedbackViewController;
            if (feedbackVC != null)
            {
                feedbackVC.isFromPreLogin = true;
                UINavigationController navController = new UINavigationController(feedbackVC);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                _controller.PresentViewController(navController, true, null);
            }
        }

        private void On_1004_Action() { }

        private void On_1005_Action() { }
    }
}