using System;
using System.Collections.Generic;
using CoreGraphics;
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
        private bool _isServicesShimmering, _isHelpShimmering;

        public DashboardHomeDataSource(DashboardHomeViewController controller,
            AccountsCardContentViewController accountsCardContentViewController,
            ServicesResponseModel services,
            List<HelpModel> helpList,
            bool isServicesShimmering,
            bool isHelpShimmering)
        {
            _controller = controller;
            _accountsCardContentViewController = accountsCardContentViewController;
            _services = services;
            _helpList = helpList;
            _isServicesShimmering = isServicesShimmering;
            _isHelpShimmering = isHelpShimmering;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return 3;
        }

        public override nfloat EstimatedHeight(UITableView tableView, NSIndexPath indexPath)
        {
            return GetHeightForRow(tableView, indexPath);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row == 0)
            {
                return _dashboardHomeHelper.GetHeightForAccountCards();
            }
            if (indexPath.Row == 1)
            {
                return _dashboardHomeHelper.GetHeightForServices(_isServicesShimmering);
            }
            if (indexPath.Row == 2)
            {
                return _dashboardHomeHelper.GetHeightForHelp(_isHelpShimmering);
            }
            return 0;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row == 0)
            {
                AccountsTableViewCell cell = tableView.DequeueReusableCell(DashboardHomeConstants.Cell_Accounts) as AccountsTableViewCell;
                cell.AddViewsToContainers(_accountsCardContentViewController);
                return cell;
            }
            if (indexPath.Row == 1)
            {
                CGRect accountHeight = tableView.RectForRowAtIndexPath(NSIndexPath.Create(0, 0));
                ServicesTableViewCell cell = tableView.DequeueReusableCell(DashboardHomeConstants.Cell_Services) as ServicesTableViewCell;
                cell._titleLabel.Text = _controller.I18NDictionary[DashboardHomeConstants.I18N_MyServices];
                cell._titleLabel.TextColor = accountHeight.Height < tableView.Frame.Height * 0.40F ? UIColor.White : MyTNBColor.PowerBlue;
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
