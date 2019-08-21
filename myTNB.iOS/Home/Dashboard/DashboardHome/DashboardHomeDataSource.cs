using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.Home.Components;
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
        private RefreshScreenComponent _refreshScreenComponent;
        private ServicesResponseModel _services;
        private List<HelpModel> _helpList;
        private bool _isServicesShimmering, _isHelpShimmering, _showRefreshScreen;
        private List<PromotionsModelV2> _promotions;

        public DashboardHomeDataSource(DashboardHomeViewController controller,
            AccountsCardContentViewController accountsCardContentViewController,
            ServicesResponseModel services,
            List<PromotionsModelV2> promotions,
            List<HelpModel> helpList,
            bool isServicesShimmering,
            bool isHelpShimmering,
            bool showRefreshScreen,
            RefreshScreenComponent refreshScreenComponent)
        {
            _controller = controller;
            _accountsCardContentViewController = accountsCardContentViewController;
            _services = services;
            _promotions = promotions;
            _helpList = helpList;
            _isServicesShimmering = isServicesShimmering;
            _isHelpShimmering = isHelpShimmering;
            _showRefreshScreen = showRefreshScreen;
            _refreshScreenComponent = refreshScreenComponent;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return 4;
        }

        public override nfloat EstimatedHeight(UITableView tableView, NSIndexPath indexPath)
        {
            return GetHeightForRow(tableView, indexPath);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row == 0)
            {
                return _showRefreshScreen ? _refreshScreenComponent?.GetViewHeight() ?? _dashboardHomeHelper.GetDefaulthHeightForRefreshScreen() : _dashboardHomeHelper.GetHeightForAccountCards();
            }
            if (indexPath.Row == 1)
            {
                return _dashboardHomeHelper.GetHeightForServices(_isServicesShimmering);
            }
            if (indexPath.Row == 2)
            {
                return _dashboardHomeHelper.GetHeightForPromotions;
            }
            if (indexPath.Row == 3)
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

                if (_showRefreshScreen)
                {
                    cell.AddRefreshViewToContainer(_refreshScreenComponent);
                }
                else
                {
                    cell.AddViewsToContainers(_accountsCardContentViewController);
                }
                return cell;
            }
            if (indexPath.Row == 1)
            {
                CGRect accountHeight = tableView.RectForRowAtIndexPath(NSIndexPath.Create(0, 0));
                ServicesTableViewCell cell = tableView.DequeueReusableCell(DashboardHomeConstants.Cell_Services) as ServicesTableViewCell;
                cell._titleLabel.Text = _controller.GetI18NValue(DashboardHomeConstants.I18N_MyServices);
                cell._titleLabel.TextColor = accountHeight.Height < tableView.Frame.Height * 0.30F ? UIColor.White : MyTNBColor.PowerBlue;
                cell.AddCards(_services, _controller._servicesActionDictionary);
                cell.ClipsToBounds = true;
                return cell;
            }
            if (indexPath.Row == 2)
            {
                PromotionTableViewCell cell = tableView.DequeueReusableCell(DashboardHomeConstants.Cell_Promotion) as PromotionTableViewCell;
                cell._titleLabel.Text = _controller.GetI18NValue(DashboardHomeConstants.I18N_Promotions);
                cell.AddCards(_promotions);
                return cell;
            }
            if (indexPath.Row == 3)
            {
                HelpTableViewCell cell = tableView.DequeueReusableCell(DashboardHomeConstants.Cell_Help) as HelpTableViewCell;
                cell._titleLabel.Text = _controller.GetI18NValue(DashboardHomeConstants.I18N_NeedHelp);
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