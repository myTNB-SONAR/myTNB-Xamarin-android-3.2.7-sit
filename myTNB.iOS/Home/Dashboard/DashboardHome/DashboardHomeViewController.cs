using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using myTNB.Dashboard;
using myTNB.Home.Dashboard.DashboardHome;
using myTNB.Model;
using myTNB.PushNotification;
using myTNB.Registration.CustomerAccounts;
using UIKit;

namespace myTNB
{
    public partial class DashboardHomeViewController : CustomUIViewController
    {
        public DashboardHomeViewController(IntPtr handle) : base(handle) { }

        DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();

        private UITableView _homeTableView;
        AccountsCardContentViewController _accountsCardContentViewController;
        private DashboardHomeHeader _dashboardHomeHeader;
        private nfloat _previousScrollOffset;
        private nfloat _imageGradientHeight;
        UIView _headerView;

        internal Dictionary<string, Action> _servicesActionDictionary;

        public override void ViewDidLoad()
        {
            foreach (UIView v in View.Subviews)
            {
                v.RemoveFromSuperview();
            }

            PageName = DashboardHomeConstants.PageName;
            IsGradientImageRequired = true;
            base.ViewDidLoad();
            NSNotificationCenter.DefaultCenter.AddObserver((NSString)"LanguageDidChange", LanguageDidChange);
            NSNotificationCenter.DefaultCenter.AddObserver((NSString)"NotificationDidChange", NotificationDidChange);
            NSNotificationCenter.DefaultCenter.AddObserver((NSString)"OnReceiveNotificationFromDashboard", NotificationDidChange);
            _imageGradientHeight = IsGradientImageRequired ? ImageViewGradientImage.Frame.Height : 0;

            SetActionsDictionary();
            SetStatusBarNoOverlap();
            AddTableView();
            AddTableViewHeader();
            _dashboardHomeHelper.GroupAccountsList(DataManager.DataManager.SharedInstance.AccountRecordsList.d);
            SetAccountsCardViewController();
            InitializeTableView();
            OnUpdateNotification();
        }

        private void SetAccountsCardViewController()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
            _accountsCardContentViewController = storyBoard.InstantiateViewController("AccountsCardContentViewController") as AccountsCardContentViewController;
            _accountsCardContentViewController._groupAccountList = DataManager.DataManager.SharedInstance.AccountsGroupList;
            _accountsCardContentViewController._homeViewController = this;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void SetStatusBarNoOverlap()
        {
            base.SetStatusBarNoOverlap();
            _statusBarView.BackgroundColor = MyTNBColor.ClearBlue;
            _statusBarView.Hidden = true;
        }

        private void NotificationDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> SUMMARY DASHBOARD NotificationDidChange");
            if (_dashboardHomeHeader != null)
            {
                _dashboardHomeHeader.SetNotificationImage(PushNotificationHelper.GetNotificationImage());
            }
            PushNotificationHelper.UpdateApplicationBadge();
        }

        private void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> SUMMARY DASHBOARD LanguageDidChange");
        }

        // <summary>
        // Initializes the table view.
        // </summary>
        private void InitializeTableView()
        {
            _homeTableView.Source = new DashboardHomeDataSource(this, _accountsCardContentViewController, new ServicesResponseModel());
            _homeTableView.ReloadData();
        }

        private void AddTableView()
        {
            nfloat tabbarHeight = TabBarController.TabBar.Frame.Height + 20.0F;
            _homeTableView = new UITableView(new CGRect(0, DeviceHelper.GetStatusBarHeight(), View.Frame.Width
                , View.Frame.Height - DeviceHelper.GetStatusBarHeight() - tabbarHeight))
            { BackgroundColor = UIColor.Clear };
            _homeTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            _homeTableView.RowHeight = UITableView.AutomaticDimension;
            _homeTableView.EstimatedRowHeight = 600.0F;
            _homeTableView.RegisterClassForCellReuse(typeof(AccountsTableViewCell), DashboardHomeConstants.Cell_Accounts);
            _homeTableView.RegisterClassForCellReuse(typeof(HelpTableViewCell), DashboardHomeConstants.Cell_Help);
            _homeTableView.RegisterClassForCellReuse(typeof(ServicesTableViewCell), DashboardHomeConstants.Cell_Services);
            View.AddSubview(_homeTableView);
        }

        private void AddTableViewHeader()
        {
            _dashboardHomeHeader = new DashboardHomeHeader(View);
            _dashboardHomeHeader.SetGreetingText(GetGreeting());
            _dashboardHomeHeader.SetNameText(_dashboardHomeHelper.GetDisplayName());
            _headerView = _dashboardHomeHeader.GetUI();
            _dashboardHomeHeader.AddNotificationAction(OnNotificationAction);
            _homeTableView.TableHeaderView = _headerView;
        }

        private void OnNotificationAction()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("PushNotification", null);
            PushNotificationViewController viewController = storyBoard.InstantiateViewController("PushNotificationViewController") as PushNotificationViewController;
            UINavigationController navController = new UINavigationController(viewController);
            PresentViewController(navController, true, null);
        }

        public void OnAddAccountAction()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("AccountRecords", null);
            var viewController = storyBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
            viewController.isDashboardFlow = true;
            viewController._needsUpdate = true;
            var navController = new UINavigationController(viewController);
            PresentViewController(navController, true, null);
        }

        private string GetGreeting()
        {
            DateTime now = DateTime.Now;
            string key = DashboardHomeConstants.I18N_Evening;
            if (now.Hour < 12)
            {
                key = DashboardHomeConstants.I18N_Morning;
            }
            else if (now.Hour < 18)
            {
                key = DashboardHomeConstants.I18N_Afternoon;
            }
            return I18NDictionary[key];
        }

        private void OnUpdateNotification()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        DataManager.DataManager.SharedInstance.IsLoadingFromDashboard = true;
                        await PushNotificationHelper.GetNotifications();
                        NSNotificationCenter.DefaultCenter.PostNotificationName("OnReceiveNotificationFromDashboard", new NSObject());
                    }
                    else
                    {
                        //Todo: user don't need to see no data connection?
                        Debug.WriteLine("No Data connection");
                    }
                });
            });
        }

        internal void OnTableViewScroll(UIScrollView scrollView)
        {
            if (ImageViewGradientImage == null) { return; }
            nfloat scrollDiff = scrollView.ContentOffset.Y - _previousScrollOffset;
            if (scrollDiff < 0 || (scrollDiff > (scrollView.ContentSize.Height - scrollView.Frame.Size.Height))) { return; }
            _previousScrollOffset = tableViewAccounts.ContentOffset.Y;
            CGRect frame = ImageViewGradientImage.Frame;
            frame.Y = scrollDiff > 0 ? 0 - scrollDiff : frame.Y + scrollDiff;
            ImageViewGradientImage.Frame = frame;
            _statusBarView.Hidden = !(scrollDiff > 0 && scrollDiff > _imageGradientHeight / 2);
        }

        private void OnGetServices()
        {
            InvokeInBackground(async () =>
            {
                ServicesResponseModel services = await GetServices();
                InvokeOnMainThread(() =>
                {
                    if (services != null && services.d != null && services.d.IsSuccess)
                    {
                        _homeTableView.BeginUpdates();
                        _homeTableView.Source = new DashboardHomeDataSource(this, _accountsCardContentViewController, services);
                        NSIndexPath indexPath = NSIndexPath.Create(0, 1);
                        _homeTableView.ReloadRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.None);
                        _homeTableView.EndUpdates();
                    }
                    else
                    {
                        //Todo: Handle fail scenario
                    }
                });
            });
        }

        private async Task<ServicesResponseModel> GetServices()
        {
            ServiceManager serviceManager = new ServiceManager();
            object usrInf = new
            {
                eid = DataManager.DataManager.SharedInstance.User.Email,
                sspuid = DataManager.DataManager.SharedInstance.User.UserID,
                did = DataManager.DataManager.SharedInstance.UDID,
                ft = DataManager.DataManager.SharedInstance.FCMToken,
                lang = DataManager.DataManager.SharedInstance.LANGUAGE,
                sec_auth_k1 = TNBGlobal.API_KEY_ID,
                sec_auth_k2 = string.Empty,
                ses_param1 = string.Empty,
                ses_param2 = string.Empty
            };
            object request = new { usrInf };
            ServicesResponseModel response = serviceManager.OnExecuteAPIV6<ServicesResponseModel>("GetServices", request);
            return response;
        }

        public void OnAccountCardSelected(DueAmountDataModel model)
        {
            var index = DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.FindIndex(x => x.accNum == model.accNum) ?? -1;

            if (index >= 0)
            {
                var selected = DataManager.DataManager.SharedInstance.AccountRecordsList.d[index];
                DataManager.DataManager.SharedInstance.SelectAccount(selected.accNum);
                DataManager.DataManager.SharedInstance.IsSameAccount = false;
                UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                var vc = storyBoard.InstantiateViewController("DashboardViewController") as DashboardViewController;
                vc.ShouldShowBackButton = true;
                ShowViewController(vc, null);
            }
        }

        private void SetActionsDictionary()
        {
            DashboardHomeActions actions = new DashboardHomeActions(this);
            _servicesActionDictionary = actions.GetActionsDictionary();
        }
    }
}