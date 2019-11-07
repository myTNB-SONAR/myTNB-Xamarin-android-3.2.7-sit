using Foundation;
using myTNB.DataManager;
using myTNB.SitecoreCMS;
using myTNB.SitecoreCMS.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UIKit;

namespace myTNB
{
    public partial class OnboardingViewController : CustomUIViewController
    {
        public OnboardingViewController(IntPtr handle) : base(handle) { }

        public List<OnboardingItemModel> model = new List<OnboardingItemModel>();
        public OnboardingEnum onboardingEnum;
        public bool isLogin, isToUpdateMobileNo;
        private bool _isMasterDataDone, _isSitecoreDone;
        private WalkthroughComponent _component;

        public override void ViewDidLoad()
        {
            PageName = OnboardingConstants.PageName;
            base.ViewDidLoad();
            _component = new WalkthroughComponent(View, ViewHeight)
            {
                GetI18NValue = GetI18NValue,
                OnboardingModel = model,
                DismissAction = DismissAction(),
                ChangeLanguageAction = OnChangeLanguage
            };
            View.AddSubview(_component.GetWalkthroughView());
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        /*protected override void LanguageDidChange(NSNotification notification)
        {
            base.LanguageDidChange(notification);
            if (_component != null)
            {
                _component.RefreshContent();
            }
        }*/

        private Action DismissAction()
        {
            if (onboardingEnum == OnboardingEnum.FreshInstall)
            { return NavigateToPreLogin; }

            if (isToUpdateMobileNo)
            { return ShowUpdateMobileNumber; }

            if (isLogin)
            { return NavigateToHome; }

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

        private void ShowUpdateMobileNumber()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("UpdateMobileNumber", null);
            UpdateMobileNumberViewController viewController =
                storyBoard.InstantiateViewController("UpdateMobileNumberViewController") as UpdateMobileNumberViewController;
            if (viewController != null)
            {
                viewController.WillHideBackButton = true;
                viewController.IsFromLogin = true;
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            }
        }

        #region Language
        /*Todo: Do service calls and set lang
         * 1. Call site core
         * 2. Call Applaunch master data
         * 3. Clear Usage cache for service call content
        */
        private void OnChangeLanguage(int index)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        LanguageUtility.SetAppLanguageByIndex(index);
                        InvokeOnMainThread(async () =>
                        {
                            List<Task> taskList = new List<Task>{
                                OnGetAppLaunchMasterData(),
                                OnExecuteSiteCore()
                           };
                            await Task.WhenAll(taskList.ToArray());
                        });
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private void ChangeLanguageCallback()
        {
            if (_isMasterDataDone && _isSitecoreDone)
            {
                InvokeOnMainThread(() =>
                {
                    //Todo: Check success and fail States
                    ClearCache();
                    Debug.WriteLine("Change Language Done");
                    NotifCenterUtility.PostNotificationName("LanguageDidChange", new NSObject());
                    I18NDictionary = LanguageManager.Instance.GetValuesByPage(PageName);
                    if (_component != null)
                    {
                        _component.RefreshContent();
                    }
                    ActivityIndicator.Hide();
                });
            }
        }

        private void ClearCache()
        {
            AccountUsageCache.ClearCache();
            AccountUsageSmartCache.ClearCache();
        }

        private Task OnGetAppLaunchMasterData()
        {
            return Task.Factory.StartNew(() =>
            {
                AppLaunchResponseModel response = ServiceCall.GetAppLaunchMasterData().Result;
                AppLaunchMasterCache.AddAppLaunchResponseData(response);
                _isMasterDataDone = true;
                ChangeLanguageCallback();
            });
        }

        private Task OnExecuteSiteCore()
        {
            return Task.Factory.StartNew(async () =>
            {
                await SitecoreServices.Instance.OnExecuteSitecoreCall(true);
                _isSitecoreDone = true;
                ChangeLanguageCallback();
            });
        }
        #endregion
    }
}
