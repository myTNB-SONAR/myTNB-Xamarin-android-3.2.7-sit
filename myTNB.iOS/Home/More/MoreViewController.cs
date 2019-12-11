using Foundation;
using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using CoreGraphics;
using myTNB.Home.More;
using System.Collections.Generic;
using myTNB.Registration;
using myTNB.DataManager;
using System.Diagnostics;
using myTNB.Profile;
using System.Threading.Tasks;
using myTNB.SitecoreCMS;

namespace myTNB
{
    public partial class MoreViewController : CustomUIViewController
    {
        public MoreViewController(IntPtr handle) : base(handle) { }
        private TitleBarComponent _titleBarComponent;
        private UILabel _lblAppVersion;
        private bool _isSitecoreDone, _isMasterDataDone;
        private GenericSelectorViewController languageViewController;

        public override void ViewDidLoad()
        {
            PageName = ProfileConstants.Pagename_Profile;
            NavigationController.NavigationBarHidden = true;
            base.ViewDidLoad();
            _isSitecoreDone = false;
            _isMasterDataDone = false;
            SetSubviews();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            //moreTableView.Source = new MoreDataSource(this, GetMoreList());
            //ReloadData();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        protected override void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> MORE LanguageDidChange");
            base.LanguageDidChange(notification);
            _titleBarComponent?.SetTitle(GetI18NValue(ProfileConstants.I18N_NavTitle));
            _lblAppVersion.Text = string.Format("{0} {1}", GetI18NValue(ProfileConstants.I18N_AppVersion), AppVersionHelper.GetAppShortVersion());
            if (!TNBGlobal.IsProduction)
            {
                _lblAppVersion.Text += string.Format("({0})", AppVersionHelper.GetBuildVersion());
            }
        }

        private Dictionary<string, List<string>> GetMoreList()
        {
            Dictionary<string, List<string>> _itemsDictionary = new Dictionary<string, List<string>>(){
                {GetI18NValue(ProfileConstants.I18N_Settings), new List<string>{ GetI18NValue(ProfileConstants.I18N_MyAccount)
                    , GetI18NValue(ProfileConstants.I18N_Notifications)
                    , GetI18NValue(ProfileConstants.I18N_SetAppLanguage)}}
                , {GetI18NValue(ProfileConstants.I18N_HelpAndSupport), new List<string>{ GetI18NValue(ProfileConstants.I18N_FindUs)
                    , GetI18NValue(ProfileConstants.I18N_CallUsOutagesAndBreakdown)
                    ,GetI18NValue(ProfileConstants.I18N_CallUsBilling)
                    ,GetI18NValue(ProfileConstants.I18N_FAQ)
                    , GetI18NValue(ProfileConstants.I18N_TNC)}}
                , {GetI18NValue(ProfileConstants.I18N_Share), new List<string>{ GetI18NValue(ProfileConstants.I18N_ShareDescription)
                    , GetI18NValue(ProfileConstants.I18N_Rate)}}
            };
            if (_itemsDictionary.ContainsKey(GetI18NValue(ProfileConstants.I18N_HelpAndSupport)) && IsValidWeblinks())
            {
                int cloIndex = DataManager.DataManager.SharedInstance.WebLinks.FindIndex(x => x.Code.ToLower().Equals("tnbclo"));
                int cleIndex = DataManager.DataManager.SharedInstance.WebLinks.FindIndex(x => x.Code.ToLower().Equals("tnbcle"));
                if (cloIndex > -1 && cleIndex > -1)
                {
                    List<string> helpAndSupportList = new List<string>
                    {
                        GetI18NValue(ProfileConstants.I18N_FindUs)
                        , DataManager.DataManager.SharedInstance.WebLinks[cloIndex].Title
                        , DataManager.DataManager.SharedInstance.WebLinks[cleIndex].Title
                        , GetI18NValue(ProfileConstants.I18N_FAQ)
                        , GetI18NValue(ProfileConstants.I18N_TNC)
                    };
                    _itemsDictionary[GetI18NValue(ProfileConstants.I18N_HelpAndSupport)] = helpAndSupportList;
                }
            }
            return _itemsDictionary;
        }

        private void SetSubviews()
        {
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            _titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = _titleBarComponent.GetUI();
            _titleBarComponent.SetTitle(GetI18NValue(ProfileConstants.I18N_NavTitle));
            _titleBarComponent.SetPrimaryVisibility(true);
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);

          /*  moreTableView.Frame = new CGRect(0, DeviceHelper.IsIphoneXUpResolution() ? 88 : 64
                , View.Frame.Width, View.Frame.Height - 64 - 49);
            moreTableView.RowHeight = 50f;
            moreTableView.SectionHeaderHeight = 48f;
            moreTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

            _lblAppVersion = new UILabel(new CGRect(18, 16, moreTableView.Frame.Width - 36, 14))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans9_300,
                Text = string.Format("{0} {1}", GetI18NValue(ProfileConstants.I18N_AppVersion), AppVersionHelper.GetAppShortVersion())
            };

            if (!TNBGlobal.IsProduction)
            {
                _lblAppVersion.Text += string.Format("({0})", AppVersionHelper.GetBuildVersion());
            }

            moreTableView.TableFooterView = _lblAppVersion;*/
        }

        internal void RenderSettingsScreen(int section, int row)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {

                    if (NetworkUtility.isReachable)
                    {
                        switch (section)
                        {
                            case 0:
                                {
                                    if (row == 0)
                                    {
                                        GoToMyAccount();
                                    }
                                    else if (row == 1)
                                    {
                                        GetNotificationPreferences();
                                    }
                                    else
                                    {
                                        GoToLanguageSettings();
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    if (row == 0)
                                    {
                                        GoToFindUs();
                                    }
                                    else if (row == 1)
                                    {
                                        CallCustomerService("tnbclo");
                                    }
                                    else if (row == 2)
                                    {
                                        CallCustomerService("tnbcle");
                                    }
                                    //else if (row == 3)
                                    //{
                                    //    ShowBrowser("bill");
                                    //}
                                    else if (row == 3)
                                    {
                                        GoToFAQ();
                                    }
                                    else if (row == 4)
                                    {
                                        GoToTermsAndCondition();
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    if (row == 0)
                                    {
                                        Share();
                                    }
                                    else if (row == 1)
                                    {
                                        OpenAppStore();
                                    }
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private void GoToFindUs()
        {
            DataManager.DataManager.SharedInstance.CurrentStoreTypeIndex = 0;
            DataManager.DataManager.SharedInstance.PreviousStoreTypeIndex = 0;
            DataManager.DataManager.SharedInstance.IsSameStoreType = false;
            DataManager.DataManager.SharedInstance.SelectedLocationTypeID = "all";
            UIStoryboard storyBoard = UIStoryboard.FromName("FindUs", null);
            FindUsViewController viewController =
                storyBoard.InstantiateViewController("FindUsViewController") as FindUsViewController;
            UINavigationController navController = new UINavigationController(viewController);
            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(navController, true, null);
        }

        private void GetNotificationPreferences()
        {
            ActivityIndicator.Show();
            PushNotificationHelper.GetUserNotificationPreferences();
            if (DataManager.DataManager.SharedInstance.NotificationTypeResponse != null
                && DataManager.DataManager.SharedInstance.NotificationTypeResponse?.d != null
                && DataManager.DataManager.SharedInstance.NotificationTypeResponse.d.IsSuccess
                && DataManager.DataManager.SharedInstance.NotificationChannelResponse != null
                && DataManager.DataManager.SharedInstance.NotificationChannelResponse?.d != null
                && DataManager.DataManager.SharedInstance.NotificationChannelResponse.d.IsSuccess)
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("NotificationSettings", null);
                NotificationSettingsViewController viewController = storyBoard.InstantiateViewController("NotificationSettingsViewController") as NotificationSettingsViewController;
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
                ActivityIndicator.Hide();
            }
            else
            {
                string errorMessage = GetErrorI18NValue(Constants.Error_DefaultErrorMessage);
                if (DataManager.DataManager.SharedInstance.NotificationTypeResponse?.d?.didSucceed == false
                    && !string.IsNullOrWhiteSpace(DataManager.DataManager.SharedInstance.NotificationTypeResponse?.d?.DisplayMessage))
                {
                    errorMessage = DataManager.DataManager.SharedInstance.NotificationTypeResponse?.d?.DisplayMessage;
                }
                else if (DataManager.DataManager.SharedInstance.NotificationChannelResponse?.d?.didSucceed == false
                    && !string.IsNullOrWhiteSpace(DataManager.DataManager.SharedInstance.NotificationChannelResponse?.d?.DisplayMessage))
                {
                    errorMessage = DataManager.DataManager.SharedInstance.NotificationChannelResponse?.d?.DisplayMessage;
                }
                DisplayServiceError(errorMessage);
                ActivityIndicator.Hide();
            }
        }

        #region Language
        private void GoToLanguageSettings()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("GenericSelector", null);
            languageViewController = (GenericSelectorViewController)storyBoard
                .InstantiateViewController("GenericSelectorViewController");
            if (languageViewController != null)
            {
                languageViewController.Title = LanguageSettings.Title;
                languageViewController.Items = LanguageSettings.SupportedLanguage;
                languageViewController.HasSectionTitle = true;
                languageViewController.SectionTitle = LanguageSettings.SectionTitle;
                languageViewController.HasCTA = true;
                languageViewController.CTATitle = LanguageSettings.CTATitle;
                languageViewController.OnSelect = OnSelectLanguage;
                languageViewController.OnBack = OnLanguageBack;
                languageViewController.SelectedIndex = LanguageSettings.SelectedLanguageIndex;
                UINavigationController navController = new UINavigationController(languageViewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            }
        }

        private void OnLanguageBack(int index)
        {
            DisplayCustomAlert(GetFormattedLangKey(Constants.Common_ChangeLanguageTitle)
                  , GetCommonI18NValue(Constants.Common_SaveLanguageMessage)
                  , new Dictionary<string, Action> {
                        { GetFormattedLangKey(Constants.Common_ChangeLanguageNo)
                            , ()=>{ DismissViewController(true, null);} }
                        ,{ GetFormattedLangKey(Constants.Common_ChangeLanguageYes)
                            , ()=>{ OnChangeLanguage(index); } } }
                  , UITextAlignment.Center
                  , UITextAlignment.Center);
        }

        private void OnSelectLanguage(int index)
        {
            DisplayCustomAlert(GetFormattedLangKey(Constants.Common_ChangeLanguageTitle)
                  , GetFormattedLangKey(Constants.Common_ChangeLanguageMessage)
                  , new Dictionary<string, Action> {
                        { GetFormattedLangKey(Constants.Common_ChangeLanguageNo), null}
                        ,{ GetFormattedLangKey(Constants.Common_ChangeLanguageYes)
                            , ()=>{ OnChangeLanguage(index); } } }
                  , UITextAlignment.Center
                  , UITextAlignment.Center);
        }

        private string GetFormattedLangKey(string key)
        {
            return GetCommonI18NValue(string.Format("{0}_{1}", key, TNBGlobal.APP_LANGUAGE));
        }

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
                LanguageUtility.SaveLanguagePreference().ContinueWith(langTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        //Todo: Check success and fail States
                        ClearCache();
                        languageViewController.DismissViewController(true, null);
                        Debug.WriteLine("Change Language Done");
                        NotifCenterUtility.PostNotificationName("LanguageDidChange", new NSObject());
                        ActivityIndicator.Hide();
                    });
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

        private void GoToMyAccount()
        {
            ActivityIndicator.Show();
            ServiceCall.GetRegisteredCards().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    UIStoryboard storyBoard = UIStoryboard.FromName("MyAccount", null);
                    MyAccountViewController viewController =
                        storyBoard.InstantiateViewController("MyAccountViewController") as MyAccountViewController;
                    UINavigationController navController = new UINavigationController(viewController);
                    navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewController(navController, true, null);
                    ActivityIndicator.Hide();
                });
            });
        }

        private void GoToTermsAndCondition()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
            TermsAndConditionViewController viewController =
                storyBoard.InstantiateViewController("TermsAndConditionViewController") as TermsAndConditionViewController;
            if (viewController != null)
            {
                viewController.isPresentedVC = true;
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            }
        }

        private void GoToFAQ()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("FAQ", null);
            FAQViewController viewController =
                storyBoard.InstantiateViewController("FAQViewController") as FAQViewController;
            UINavigationController navController = new UINavigationController(viewController);
            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(navController, true, null);
        }

        private bool IsValidWeblinks()
        {
            return DataManager.DataManager.SharedInstance.WebLinks != null;
        }

        private void CallCustomerService(string code)
        {
            if (IsValidWeblinks())
            {
                int index = DataManager.DataManager.SharedInstance.WebLinks.FindIndex(x => x.Code.ToLower().Equals(code.ToLower()));
                if (index > -1)
                {
                    string number = DataManager.DataManager.SharedInstance.WebLinks[index].Url;
                    if (!string.IsNullOrEmpty(number) && !string.IsNullOrWhiteSpace(number))
                    {
                        NSUrl url = new NSUrl(new Uri("tel:" + number).AbsoluteUri);
                        UIApplication.SharedApplication.OpenUrl(url);
                        return;
                    }
                }
            }
            DisplayServiceError(GetErrorI18NValue(Constants.Error_NumberNotAvailable));
        }

        private void OpenAppStore()
        {
            if (IsValidWeblinks())
            {
                int index = DataManager.DataManager.SharedInstance.WebLinks.FindIndex(x => x.Code.ToLower().Equals("ios"));
                if (index > -1)
                {
                    string url = DataManager.DataManager.SharedInstance.WebLinks[index].Url;
                    if (!string.IsNullOrEmpty(url) && !string.IsNullOrWhiteSpace(url))
                    {
                        UIApplication.SharedApplication.OpenUrl(new NSUrl(string.Format(url)));
                        return;
                    }
                }
            }
            DisplayServiceError(GetErrorI18NValue(Constants.Error_RatingNotAvailable));
        }

        private void Share()
        {
            if (IsValidWeblinks())
            {
                int index = DataManager.DataManager.SharedInstance.WebLinks?.FindIndex(x => x.Code.ToLower().Equals("ios")) ?? -1;
                if (index > -1)
                {
                    NSObject message = NSObject.FromObject(GetI18NValue(ProfileConstants.I18N_ShareMessage));
                    string url = DataManager.DataManager.SharedInstance.WebLinks[index].Url;
                    NSObject item = NSObject.FromObject(url);
                    NSObject[] activityItems = { message, item };
                    UIActivity[] applicationActivities = null;
                    UIActivityViewController activityController = new UIActivityViewController(activityItems, applicationActivities);
                    UIBarButtonItem.AppearanceWhenContainedIn(new[] { typeof(UINavigationBar) }).TintColor = UIColor.White;
                    activityController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewController(activityController, true, null);
                    return;
                }
            }
            DisplayServiceError(GetErrorI18NValue(Constants.Error_ShareNotAvailable));
        }
    }
}