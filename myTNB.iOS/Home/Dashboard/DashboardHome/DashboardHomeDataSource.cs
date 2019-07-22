﻿using System;
using System.Collections.Generic;
using Foundation;
using myTNB.Model;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class DashboardHomeDataSource : UITableViewSource
    {
        private DashboardHomeViewController _controller;
        private AccountsCardContentViewController _accountsCardContentViewController;
        private DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();
        private ServicesResponseModel _services;
        private List<HelpModel> _helpList;

        public DashboardHomeDataSource(DashboardHomeViewController controller
            , AccountsCardContentViewController accountsCardContentViewController
           , ServicesResponseModel services, List<HelpModel> helpList)
        {
            _controller = controller;
            _accountsCardContentViewController = accountsCardContentViewController;
            _services = services;
            _helpList = helpList;
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
                //cell.UpdateCell(_dashboardHomeHelper.GetHeightForAccountCards());
                cell.AddViewsToContainers(_accountsCardContentViewController);
                return cell;
            }
            if (indexPath.Row == 1)
            {
                ServicesTableViewCell cell = tableView.DequeueReusableCell(DashboardHomeConstants.Cell_Services) as ServicesTableViewCell;
                cell._titleLabel.Text = _controller.I18NDictionary[DashboardHomeConstants.I18N_MyServices];
                cell.AddCards(_services, _controller._servicesActionDictionary);
                return cell;
            }
            if (indexPath.Row == 2)
            {
                HelpTableViewCell cell = tableView.DequeueReusableCell(DashboardHomeConstants.Cell_Help) as HelpTableViewCell;
                cell._titleLabel.Text = _controller.I18NDictionary[DashboardHomeConstants.I18N_NeedHelp];
                cell.AddCards(_helpList);
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
