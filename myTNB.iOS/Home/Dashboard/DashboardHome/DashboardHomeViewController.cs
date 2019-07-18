using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using myTNB.PushNotification;
using UIKit;

namespace myTNB
{
    public partial class DashboardHomeViewController : CustomUIViewController
    {
        public DashboardHomeViewController(IntPtr handle) : base(handle) { }

        private UITableView _homeTableView;
        UIPageViewController _accountsPageViewController;
        private DashboardHomeHeader _dashboardHomeHeader;
        private nfloat _previousScrollOffset;
        private nfloat _imageGradientHeight;

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

            SetStatusBarNoOverlap();
            AddTableView();
            AddTableViewHeader();
            GetGroupedAccountsList();
            InitializePageView();
            InitializeAccountsPageView();
            OnUpdateNotification();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (DataManager.DataManager.SharedInstance.SummaryNeedsRefresh)
            {
                DataManager.DataManager.SharedInstance.AccountsGroupList.Clear();
                DataManager.DataManager.SharedInstance.SummaryNeedsRefresh = false;
                GetGroupedAccountsList();
                if (_accountsPageViewController != null)
                {
                    _accountsPageViewController.DataSource = new AccountsPageViewDataSource(this, DataManager.DataManager.SharedInstance.AccountsGroupList);
                    var startingViewController = ViewControllerAtIndex(0) as AccountsContentViewController;
                    var viewControllers = new UIViewController[] { startingViewController };
                    _accountsPageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, false, null);
                }
                InitializeAccountsPageView();
            }
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
        // Initializes the accounts page view.
        // </summary>
        private void InitializeAccountsPageView()
        {
            _homeTableView.Source = new DashboardHomeDataSource(this, _accountsPageViewController);
            _homeTableView.ReloadData();
        }

        private void GetGroupedAccountsList()
        {
            var sortedAccounts = new List<CustomerAccountRecordModel>();

            var accountsList = DataManager.DataManager.SharedInstance.AccountRecordsList.d;
            var results = accountsList.GroupBy(x => x.IsREAccount);

            if (results != null && results?.Count() > 0)
            {
                var reAccts = results.Where(x => x.Key == true).SelectMany(y => y).OrderBy(o => o.accountNickName).ToList();
                var normalAccts = results.Where(x => x.Key == false).SelectMany(y => y).OrderBy(o => o.accountNickName).ToList();
                reAccts.AddRange(normalAccts);
                sortedAccounts = reAccts;
            }

            var groupedAccountsList = new List<List<DueAmountDataModel>>();

            int count = 0;
            List<DueAmountDataModel> batchList = new List<DueAmountDataModel>();
            for (int i = 0; i < sortedAccounts.Count; i++)
            {
                if (count < DashboardHomeConstants.MaxAccountPerCard)
                {
                    DueAmountDataModel item = new DueAmountDataModel
                    {
                        accNum = sortedAccounts[i].accNum,
                        accNickName = sortedAccounts[i].accountNickName,
                        IsReAccount = sortedAccounts[i].IsREAccount,
                        IsNormalAccount = sortedAccounts[i].IsNormalMeter
                    };

                    batchList.Add(item);
                    count++;
                }
                else
                {
                    groupedAccountsList.Add(batchList);
                    batchList = new List<DueAmountDataModel>();
                    DueAmountDataModel item = new DueAmountDataModel
                    {
                        accNum = sortedAccounts[i].accNum,
                        accNickName = sortedAccounts[i].accountNickName,
                        IsReAccount = sortedAccounts[i].IsREAccount,
                        IsNormalAccount = sortedAccounts[i].IsNormalMeter
                    };
                    batchList.Add(item);
                    count = 1;
                }

                if (i + 1 == sortedAccounts.Count)
                {
                    groupedAccountsList.Add(batchList);
                }
            }
            DataManager.DataManager.SharedInstance.AccountsGroupList = new List<List<DueAmountDataModel>>();
            DataManager.DataManager.SharedInstance.AccountsGroupList = groupedAccountsList;
        }

        private void InitializePageView()
        {
            _accountsPageViewController = new UIPageViewController(UIPageViewControllerTransitionStyle.Scroll, UIPageViewControllerNavigationOrientation.Horizontal, UIPageViewControllerSpineLocation.Min)
            {
                WeakDelegate = this
            };
            _accountsPageViewController.DataSource = new AccountsPageViewDataSource(this, DataManager.DataManager.SharedInstance.AccountsGroupList);

            var startingViewController = ViewControllerAtIndex(0) as AccountsContentViewController;
            var viewControllers = new UIViewController[] { startingViewController };

            _accountsPageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, false, null);
            _accountsPageViewController.View.Frame = new CGRect(0, 0, View.Frame.Width, 395f);

            AddChildViewController(_accountsPageViewController);
            _accountsPageViewController.DidMoveToParentViewController(this);
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
            _dashboardHomeHeader.SetNameText(GetDisplayName());
            _homeTableView.TableHeaderView = _dashboardHomeHeader.GetUI();
            _dashboardHomeHeader.AddNotificationAction(OnNotificationAction);
        }

        private void OnNotificationAction()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("PushNotification", null);
            PushNotificationViewController viewController = storyBoard.InstantiateViewController("PushNotificationViewController") as PushNotificationViewController;
            UINavigationController navController = new UINavigationController(viewController);
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

        private string GetDisplayName()
        {
            if (DataManager.DataManager.SharedInstance.UserEntity?.Count > 0 && DataManager.DataManager.SharedInstance.UserEntity[0] != null)
            {
                return string.Format("{0}!", DataManager.DataManager.SharedInstance.UserEntity[0]?.displayName);
            }
            return string.Empty;
        }

        public UIViewController ViewControllerAtIndex(int index)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
            var vc = storyBoard.InstantiateViewController("AccountsContentViewController") as AccountsContentViewController;
            vc.pageIndex = index;
            Debug.WriteLine("index: " + index);
            return vc;
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
    }
}
