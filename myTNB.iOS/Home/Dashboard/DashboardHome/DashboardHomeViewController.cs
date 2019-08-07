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
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.Registration.CustomerAccounts;
using UIKit;
using myTNB.Home.Components;

namespace myTNB
{
    public partial class DashboardHomeViewController : CustomUIViewController
    {
        public DashboardHomeViewController(IntPtr handle) : base(handle) { }

        DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();

        private UITableView _homeTableView;
        private AccountsCardContentViewController _accountsCardContentViewController;
        DashboardHomeHeader _dashboardHomeHeader;
        RefreshScreenComponent _refreshScreenComponent;
        public ServicesResponseModel _services;
        public List<HelpModel> _helpList;
        private nfloat _previousScrollOffset;
        private nfloat _imageGradientHeight;
        internal Dictionary<string, Action> _servicesActionDictionary;
        private bool _servicesIsShimmering = true;
        private bool _helpIsShimmering = true;
        private bool _isRefreshScreenEnabled = false;
        private nfloat _addtlYValue = 0;

        public override void ViewDidLoad()
        {
            foreach (UIView v in View.Subviews)
            {
                v.RemoveFromSuperview();
            }

            PageName = DashboardHomeConstants.PageName;
            IsGradientImageRequired = true;
            base.ViewDidLoad();
            NSNotificationCenter.DefaultCenter.AddObserver((NSString)"NotificationDidChange", NotificationDidChange);
            NSNotificationCenter.DefaultCenter.AddObserver((NSString)"OnReceiveNotificationFromDashboard", NotificationDidChange);
            NSNotificationCenter.DefaultCenter.AddObserver((NSString)"LanguageDidChange", LanguageDidChange);
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillEnterForegroundNotification, OnEnterForeground);
            _imageGradientHeight = IsGradientImageRequired ? ImageViewGradientImage.Frame.Height : 0;
            _services = new ServicesResponseModel();
            _helpList = new List<HelpModel>();
            SetActionsDictionary();
            SetStatusBarNoOverlap();
            SetValuesForRefreshState();
            AddTableView();
            _dashboardHomeHelper.GroupAccountsList(DataManager.DataManager.SharedInstance.AccountRecordsList.d);
            SetAccountsCardViewController();
            InitializeTableView();
            SetGreetingView();
        }

        private void SetGreetingView()
        {
            _dashboardHomeHeader = new DashboardHomeHeader(View, this);
            _dashboardHomeHeader.SetGreetingText(GetGreeting());
            _dashboardHomeHeader.SetNameText(_dashboardHomeHelper.GetDisplayName());
            _homeTableView.TableHeaderView = _dashboardHomeHeader.GetUI();
            _dashboardHomeHeader.SetNotificationActionRecognizer(new UITapGestureRecognizer(() =>
            {
                OnNotificationAction();
            }));
        }
        private void SetAccountsCardViewController()
        {
            if (_accountsCardContentViewController != null)
            {
                _accountsCardContentViewController.View.RemoveFromSuperview();
                _accountsCardContentViewController = null;
            }
            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
            _accountsCardContentViewController = storyBoard.InstantiateViewController("AccountsCardContentViewController") as AccountsCardContentViewController;
            _accountsCardContentViewController._groupAccountList = DataManager.DataManager.SharedInstance.AccountsGroupList;
            _accountsCardContentViewController._homeViewController = this;
        }

        private void SetValuesForRefreshState()
        {
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                if (DeviceHelper.IsIphoneXOrXs())
                {
                    _addtlYValue = -165f;
                }
                else
                {
                    _addtlYValue = -195f;
                }
            }
            else if (DeviceHelper.IsIphone6UpResolution())
            {
                _addtlYValue = -125f;
            }
            else
            {
                _addtlYValue = -80f;
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            UpdateGreeting(GetGreeting());
            if (DataManager.DataManager.SharedInstance.SummaryNeedsRefresh)
            {
                SetAccountsCardViewController();
                ReloadAccountsTable();
            }
            SSMRAccounts.SetEligibleAccounts();
            OnLoadHomeData();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            DataManager.DataManager.SharedInstance.SummaryNeedsRefresh = !_isRefreshScreenEnabled;
            base.ViewDidDisappear(animated);
        }

        public void UpdateGreeting(string greeting)
        {
            if (_dashboardHomeHeader != null)
            {
                _dashboardHomeHeader.SetGreetingText(greeting);
            }
        }

        public override void SetStatusBarNoOverlap()
        {
            base.SetStatusBarNoOverlap();
            _statusBarView.BackgroundColor = MyTNBColor.ClearBlue;
            _statusBarView.Hidden = true;
        }

        #region Observer Methods
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
        #endregion

        private void OnEnterForeground(NSNotification notification)
        {
            Debug.WriteLine("On Enter Foreground");
            OnLoadHomeData();
        }

        private void OnLoadHomeData()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                if (NetworkUtility.isReachable)
                {
                    _services = new ServicesResponseModel();
                    _helpList = new List<HelpModel>();
                    OnGetServices();
                    OnUpdateNotification();
                    InvokeOnMainThread(() =>
                    {
                        _helpIsShimmering = true;
                        OnGetHelpInfo().ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                _helpList = new HelpEntity().GetAllItems();
                                DataManager.DataManager.SharedInstance.HelpList = _helpList;
                                _helpIsShimmering = false;
                                OnUpdateCell(DashboardHomeConstants.CellIndex_Help);
                            });
                        });
                    });
                }
                else
                {
                    //Todo: handling?
                    Debug.WriteLine("No data connection");
                }
            });
        }

        // <summary>
        // Initializes the table view.
        // </summary>
        private void InitializeTableView()
        {
            _homeTableView.Source = new DashboardHomeDataSource(this, _accountsCardContentViewController, _services, _helpList, _servicesIsShimmering, _helpIsShimmering, _isRefreshScreenEnabled, _refreshScreenComponent);
            _homeTableView.ReloadData();
        }

        private void AddTableView()
        {
            _homeTableView = new UITableView(new CGRect(0, DeviceHelper.GetStatusBarHeight(), ViewWidth, ViewHeight)) { BackgroundColor = UIColor.Clear };
            _homeTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            _homeTableView.RegisterClassForCellReuse(typeof(AccountsTableViewCell), DashboardHomeConstants.Cell_Accounts);
            _homeTableView.RegisterClassForCellReuse(typeof(ServicesTableViewCell), DashboardHomeConstants.Cell_Services);
            _homeTableView.RegisterClassForCellReuse(typeof(PromotionTableViewCell), DashboardHomeConstants.Cell_Promotion);
            _homeTableView.RegisterClassForCellReuse(typeof(HelpTableViewCell), DashboardHomeConstants.Cell_Help);
            View.AddSubview(_homeTableView);
        }

        public void OnNotificationAction()
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

        public string GetGreeting()
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
            return GetI18NValue(key);
        }

        private void OnUpdateNotification()
        {
            InvokeInBackground(async () =>
            {
                DataManager.DataManager.SharedInstance.IsLoadingFromDashboard = true;
                await PushNotificationHelper.GetNotifications(false);
                InvokeOnMainThread(() =>
                {
                    PushNotificationHelper.UpdateApplicationBadge();
                    NSNotificationCenter.DefaultCenter.PostNotificationName("OnReceiveNotificationFromDashboard", new NSObject());
                });
            });
        }

        internal void OnTableViewScroll(UIScrollView scrollView)
        {
            if (ImageViewGradientImage == null) { return; }
            nfloat addtl = _isRefreshScreenEnabled ? _addtlYValue : 0;
            nfloat scrollDiff = scrollView.ContentOffset.Y - _previousScrollOffset;
            if (scrollDiff < 0 || (scrollDiff > (scrollView.ContentSize.Height - scrollView.Frame.Size.Height))) { return; }
            _previousScrollOffset = tableViewAccounts.ContentOffset.Y;
            CGRect frame = ImageViewGradientImage.Frame;
            frame.Y = scrollDiff > 0 ? 0 - scrollDiff + addtl : frame.Y + scrollDiff;
            ImageViewGradientImage.Frame = frame;
            _statusBarView.Hidden = !(scrollDiff > 0 && scrollDiff > _imageGradientHeight / 2);
        }

        public void UpdateAccountsTableViewCell()
        {
            _homeTableView.BeginUpdates();
            _homeTableView.Source = new DashboardHomeDataSource(this, _accountsCardContentViewController, _services, _helpList, _servicesIsShimmering, _helpIsShimmering, _isRefreshScreenEnabled, _refreshScreenComponent);
            NSIndexPath indexPath = NSIndexPath.Create(0, 1);
            _homeTableView.ReloadRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.None);
            _homeTableView.EndUpdates();
        }

        private void ReloadAccountsTable()
        {
            _homeTableView.Source = new DashboardHomeDataSource(this, _accountsCardContentViewController, _services, _helpList, _servicesIsShimmering, _helpIsShimmering, _isRefreshScreenEnabled, _refreshScreenComponent);
            _homeTableView.ReloadData();
        }

        private void OnGetServices()
        {
            InvokeInBackground(async () =>
            {
                _servicesIsShimmering = true;
                _services = await GetServices();
                InvokeOnMainThread(() =>
                {
                    _servicesIsShimmering = false;
                    if (_services != null && _services.d != null && _services.d.IsSuccess)
                    {
                        DataManager.DataManager.SharedInstance.ServicesList = _services.d.data?.services;
                        OnUpdateCell(DashboardHomeConstants.CellIndex_Services);
                    }
                    else
                    {
                        //Todo: Handle fail scenario
                    }
                });
            });
        }

        private Task OnGetHelpInfo()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , string.Empty, TNBGlobal.SITECORE_URL, TNBGlobal.DEFAULT_LANGUAGE);
                HelpTimeStampResponseModel timeStamp = iService.GetHelpTimestampItem();
                bool needsUpdate = true;
                if (timeStamp != null && timeStamp.Data != null && timeStamp.Data.Count > 0 && timeStamp.Data[0] != null
                    && !string.IsNullOrEmpty(timeStamp.Data[0].Timestamp))
                {
                    NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                    string currentTS = sharedPreference.StringForKey("SiteCoreHelpTimeStamp");
                    if (string.IsNullOrEmpty(currentTS) || string.IsNullOrWhiteSpace(currentTS))
                    {
                        sharedPreference.SetString(timeStamp.Data[0].Timestamp, "SiteCoreHelpTimeStamp");
                        sharedPreference.Synchronize();
                    }
                    else
                    {
                        if (currentTS.Equals(timeStamp.Data[0].Timestamp))
                        {
                            needsUpdate = false;
                        }
                        else
                        {
                            sharedPreference.SetString(timeStamp.Data[0].Timestamp, "SiteCoreHelpTimeStamp");
                            sharedPreference.Synchronize();
                        }
                    }
                }
                else
                {
                    //Todo: Handle fail scenario
                }

                if (needsUpdate)
                {
                    HelpResponseModel helpItems = iService.GetHelpItems();
                    if (helpItems != null && helpItems.Data != null && helpItems.Data.Count > 0)
                    {
                        HelpEntity wsManager = new HelpEntity();
                        wsManager.DeleteTable();
                        wsManager.CreateTable();
                        wsManager.InsertListOfItems(helpItems.Data);
                    }
                }
            });
        }

        private async Task<ServicesResponseModel> GetServices()
        {
            ServiceManager serviceManager = new ServiceManager();
            object request = new { serviceManager.usrInf };
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

        private void OnUpdateCell(int row)
        {
            _homeTableView.BeginUpdates();
            _homeTableView.Source = new DashboardHomeDataSource(this, _accountsCardContentViewController, _services, _helpList, _servicesIsShimmering, _helpIsShimmering, _isRefreshScreenEnabled, _refreshScreenComponent);
            NSIndexPath indexPath = NSIndexPath.Create(0, row);
            _homeTableView.ReloadRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.None);
            _homeTableView.EndUpdates();
        }

        public void OnReloadTableForSearch()
        {
            _homeTableView.BeginUpdates();
            NSIndexPath indexPath = NSIndexPath.Create(0, DashboardHomeConstants.CellIndex_Services);
            _homeTableView.ReloadRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.None);
            _homeTableView.EndUpdates();
        }

        public void ShowRefreshScreen(bool isFail, RefreshScreenInfoModel model = null)
        {
            InvokeOnMainThread(() =>
            {
                _isRefreshScreenEnabled = isFail;
                if (_isRefreshScreenEnabled)
                {
                    CGRect frame = ImageViewGradientImage.Frame;
                    frame.Y = _addtlYValue;
                    ImageViewGradientImage.Frame = frame;
                }

                if (_refreshScreenComponent != null)
                {
                    if (_refreshScreenComponent.GetView() != null)
                    {
                        _refreshScreenComponent.GetView().RemoveFromSuperview();
                        _refreshScreenComponent = null;
                    }
                }

                _refreshScreenComponent = new RefreshScreenComponent(View);
                _refreshScreenComponent.SetButtonText(model?.RefreshBtnText ?? string.Empty);
                _refreshScreenComponent.SetDescription(model?.RefreshMessage ?? string.Empty);
                _refreshScreenComponent.CreateComponent();

                _homeTableView.BeginUpdates();
                _homeTableView.Source = new DashboardHomeDataSource(this, _accountsCardContentViewController, _services, _helpList, _servicesIsShimmering, _helpIsShimmering, _isRefreshScreenEnabled, _refreshScreenComponent);
                NSIndexPath indexPath = NSIndexPath.Create(0, 0);
                _homeTableView.ReloadRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.None);
                _homeTableView.EndUpdates();
            });
        }

        public void DismissmissActiveKeyboard()
        {
            if (_accountsCardContentViewController != null)
            {
                _accountsCardContentViewController.DismissActiveKeyboard();
            }
        }
    }
}
