using CoreGraphics;
using Foundation;
using myTNB.DataManager;
using myTNB.Profile;
using myTNB.Registration;
using myTNB.SitecoreCMS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UIKit;

namespace myTNB
{
    public partial class ProfileViewController : CustomUIViewController
    {
        public ProfileViewController(IntPtr handle) : base(handle) { }

        private UIView _viewNotificationMsg;
        private UILabel _lblAppVersion, _lblNotificationDetails;
        private GenericSelectorViewController languageViewController;
        private CustomUIButtonV2 _btnLogout;
        private UITableView _profileTableview;
        private bool IsChangeLanguage;

        public override void ViewDidLoad()
        {
            PageName = ProfileConstants.Pagename_Profile;
            base.ViewDidLoad();
            _isSitecoreDone = false;
            _isMasterDataDone = false;
            SetTableView();
            SetFooterView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ProfileDataSource dataSource = new ProfileDataSource()
            {
                ProfileList = ProfileList,
                ProfileLabels = ProfileLabels,
                OnRowSelect = OnRowSelect
            };
            _profileTableview.Source = dataSource;
            _profileTableview.ReloadData();

            InitializeNotificationMessage();
            if (DataManager.DataManager.SharedInstance.IsMobileNumberUpdated)
            {
                _lblNotificationDetails.Text = GetI18NValue(ProfileConstants.I18N_MobileNumberVerified);
                ShowNotificationMessage();
                DataManager.DataManager.SharedInstance.IsMobileNumberUpdated = false;
            }
            if (DataManager.DataManager.SharedInstance.IsPasswordUpdated)
            {
                _lblNotificationDetails.Text = GetI18NValue(ProfileConstants.I18N_PasswordUpdateSuccess);
                ShowNotificationMessage();
                DataManager.DataManager.SharedInstance.IsPasswordUpdated = false;
            }
            OnGetRegisteredCards();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (IsChangeLanguage)
            {
                DisplayToast(GetI18NValue(ProfileConstants.I18N_ChangeLanguageSuccess), true);
                IsChangeLanguage = false;
            }
        }

        protected override void LanguageDidChange(NSNotification notification)
        {
            base.LanguageDidChange(notification);
            Title = GetI18NValue(ProfileConstants.I18N_NavTitle);
            _lblAppVersion.Text = Version;
            _btnLogout.SetTitle(GetCommonI18NValue(Constants.Common_Logout), UIControlState.Normal);

            ProfileDataSource dataSource = new ProfileDataSource()
            {
                ProfileList = ProfileList,
                ProfileLabels = ProfileLabels,
                OnRowSelect = OnRowSelect
            };
            _profileTableview.Source = dataSource;
            _profileTableview.ReloadData();
            IsChangeLanguage = true;
        }

        private void OnGetRegisteredCards()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        InvokeInBackground(() =>
                        {
                            ServiceCall.GetRegisteredCards().ContinueWith(task =>
                            {
                                InvokeOnMainThread(() =>
                                {
                                    _profileTableview.BeginUpdates();
                                    NSIndexPath indexPath = NSIndexPath.Create(0, 5);
                                    _profileTableview.ReloadRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.None);
                                    _profileTableview.EndUpdates();

                                    if (DataManager.DataManager.SharedInstance.RegisteredCards != null && DataManager.DataManager.SharedInstance.RegisteredCards.d != null)
                                    {
                                        if (!DataManager.DataManager.SharedInstance.RegisteredCards.d.IsSuccess)
                                        {
                                            DisplayCustomAlert(GetErrorI18NValue(Constants.Error_DefaultErrorTitle),
                                            GetErrorI18NValue(Constants.Error_ProfileCCErrorMsg),
                                            new Dictionary<string, Action> { { GetCommonI18NValue(Constants.Common_Ok), null } });
                                        }
                                    }
                                    else
                                    {
                                        DisplayCustomAlert(GetErrorI18NValue(Constants.Error_DefaultErrorTitle),
                                            GetErrorI18NValue(Constants.Error_ProfileCCErrorMsg),
                                            new Dictionary<string, Action> { { GetCommonI18NValue(Constants.Common_Ok), null } });
                                    }
                                });
                            });
                        });
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private Dictionary<string, List<string>> ProfileList
        {
            get
            {
                Dictionary<string, List<string>> profileList = new Dictionary<string, List<string>>();
                profileList.Add(GetI18NValue(ProfileConstants.I18N_MyTNBAccount), new List<string>());
                profileList.Add(GetI18NValue(ProfileConstants.I18N_Settings), new List<string> {
                    GetI18NValue(ProfileConstants.I18N_Notifications)
                    , GetI18NValue(ProfileConstants.I18N_SetAppLanguage)
                });
                profileList.Add(GetI18NValue(ProfileConstants.I18N_HelpAndSupport), new List<string>{
                    GetI18NValue(ProfileConstants.I18N_FindUs)
                     , GetI18NValue(ProfileConstants.I18N_CallUsBilling)
                     , GetI18NValue(ProfileConstants.I18N_CallUsOutagesAndBreakdown)
                     , GetI18NValue(ProfileConstants.I18N_FAQ)
                     , GetI18NValue(ProfileConstants.I18N_TNC)});
                profileList.Add(GetI18NValue(ProfileConstants.I18N_Share), new List<string>{
                    GetI18NValue(ProfileConstants.I18N_ShareDescription)
                    , GetI18NValue(ProfileConstants.I18N_Rate)});
                EvaluateHelpAndSupportList(ref profileList);
                return profileList;
            }
        }

        private void EvaluateHelpAndSupportList(ref Dictionary<string, List<string>> profileList)
        {
            if (profileList.ContainsKey(GetI18NValue(ProfileConstants.I18N_HelpAndSupport)) && IsValidWeblinks)
            {
                int cloIndex = DataManager.DataManager.SharedInstance.WebLinks.FindIndex(x => x.Code.ToLower().Equals("tnbclo"));
                int cleIndex = DataManager.DataManager.SharedInstance.WebLinks.FindIndex(x => x.Code.ToLower().Equals("tnbcle"));
                if (cloIndex > -1 && cleIndex > -1)
                {
                    List<string> helpAndSupportList = new List<string>
                    {
                        GetI18NValue(ProfileConstants.I18N_FindUs)
                        , DataManager.DataManager.SharedInstance.WebLinks[cleIndex].Title
                        , DataManager.DataManager.SharedInstance.WebLinks[cloIndex].Title
                        , GetI18NValue(ProfileConstants.I18N_FAQ)
                        , GetI18NValue(ProfileConstants.I18N_TNC)
                    };
                    profileList[GetI18NValue(ProfileConstants.I18N_HelpAndSupport)] = helpAndSupportList;
                }
            }
        }

        private bool IsValidWeblinks
        {
            get
            {
                return DataManager.DataManager.SharedInstance.WebLinks != null;
            }
        }

        private List<string> ProfileLabels
        {
            get
            {
                return new List<string> {
                    GetCommonI18NValue(Constants.Common_Fullname).ToUpper()
                    , GetCommonI18NValue(Constants.Common_IDNumber).ToUpper()
                    , GetCommonI18NValue(Constants.Common_Email).ToUpper()
                    , GetCommonI18NValue(Constants.Common_MobileNo).ToUpper()
                    , GetCommonI18NValue(Constants.Common_Password).ToUpper()
                    , GetCommonI18NValue(Constants.Common_Cards).ToUpper()
                    , GetI18NValue(ProfileConstants.I18N_ElectricityAccount).ToUpper()
                };
            }
        }

        private void SetTableView()
        {
            Title = GetI18NValue(ProfileConstants.I18N_NavTitle);
            nfloat yLoc = DeviceHelper.IsIOS10AndBelow ? 0 : NavigationController.NavigationBar.Frame.GetMaxY();
            nfloat tabHeight = TabBarController != null && TabBarController.TabBar != null
                && TabBarController.TabBar.Frame != null ? TabBarController.TabBar.Frame.Height : 0;
            nfloat height = DeviceHelper.IsIOS10AndBelow ? View.Frame.Height - tabHeight : ViewHeight;
            _profileTableview = new UITableView(new CGRect(0, yLoc + DeviceHelper.TopSafeAreaInset, View.Frame.Width, height))
            {
                SeparatorStyle = UITableViewCellSeparatorStyle.None,
                ShowsVerticalScrollIndicator = false
            };
            _profileTableview.RegisterClassForCellReuse(typeof(ProfileCell), ProfileConstants.Cell_Profile);
            View.AddSubview(_profileTableview);
        }

        private void SetFooterView()
        {
            UIView footerView = new UIView(new CGRect(0, 0, ViewWidth, GetScaledHeight(118))) { BackgroundColor = MyTNBColor.LightGrayBG };

            _lblAppVersion = new UILabel(new CGRect(BaseMargin, GetScaledHeight(8), BaseMarginedWidth, GetScaledHeight(14)))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = TNBFont.MuseoSans_9_300,
                Text = Version
            };

            UIView logoutView = new UIView(new CGRect(0, GetScaledHeight(38), ViewWidth, GetScaledHeight(80))) { BackgroundColor = UIColor.White };
            _btnLogout = new CustomUIButtonV2
            {
                Frame = new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.FreshGreen
            };
            _btnLogout.SetTitle(GetCommonI18NValue(Constants.Common_Logout), UIControlState.Normal);
            _btnLogout.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("Logout");
                UIAlertController alert = UIAlertController.Create(GetI18NValue(ProfileConstants.I18N_Logout)
                   , GetI18NValue(ProfileConstants.I18N_LogoutMessage), UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create(GetCommonI18NValue(Constants.Common_Ok), UIAlertActionStyle.Default, (obj) =>
                {
                    UIStoryboard storyBoard = UIStoryboard.FromName("Logout", null);
                    LogoutViewController viewController =
                        storyBoard.InstantiateViewController("LogoutViewController") as LogoutViewController;
                    UINavigationController navController = new UINavigationController(viewController)
                    {
                        ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                    };
                    PresentViewController(navController, true, null);
                }));
                alert.AddAction(UIAlertAction.Create(GetCommonI18NValue(Constants.Common_Cancel), UIAlertActionStyle.Cancel, null));
                alert.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(alert, animated: true, completionHandler: null);
            }));
            logoutView.AddSubview(_btnLogout);
            footerView.AddSubviews(new UIView[] { _lblAppVersion, logoutView });
            _profileTableview.TableFooterView = footerView;
        }

        private string Version
        {
            get
            {
                string appVersion = string.Format("{0} {1}", GetI18NValue(ProfileConstants.I18N_AppVersion)
                    , AppVersionHelper.GetAppShortVersion());
                if (!TNBGlobal.IsProduction)
                {
                    appVersion += string.Format("({0}): {1}", AppVersionHelper.GetBuildVersion(), DataManager.DataManager.SharedInstance.UDID);
                }
                return appVersion;
            }
        }

        private void OnRowSelect(int section, int row)
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
                                    switch (row)
                                    {
                                        case 3:
                                            {
                                                UpdateMobileNumber();
                                                break;
                                            }
                                        case 4:
                                            {
                                                UpdatePassword();
                                                break;
                                            }
                                        case 5:
                                            {
                                                ManageRegisteredCards();
                                                break;
                                            }
                                        case 6:
                                            {
                                                GoToMyAccount();
                                                break;
                                            }
                                        default:
                                            {
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    switch (row)
                                    {
                                        case 0:
                                            GetNotificationPreferences();
                                            break;
                                        case 1:
                                            GoToLanguageSettings();
                                            break;
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    switch (row)
                                    {
                                        case 0:
                                            GoToFindUs();
                                            break;
                                        case 1:
                                            CallCustomerService("tnbcle");
                                            break;
                                        case 2:
                                            CallCustomerService("tnbclo");
                                            break;
                                        case 3:
                                            GoToFAQ();
                                            break;
                                        case 4:
                                            GoToTermsAndCondition();
                                            break;
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    switch (row)
                                    {
                                        case 0:
                                            Share();
                                            break;
                                        case 1:
                                            OpenAppStore();
                                            break;
                                    }
                                    break;
                                }
                            default: { break; }
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
        private bool _isSitecoreDone, _isMasterDataDone;
        private int _currentLanguageIndex = LanguageUtility.CurrentLanguageIndex;
        private void GoToLanguageSettings()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("GenericSelector", null);
            languageViewController = (GenericSelectorViewController)storyBoard
                .InstantiateViewController("GenericSelectorViewController");
            if (languageViewController != null)
            {
                _currentLanguageIndex = LanguageUtility.CurrentLanguageIndex;
                languageViewController.Title = LanguageUtility.LanguageTitle;
                languageViewController.Items = LanguageUtility.SupportedLanguageList;
                languageViewController.HasSectionTitle = true;
                languageViewController.SectionTitle = LanguageUtility.LanguageSectionTitle;
                languageViewController.HasCTA = true;
                languageViewController.CTATitle = LanguageUtility.LanguageCTATitle;
                languageViewController.OnSelect = OnSelectLanguage;
                languageViewController.OnBack = OnLanguageBack;
                languageViewController.SelectedIndex = LanguageUtility.CurrentLanguageIndex;
                UINavigationController navController = new UINavigationController(languageViewController)
                {
                    ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                };
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

        private void OnChangeLanguage(int index)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        _currentLanguageIndex = LanguageUtility.CurrentLanguageIndex;
                        LanguageUtility.SetAppLanguageByIndex(index);
                        InvokeOnMainThread(async () =>
                        {
                            AppLaunchResponseModel response = await ServiceCall.GetAppLaunchMasterData();
                            if (response != null && response.d != null && response.d.IsSuccess)
                            {
                                AppLaunchMasterCache.AddAppLaunchResponseData(response);
                                _isMasterDataDone = true;
                                List<Task> taskList = new List<Task>{
                                    OnExecuteSiteCore()
                                };
                                await Task.WhenAll(taskList.ToArray());
                            }
                            else
                            {
                                LanguageUtility.SetAppLanguageByIndex(_currentLanguageIndex);
                                languageViewController.DismissViewController(true, null);
                                DisplayServiceError(response?.d?.DisplayMessage ?? string.Empty);
                                ActivityIndicator.Hide();
                            }
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
                if (SitecoreServices.IsForcedUpdate)
                {
                    Task.Factory.StartNew(() =>
                    {
                        ChangeLanguageCallback();
                    });
                }
                else
                {
                    LanguageUtility.SaveLanguagePreference().ContinueWith(langTask =>
                    {
                        InvokeOnMainThread(() =>
                        {
                            ClearCache();
                            languageViewController.DismissViewController(true, null);
                            Debug.WriteLine("Change Language Done");
                            NotifCenterUtility.PostNotificationName("LanguageDidChange", new NSObject());
                            ActivityIndicator.Hide();
                        });
                    });
                }
            }
        }

        private void ClearCache()
        {
            DataManager.DataManager.SharedInstance.IsSameAccount = false;
            AccountUsageCache.ClearCache();
            AccountUsageSmartCache.ClearCache();
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
            UIStoryboard storyBoard = UIStoryboard.FromName("MyAccount", null);
            MyAccountViewController viewController =
                storyBoard.InstantiateViewController("MyAccountViewController") as MyAccountViewController;
            UINavigationController navController = new UINavigationController(viewController);
            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(navController, true, null);
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

        private void CallCustomerService(string code)
        {
            if (IsValidWeblinks)
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
            if (IsValidWeblinks)
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
            if (IsValidWeblinks)
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

        private void UpdateMobileNumber()
        {
            UpdateMobileNoViewController viewController = new UpdateMobileNoViewController()
            {
                IsUpdate = true
            };
            UINavigationController navController = new UINavigationController(viewController)
            {
                ModalPresentationStyle = UIModalPresentationStyle.FullScreen
            };
            PresentViewController(navController, true, null);
        }

        private void UpdatePassword()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("UpdatePassword", null);
            UpdatePasswordViewController viewController =
                storyBoard.InstantiateViewController("UpdatePasswordViewController") as UpdatePasswordViewController;
            UINavigationController navController = new UINavigationController(viewController)
            {
                ModalPresentationStyle = UIModalPresentationStyle.FullScreen
            };
            PresentViewController(navController, true, null);
        }

        private void ManageRegisteredCards()
        {
            ActivityIndicator.Show();
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        UIStoryboard storyBoard = UIStoryboard.FromName("ManageCards", null);
                        ManageCardViewController viewController =
                            storyBoard.InstantiateViewController("ManageCardViewController") as ManageCardViewController;
                        UINavigationController navController = new UINavigationController(viewController)
                        {
                            ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                        };
                        PresentViewController(navController, true, null);
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        private void InitializeNotificationMessage()
        {
            if (_viewNotificationMsg == null)
            {
                _viewNotificationMsg = new UIView(new CGRect(18, 32, View.Frame.Width - 36, 64))
                {
                    BackgroundColor = MyTNBColor.SunGlow,
                    Hidden = true
                };
                _viewNotificationMsg.Layer.CornerRadius = 2.0f;

                _lblNotificationDetails = new UILabel(new CGRect(16, 16, _viewNotificationMsg.Frame.Width - 32, 32))
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = MyTNBFont.MuseoSans12,
                    TextColor = MyTNBColor.TunaGrey(),
                    Text = TNBGlobal.EMPTY_ADDRESS,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap
                };

                _viewNotificationMsg.AddSubview(_lblNotificationDetails);

                UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
                currentWindow.AddSubview(_viewNotificationMsg);
            }
        }

        private void ShowNotificationMessage()
        {
            _viewNotificationMsg.Hidden = false;
            _viewNotificationMsg.Alpha = 1.0f;
            UIView.Animate(1, 3, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                _viewNotificationMsg.Alpha = 0.0f;
            }, () =>
            {
                _viewNotificationMsg.Hidden = true;
            });
        }
    }
}