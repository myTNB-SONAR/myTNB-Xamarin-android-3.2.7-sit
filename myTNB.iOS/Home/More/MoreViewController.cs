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

namespace myTNB
{
    public partial class MoreViewController : CustomUIViewController
    {
        public MoreViewController(IntPtr handle) : base(handle)
        {
        }

        TitleBarComponent _titleBarComponent;
        UILabel _lblAppVersion;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NSNotificationCenter.DefaultCenter.AddObserver((Foundation.NSString)"LanguageDidChange", LanguageDidChange);
            SetSubviews();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            moreTableView.Source = new MoreDataSource(this, GetMoreList());
            moreTableView.ReloadData();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> MORE LanguageDidChange");
            _titleBarComponent?.SetTitle("More_Title".Translate());
            _lblAppVersion.Text = string.Format("{0} {1}", "More_AppVersion".Translate()
                , NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString());
        }


        Dictionary<string, List<string>> GetMoreList()
        {
            Dictionary<string, List<string>> _itemsDictionary = new Dictionary<string, List<string>>(){
                {"More_Settings".Translate(), new List<string>{ "More_MyAccount".Translate()
                    , "More_Notifications".Translate()}}//, LanguageSettings.Title}}
                , {"More_HelpAndSupport".Translate(), new List<string>{ "More_FindUs".Translate()
                    , "More_CallUsOutagesAndBreakdown".Translate()
                    , "More_CallUsBilling".Translate()
                    , "More_FAQ".Translate()
                    , "More_TnC".Translate()}}
                , {"More_Share".Translate(), new List<string>{ "More_ShareThisApp".Translate()
                    , "More_RateThisApp".Translate()}}
            };
            if (_itemsDictionary.ContainsKey("More_HelpAndSupport".Translate()) && IsValidWeblinks())
            {
                int cloIndex = DataManager.DataManager.SharedInstance.WebLinks.FindIndex(x => x.Code.ToLower().Equals("tnbclo"));
                int cleIndex = DataManager.DataManager.SharedInstance.WebLinks.FindIndex(x => x.Code.ToLower().Equals("tnbcle"));
                if (cloIndex > -1 && cleIndex > -1)
                {
                    List<string> helpAndSupportList = new List<string>
                    {
                        "More_FindUs".Translate()
                        , DataManager.DataManager.SharedInstance.WebLinks[cloIndex].Title
                        , DataManager.DataManager.SharedInstance.WebLinks[cleIndex].Title
                        , "More_FAQ".Translate()
                        , "More_TnC".Translate()
                    };
                    _itemsDictionary["More_HelpAndSupport".Translate()] = helpAndSupportList;
                }
            }
            return _itemsDictionary;
        }

        void SetSubviews()
        {
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            _titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = _titleBarComponent.GetUI();
            _titleBarComponent.SetTitle("More_Title".Translate());
            _titleBarComponent.SetPrimaryVisibility(true);
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);

            moreTableView.Frame = new CGRect(0, DeviceHelper.IsIphoneXUpResolution() ? 88 : 64
                , View.Frame.Width, View.Frame.Height - 64 - 49);
            moreTableView.RowHeight = 50f;
            moreTableView.SectionHeaderHeight = 48f;
            moreTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

            _lblAppVersion = new UILabel(new CGRect(18, 16, moreTableView.Frame.Width - 36, 14))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans9_300,
                Text = string.Format("{0} {1}", "More_AppVersion".Translate()
               , NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString())
            };

            moreTableView.TableFooterView = _lblAppVersion;
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
                                    /*else
                                    {
                                        GoToLanguageSetting();
                                    }*/
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
                        AlertHandler.DisplayNoDataAlert(this);
                    }
                });
            });
        }

        void GoToFindUs()
        {
            DataManager.DataManager.SharedInstance.CurrentStoreTypeIndex = 0;
            DataManager.DataManager.SharedInstance.PreviousStoreTypeIndex = 0;
            DataManager.DataManager.SharedInstance.IsSameStoreType = false;
            DataManager.DataManager.SharedInstance.SelectedLocationTypeID = "all";
            UIStoryboard storyBoard = UIStoryboard.FromName("FindUs", null);
            FindUsViewController viewController =
                storyBoard.InstantiateViewController("FindUsViewController") as FindUsViewController;
            var navController = new UINavigationController(viewController);
            PresentViewController(navController, true, null);
        }

        void GetNotificationPreferences()
        {
            ActivityIndicator.Show();
            PushNotificationHelper.GetUserNotificationPreferences();
            if (DataManager.DataManager.SharedInstance.NotificationTypeResponse != null
                && DataManager.DataManager.SharedInstance.NotificationTypeResponse?.d != null
                && DataManager.DataManager.SharedInstance.NotificationTypeResponse?.d?.status == "success"
                && DataManager.DataManager.SharedInstance.NotificationTypeResponse?.d?.didSucceed == true
                && DataManager.DataManager.SharedInstance.NotificationChannelResponse != null
                && DataManager.DataManager.SharedInstance.NotificationChannelResponse?.d != null
                && DataManager.DataManager.SharedInstance.NotificationChannelResponse?.d?.status == "success"
                && DataManager.DataManager.SharedInstance.NotificationChannelResponse?.d?.didSucceed == true)
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("NotificationSettings", null);
                NotificationSettingsViewController viewController = storyBoard.InstantiateViewController("NotificationSettingsViewController") as NotificationSettingsViewController;
                var navController = new UINavigationController(viewController);
                PresentViewController(navController, true, null);
                ActivityIndicator.Hide();
            }
            else
            {
                string errorMessage = "Error_DefaultMessage".Translate();
                if (DataManager.DataManager.SharedInstance.NotificationTypeResponse?.d?.didSucceed == false
                    && !string.IsNullOrWhiteSpace(DataManager.DataManager.SharedInstance.NotificationTypeResponse?.d?.message))
                {
                    errorMessage = DataManager.DataManager.SharedInstance.NotificationTypeResponse?.d?.message;
                }
                else if (DataManager.DataManager.SharedInstance.NotificationChannelResponse?.d?.didSucceed == false
                    && !string.IsNullOrWhiteSpace(DataManager.DataManager.SharedInstance.NotificationChannelResponse?.d?.message))
                {
                    errorMessage = DataManager.DataManager.SharedInstance.NotificationChannelResponse?.d?.message;
                }
                AlertHandler.DisplayServiceError(this, errorMessage);
                ActivityIndicator.Hide();
            }
        }

        void GoToLanguageSetting()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("GenericSelector", null);
            GenericSelectorViewController viewController = (GenericSelectorViewController)storyBoard
                .InstantiateViewController("GenericSelectorViewController");
            viewController.Title = LanguageSettings.Title;
            viewController.Items = LanguageSettings.SupportedLanguage;
            viewController.OnSelect = LanguageSettings.OnSelect;
            viewController.SelectedIndex = LanguageSettings.SelectedLangugageIndex;
            var navController = new UINavigationController(viewController);
            PresentViewController(navController, true, null);
        }

        void GoToMyAccount()
        {
            ActivityIndicator.Show();
            ServiceCall.GetRegisteredCards().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    UIStoryboard storyBoard = UIStoryboard.FromName("MyAccount", null);
                    MyAccountViewController viewController =
                        storyBoard.InstantiateViewController("MyAccountViewController") as MyAccountViewController;
                    var navController = new UINavigationController(viewController);
                    PresentViewController(navController, true, null);
                    ActivityIndicator.Hide();
                });
            });
        }

        void GoToTermsAndCondition()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
            TermsAndConditionViewController viewController =
                storyBoard.InstantiateViewController("TermsAndConditionViewController") as TermsAndConditionViewController;
            if (viewController != null)
            {
                viewController.isPresentedVC = true;
                var navController = new UINavigationController(viewController);
                PresentViewController(navController, true, null);
            }
        }

        void GoToFAQ()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("FAQ", null);
            FAQViewController viewController =
                storyBoard.InstantiateViewController("FAQViewController") as FAQViewController;
            var navController = new UINavigationController(viewController);
            PresentViewController(navController, true, null);
        }

        bool IsValidWeblinks()
        {
            return DataManager.DataManager.SharedInstance.WebLinks != null;
        }

        void ShowBrowser(string code)
        {
            if (IsValidWeblinks())
            {
                int index = DataManager.DataManager.SharedInstance.WebLinks.FindIndex(x => x.Code.ToLower().Equals(code.ToLower()));
                if (index > -1)
                {
                    string title = DataManager.DataManager.SharedInstance.WebLinks[index].Title;
                    string url = DataManager.DataManager.SharedInstance.WebLinks[index].Url;
                    UIStoryboard storyBoard = UIStoryboard.FromName("Browser", null);
                    BrowserViewController viewController =
                        storyBoard.InstantiateViewController("BrowserViewController") as BrowserViewController;
                    if (viewController != null)
                    {
                        viewController.NavigationTitle = title;
                        viewController.URL = url;
                        viewController.IsDelegateNeeded = false;
                        var navController = new UINavigationController(viewController);
                        PresentViewController(navController, true, null);
                    }
                    return;
                }
            }
            AlertHandler.DisplayServiceError(this, "Error_LinkNotAvailable".Translate());
        }

        void CallCustomerService(string code)
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
            AlertHandler.DisplayServiceError(this, "Error_TelephoneNumberNotAvailable".Translate());
        }

        void OpenAppStore()
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
            AlertHandler.DisplayServiceError(this, "Error_RatingNotAvailable".Translate());
        }

        void Share()
        {
            if (IsValidWeblinks())
            {
                int index = DataManager.DataManager.SharedInstance.WebLinks?.FindIndex(x => x.Code.ToLower().Equals("ios")) ?? -1;
                if (index > -1)
                {
                    var message = NSObject.FromObject("More_ShareMessage".Translate());
                    string url = DataManager.DataManager.SharedInstance.WebLinks[index].Url;
                    var item = NSObject.FromObject(url);
                    var activityItems = new NSObject[] { message, item };
                    UIActivity[] applicationActivities = null;
                    var activityController = new UIActivityViewController(activityItems, applicationActivities);
                    UIBarButtonItem.AppearanceWhenContainedIn(new[] { typeof(UINavigationBar) }).TintColor = UIColor.White;
                    PresentViewController(activityController, true, null);
                    return;
                }
            }
            AlertHandler.DisplayServiceError(this, "Error_ShareNotAvailable".Translate());
        }
    }
}