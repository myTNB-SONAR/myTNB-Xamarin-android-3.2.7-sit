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
                UIStoryboard storyBoard = UIStoryboard.FromName("SSMR", null);
                SSMRApplicationViewController viewController =
                    storyBoard.InstantiateViewController("SSMRApplicationViewController") as SSMRApplicationViewController;
                viewController.IsApplication = true;
                UINavigationController navController = new UINavigationController(viewController);
                _controller.PresentViewController(navController, true, null);
            }
            else
            {
                UIStoryboard onboardingStoryboard = UIStoryboard.FromName("Onboarding", null);
                GenericPageRootViewController onboardingVC =
                    onboardingStoryboard.InstantiateViewController("GenericPageRootViewController") as GenericPageRootViewController;
                onboardingVC.PageType = GenericPageViewEnum.Type.SSMR;
                var navController = new UINavigationController(onboardingVC);
                _controller.PresentViewController(navController, true, null);
            }
        }

        private void On_1002_Action()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("SSMR", null);
            SSMRCaptureMeterViewController viewController =
                storyBoard.InstantiateViewController("SSMRCaptureMeterViewController") as SSMRCaptureMeterViewController;
            viewController.ReadingDictionary = new Dictionary<string, bool> {
                {"kwh", false },
                {"kvarh",false },
                {"kw",false }
            };
            var navController = new UINavigationController(viewController);
            _controller.PresentViewController(navController, true, null);
        }

        private void On_1003_Action()
        {
            DataManager.DataManager.SharedInstance.IsPreloginFeedback = false;
            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
            FeedbackViewController feedbackVC = storyBoard.InstantiateViewController("FeedbackViewController") as FeedbackViewController;
            if (feedbackVC != null)
            {
                feedbackVC.isFromPreLogin = true;
                var navController = new UINavigationController(feedbackVC);
                _controller.PresentViewController(navController, true, null);
            }
        }

        private void On_1004_Action()
        {

        }

        private void On_1005_Action()
        {

        }
    }
}