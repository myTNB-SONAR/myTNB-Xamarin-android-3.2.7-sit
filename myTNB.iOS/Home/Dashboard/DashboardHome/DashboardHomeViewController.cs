using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoreGraphics;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public partial class DashboardHomeViewController : CustomUIViewController
    {
        public DashboardHomeViewController(IntPtr handle) : base(handle) { }

        List<List<CustomerAccountRecordModel>> _groupedAccountsList;

        private UITableView _homeTableView;
        UIPageViewController _accountsPageViewController;

        public override void ViewDidLoad()
        {
            foreach (UIView v in View.Subviews)
            {
                v.RemoveFromSuperview();
            }
            PageName = DashboardHomeConstants.PageName;
            IsGradientRequired = true;
            base.ViewDidLoad();

            SetStatusBarNoOverlap();
            AddTableView();
            AddTableViewHeader();
            GetGroupedAccountsList();
            LoadAccounts();

            _homeTableView.Source = new DashboardHomeDataSource(this, _accountsPageViewController);
            _homeTableView.ReloadData();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
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

            var groupedAccountsList = new List<List<CustomerAccountRecordModel>>();

            int count = 0;
            List<CustomerAccountRecordModel> batchList = new List<CustomerAccountRecordModel>();
            for (int i = 0; i < sortedAccounts.Count; i++)
            {
                if (count < DashboardHomeConstants.MaxAccountPerCard)
                {
                    batchList.Add(sortedAccounts[i]);
                    count++;
                }
                else
                {
                    groupedAccountsList.Add(batchList);
                    batchList = new List<CustomerAccountRecordModel>();
                    batchList.Add(sortedAccounts[i]);
                    count = 1;
                }

                if (i + 1 == sortedAccounts.Count)
                {
                    groupedAccountsList.Add(batchList);
                }
            }

            _groupedAccountsList = groupedAccountsList;
        }

        private void LoadAccounts()
        {
            _accountsPageViewController = new UIPageViewController(UIPageViewControllerTransitionStyle.Scroll, UIPageViewControllerNavigationOrientation.Horizontal, UIPageViewControllerSpineLocation.Min)
            {
                WeakDelegate = this
            };
            _accountsPageViewController.DataSource = new AccountsPageViewDataSource(this, _groupedAccountsList);

            var startingViewController = ViewControllerAtIndex(0) as AccountsContentViewController;
            var viewControllers = new UIViewController[] { startingViewController };

            _accountsPageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, false, null);
            _accountsPageViewController.View.Frame = new CGRect(0, 0, View.Frame.Width, 395f);
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
            DashboardHomeHeader dashboardHomeHeader = new DashboardHomeHeader(View);
            dashboardHomeHeader.SetGreetingText(GetGreeting());
            dashboardHomeHeader.SetNameText(GetDisplayName());
            _homeTableView.TableHeaderView = dashboardHomeHeader.GetUI();
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
            var vc = Storyboard.InstantiateViewController("AccountsContentViewController") as AccountsContentViewController;
            vc.pageIndex = index;
            return vc;
        }

    }
}
