using Foundation;
using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using CoreGraphics;
using myTNB.Home.More;
using System.Collections.Generic;
using myTNB.Registration;
using myTNB.DataManager;
using System.Threading.Tasks;
using myTNB.Model;
using myTNB.Extensions;

namespace myTNB
{
    public partial class MoreViewController : UIViewController
    {
        public MoreViewController(IntPtr handle) : base(handle)
        {
        }

        Dictionary<string, List<string>> _itemsDictionary = new Dictionary<string, List<string>>(){
            {"Settings", new List<string>{"My Account", "Notifications"}}
            , {"Help & Support", new List<string>{"Find Us", "Call Us (Outages & Breakdown)", "Call Us (Billing Enquiries)", "FAQ", "Terms & Conditions"}}
            , {"Share", new List<string>{"Share this app", "Rate this app"}}
        };

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetSubviews();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            UpdateDictionary();
            moreTableView.Source = new MoreDataSource(this, _itemsDictionary);
            moreTableView.ReloadData();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        void UpdateDictionary()
        {
            if (_itemsDictionary.ContainsKey("Help & Support") && IsValidWeblinks())
            {
                int cloIndex = DataManager.DataManager.SharedInstance.WebLinks.FindIndex(x => x.Code.ToLower().Equals("tnbclo"));
                int cleIndex = DataManager.DataManager.SharedInstance.WebLinks.FindIndex(x => x.Code.ToLower().Equals("tnbcle"));
                if (cloIndex > -1 && cleIndex > -1)
                {
                    List<string> helpAndSupportList = new List<string>
                    {
                        "Find Us"
                        , DataManager.DataManager.SharedInstance.WebLinks[cloIndex].Title
                        , DataManager.DataManager.SharedInstance.WebLinks[cleIndex].Title
                        , "FAQ"
                        , "Terms & Conditions"
                    };
                    _itemsDictionary["Help & Support"] = helpAndSupportList;
                }
            }
        }

        void SetSubviews()
        {
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("More");
            titleBarComponent.SetNotificationVisibility(true);
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);

            moreTableView.Frame = new CGRect(0, DeviceHelper.IsIphoneXUpResolution() ? 88 : 64, View.Frame.Width, View.Frame.Height - 64 - 49);
            moreTableView.RowHeight = 50f;
            moreTableView.SectionHeaderHeight = 48f;
            moreTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

            UILabel lblAppVersion = new UILabel(new CGRect(18, 16, moreTableView.Frame.Width - 36, 14));
            lblAppVersion.TextColor = myTNBColor.SilverChalice();
            lblAppVersion.Font = myTNBFont.MuseoSans9_300();
            lblAppVersion.Text = "App Version " + NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();

            moreTableView.TableFooterView = lblAppVersion;
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
                        Console.WriteLine("No Network");
                        DisplayAlertMessage("ErrNoNetworkTitle".Translate(), "ErrNoNetworkMsg".Translate());
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
                string errorMessage = string.Empty;
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
                else
                {
                    errorMessage = "DefaultErrorMessage".Translate();
                }
                DisplayAlertMessage("ErrorTitle".Translate(), errorMessage);
                ActivityIndicator.Hide();
            }
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
            viewController.isPresentedVC = true;
            var navController = new UINavigationController(viewController);
            PresentViewController(navController, true, null);
        }

        void GoToFAQ()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("FAQ", null);
            FAQViewController viewController =
                storyBoard.InstantiateViewController("FAQViewController") as FAQViewController;
            var navController = new UINavigationController(viewController);
            PresentViewController(navController, true, null);
        }

        void DisplayAlertMessage(string title, string message)
        {
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, animated: true, completionHandler: null);
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
                    viewController.NavigationTitle = title;
                    viewController.URL = url;
                    viewController.IsDelegateNeeded = false;
                    var navController = new UINavigationController(viewController);
                    PresentViewController(navController, true, null);
                    return;
                }
            }
            var alert = UIAlertController.Create("Browser Error", "Links are not available right now. Please try again later.", UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, animated: true, completionHandler: null);
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
            var alert = UIAlertController.Create("Number Error", "Number is not available right now. Please try again later.", UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, animated: true, completionHandler: null);
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
            var alert = UIAlertController.Create("Rating Error", "Rating is not available right now. Please try again later.", UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, animated: true, completionHandler: null);
        }

        void Share()
        {
            if (IsValidWeblinks())
            {
                int index = DataManager.DataManager.SharedInstance.WebLinks?.FindIndex(x => x.Code.ToLower().Equals("ios")) ?? -1;
                if (index > -1)
                {
                    var message = NSObject.FromObject("New myTNB app is now available in App Store.");
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
            var alert = UIAlertController.Create("Share Error", "Sharing is not available right now. Please try again later.", UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, animated: true, completionHandler: null);
        }
    }
}