﻿using System;
using Foundation;
using UIKit;

namespace myTNB
{
    public class DashboardHomeDataSource : UITableViewSource
    {
        DashboardHomeViewController _controller;
        UIPageViewController _accountsPageViewController;
        AccountsCardContentViewController _accountsCardContentViewController;
        UIView _headerView;
        DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();
        public DashboardHomeDataSource(DashboardHomeViewController controller, UIPageViewController accountsPageViewController, UIView headerView, AccountsCardContentViewController accountsCardContentViewController)
        {
            _controller = controller;
            _accountsPageViewController = accountsPageViewController;
            _headerView = headerView;
            _accountsCardContentViewController = accountsCardContentViewController;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return 3;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row == 0)
            {
                AccountsTableViewCell cell = tableView.DequeueReusableCell(DashboardHomeConstants.Cell_Accounts) as AccountsTableViewCell;
                cell.UpdateCell(_dashboardHomeHelper.GetHeightForAccountCards());
                cell.AddViewsToContainersV2(_accountsCardContentViewController);
                //cell.AddViewsToContainers(_accountsPageViewController, _headerView);
                return cell;
            }
            if (indexPath.Row == 1)
            {
                ServicesTableViewCell cell = tableView.DequeueReusableCell(DashboardHomeConstants.Cell_Services) as ServicesTableViewCell;
                cell._titleLabel.Text = _controller.I18NDictionary[DashboardHomeConstants.I18N_MyServices];
                cell.AddCards();
                return cell;
            }
            if (indexPath.Row == 2)
            {
                HelpTableViewCell cell = tableView.DequeueReusableCell(DashboardHomeConstants.Cell_Help) as HelpTableViewCell;
                cell._titleLabel.Text = _controller.I18NDictionary[DashboardHomeConstants.I18N_NeedHelp];
                cell.AddCards();
                return cell;
            }
            return new UITableViewCell() { BackgroundColor = UIColor.Clear };
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath) { }

        public override void Scrolled(UIScrollView scrollView)
        {
            _controller.OnTableViewScroll(scrollView);
        }
    }
}
