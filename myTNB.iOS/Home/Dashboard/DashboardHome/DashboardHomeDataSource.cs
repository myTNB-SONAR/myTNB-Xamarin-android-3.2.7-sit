﻿using System;
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
        private AccountListViewController _accountListViewController;
        private DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();
        private RefreshScreenComponent _refreshScreenComponent;
        private List<ServiceItemModel> _services;
        private List<HelpModel> _helpList;
        private bool _isServicesShimmering, _isHelpShimmering, _showRefreshScreen;
        private List<PromotionsModelV2> _promotions;
        public Action _onReload;
        public Func<string, string> _getI18NValue;

        public DashboardHomeDataSource(DashboardHomeViewController controller,
            AccountListViewController accountListViewController,
            List<ServiceItemModel> services,
            List<PromotionsModelV2> promotions,
            List<HelpModel> helpList,
            bool isServicesShimmering,
            bool isHelpShimmering,
            bool showRefreshScreen,
            RefreshScreenComponent refreshScreenComponent,
            Func<string, string> getI18NValue,
            Action onReload)
        {
            _controller = controller;
            _accountListViewController = accountListViewController;
            _services = services;
            _promotions = promotions;
            _helpList = helpList;
            _isServicesShimmering = isServicesShimmering;
            _isHelpShimmering = isHelpShimmering;
            _showRefreshScreen = showRefreshScreen;
            _refreshScreenComponent = refreshScreenComponent;
            _getI18NValue = getI18NValue;
            _onReload = onReload;
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
                return _showRefreshScreen ? _refreshScreenComponent?.GetViewHeight() ?? _dashboardHomeHelper.GetDefaulthHeightForRefreshScreen() : _dashboardHomeHelper.GetHeightForAccountList();
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

                if (_showRefreshScreen)
                {
                    cell.AddRefreshViewToContainer(_refreshScreenComponent);
                }
                else
                {
                    cell.AddViewsToContainers(_accountListViewController);
                }
                return cell;
            }
            if (indexPath.Row == 1)
            {
                ServicesTableViewCell cell = tableView.DequeueReusableCell(DashboardHomeConstants.Cell_Services) as ServicesTableViewCell;
                cell.GetI18NValue = _getI18NValue;
                cell.IsRefreshScreen = _showRefreshScreen;
                cell.IsLoading = _isServicesShimmering;
                cell.OnReload = _onReload;
                cell.AddCards(_services, _controller._servicesActionDictionary, _isServicesShimmering);
                cell.ClipsToBounds = true;
                return cell;
            }
            if (indexPath.Row == 2)
            {
                HelpTableViewCell cell = tableView.DequeueReusableCell(DashboardHomeConstants.Cell_Help) as HelpTableViewCell;
                cell._titleLabel.Text = _controller.GetI18NValue(DashboardHomeConstants.I18N_NeedHelp);
                cell.AddCards(_helpList, _isHelpShimmering);
                cell.ClipsToBounds = true;
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