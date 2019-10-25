using Foundation;
using myTNB.SitecoreCMS.Model;
using System;
using System.Collections.Generic;
using UIKit;

namespace myTNB
{
    public partial class OnboardingViewController : CustomUIViewController
    {
        public List<OnboardingItemModel> model = new List<OnboardingItemModel>();
        public OnboardingEnum onboardingEnum;
        public bool isLogin;
        public OnboardingViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = OnboardingConstants.PageName;
            base.ViewDidLoad();
            WalkthroughComponent component = new WalkthroughComponent(View, ViewHeight);
            component.GetI18NValue = GetI18NValue;
            component.OnboardingModel = model;
            component.DismissAction = DismissAction();
            View.AddSubview(component.GetWalkthroughView());
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        private Action DismissAction()
        {
            if (onboardingEnum == OnboardingEnum.FreshInstall)
                return NavigateToPreLogin;

            if (isLogin)
                return NavigateToHome;

            return NavigateToPreLogin;
        }

        private void NavigateToHome()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
            UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
            loginVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            ShowViewController(loginVC, this);
        }

        private void NavigateToPreLogin()
        {
            UIStoryboard loginStoryboard = UIStoryboard.FromName("Login", null);
            UIViewController preLoginVC = loginStoryboard.InstantiateViewController("PreloginViewController");
            preLoginVC.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            preLoginVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(preLoginVC, true, null);
        }
    }
}
