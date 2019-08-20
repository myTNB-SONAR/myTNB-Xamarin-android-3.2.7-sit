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
using Newtonsoft.Json;

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
        private List<PromotionsModelV2> _promotions;
        private nfloat _previousScrollOffset;
        private nfloat _imageGradientHeight;
        internal Dictionary<string, Action> _servicesActionDictionary;
        private bool _servicesIsShimmering = true;
        private bool _helpIsShimmering = true;
        public bool _isRefreshScreenEnabled = false;
        private nfloat _addtlYValue = 0;
        private bool _isBCRMAvailable = false;

        public override void ViewDidLoad()
        {
            foreach (UIView v in View.Subviews)
            {
                v.RemoveFromSuperview();
            }

            PageName = DashboardHomeConstants.PageName;
            IsGradientImageRequired = true;
            base.ViewDidLoad();
            _isBCRMAvailable = DataManager.DataManager.SharedInstance.IsBcrmAvailable;
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
            PrepareTableView();
            SetGreetingView();
        }

        private void PrepareTableView()
        {
            if (_isBCRMAvailable)
            {
                _dashboardHomeHelper.GroupAccountsList(DataManager.DataManager.SharedInstance.AccountRecordsList.d);
                SetAccountsCardViewController();
                InitializeTableView();
            }
            else
            {
                InitializeTableView();
                ShowRefreshScreen(true, null);
            }
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
            //SSMRAccounts.SetEligibleAccounts();
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
                    //UpdatePromotions();
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
                    DisplayNoDataAlert();
                    Debug.WriteLine("No data connection");
                }
            });
        }

        // <summary>
        // Initializes the table view.
        // </summary>
        private void InitializeTableView()
        {
            _homeTableView.Source = new DashboardHomeDataSource(this, _accountsCardContentViewController, _services, _promotions, _helpList, _servicesIsShimmering, _helpIsShimmering, _isRefreshScreenEnabled, _refreshScreenComponent);
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

        private void ReloadAccountsTable()
        {
            _homeTableView.Source = new DashboardHomeDataSource(this, _accountsCardContentViewController, _services, _promotions, _helpList, _servicesIsShimmering, _helpIsShimmering, _isRefreshScreenEnabled, _refreshScreenComponent);
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
                    if (!string.IsNullOrEmpty(helpItems.Status) && helpItems.Status.ToUpper() == DashboardHomeConstants.Sitecore_Success)
                    {
                        HelpEntity wsManager = new HelpEntity();
                        wsManager.DeleteTable();
                        wsManager.CreateTable();
                        if (helpItems != null && helpItems.Data != null && helpItems.Data.Count > 0)
                        {
                            wsManager.InsertListOfItems(helpItems.Data);
                        }
                    }
                }
            });
        }

        private void UpdatePromotions()
        {
            InvokeInBackground(async () =>
            {
                await OnGetPromotions();
                PromotionsEntity entity = new PromotionsEntity();
                _promotions = entity.GetAllItemsV2();
                InvokeOnMainThread(() =>
                {
                    OnUpdateCell(DashboardHomeConstants.CellIndex_Promotion);
                });
            });
        }

        private Task OnGetPromotions()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS, DataManager.DataManager.SharedInstance.ImageSize
                    , TNBGlobal.SITECORE_URL, TNBGlobal.DEFAULT_LANGUAGE);
                bool isValidTimeStamp = false;
                string promotionTS = iService.GetPromotionsTimestampItem();
                PromotionsTimestampResponseModel promotionTimeStamp = JsonConvert.DeserializeObject<PromotionsTimestampResponseModel>(promotionTS);
                if (promotionTimeStamp != null && promotionTimeStamp.Status.Equals("Success")
                    && promotionTimeStamp.Data != null && promotionTimeStamp.Data[0] != null
                    && !string.IsNullOrEmpty(promotionTimeStamp.Data[0].Timestamp)
                    && !string.IsNullOrWhiteSpace(promotionTimeStamp.Data[0].Timestamp))
                {
                    var sharedPreference = NSUserDefaults.StandardUserDefaults;
                    string currentTS = sharedPreference.StringForKey(DashboardHomeConstants.Sitecore_Timestamp);
                    if (string.IsNullOrEmpty(currentTS) || string.IsNullOrWhiteSpace(currentTS))
                    {
                        sharedPreference.SetString(promotionTimeStamp.Data[0].Timestamp, DashboardHomeConstants.Sitecore_Timestamp);
                        sharedPreference.Synchronize();
                        isValidTimeStamp = true;
                    }
                    else
                    {
                        if (currentTS.Equals(promotionTimeStamp.Data[0].Timestamp))
                        {
                            isValidTimeStamp = false;
                        }
                        else
                        {
                            sharedPreference.SetString(promotionTimeStamp.Data[0].Timestamp, DashboardHomeConstants.Sitecore_Timestamp);
                            sharedPreference.Synchronize();
                            isValidTimeStamp = true;
                        }
                    }
                }
                if (isValidTimeStamp)
                {
                    string promotionsItems = iService.GetPromotionsItem();
                    //Debug.WriteLine("debug: promo items: " + promotionsItems);
                    PromotionsV2ResponseModel promotionResponse = JsonConvert.DeserializeObject<PromotionsV2ResponseModel>(promotionsItems);
                    if (promotionResponse != null && promotionResponse.Status.Equals("Success")
                        && promotionResponse.Data != null && promotionResponse.Data.Count > 0)
                    {
                        PromotionsEntity wsManager = new PromotionsEntity();
                        PromotionsEntity.DeleteTable();
                        wsManager.CreateTable();
                        wsManager.InsertListOfItemsV2(HomeTabBarController.SetValueForNullEndDate(promotionResponse.Data));
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
            if (index > -1)
            {
                var selected = DataManager.DataManager.SharedInstance.AccountRecordsList.d[index];
                DataManager.DataManager.SharedInstance.SelectAccount(selected.accNum);
                DataManager.DataManager.SharedInstance.IsSameAccount = false;

                //AccountManager.Instance.CurrentAccountIndex = index;
                UIStoryboard storyBoard = UIStoryboard.FromName("Usage", null);
                var viewController = storyBoard.InstantiateViewController("UsageViewController") as UsageViewController;
                var navController = new UINavigationController(viewController);
                PresentViewController(navController, true, null);
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
            _homeTableView.Source = new DashboardHomeDataSource(this, _accountsCardContentViewController, _services, _promotions, _helpList, _servicesIsShimmering, _helpIsShimmering, _isRefreshScreenEnabled, _refreshScreenComponent);
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
                if (_refreshScreenComponent != null)
                {
                    if (_refreshScreenComponent.GetView() != null)
                    {
                        _refreshScreenComponent.GetView().RemoveFromSuperview();
                        _refreshScreenComponent = null;
                    }
                }
                _isRefreshScreenEnabled = isFail;
                if (_isRefreshScreenEnabled)
                {
                    CGRect frame = ImageViewGradientImage.Frame;
                    frame.Y = _addtlYValue;
                    ImageViewGradientImage.Frame = frame;

                    var bcrm = DataManager.DataManager.SharedInstance.SystemStatus?.Find(x => x.SystemType == Enums.SystemEnum.BCRM);
                    var bcrmMsg = bcrm?.DowntimeMessage ?? "Error_BCRMMessage".Translate();
                    string desc = _isBCRMAvailable ? model?.RefreshMessage ?? string.Empty : bcrmMsg;

                    _refreshScreenComponent = new RefreshScreenComponent(this, View);
                    _refreshScreenComponent.SetIsBCRMDown(!_isBCRMAvailable);
                    _refreshScreenComponent.SetRefreshButtonHidden(!_isBCRMAvailable);
                    _refreshScreenComponent.SetButtonText(model?.RefreshBtnText ?? string.Empty);
                    _refreshScreenComponent.SetDescription(desc);
                    _refreshScreenComponent.CreateComponent();
                    _refreshScreenComponent.OnButtonTap = RefreshViewForAccounts;

                    _homeTableView.BeginUpdates();
                    _homeTableView.Source = new DashboardHomeDataSource(this, null, _services, _promotions, _helpList, _servicesIsShimmering, _helpIsShimmering, _isRefreshScreenEnabled, _refreshScreenComponent);
                    NSIndexPath indexPath = NSIndexPath.Create(0, 0);
                    _homeTableView.ReloadRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.None);
                    _homeTableView.EndUpdates();

                    if (_accountsCardContentViewController != null)
                    {
                        _accountsCardContentViewController.View.RemoveFromSuperview();
                        _accountsCardContentViewController = null;
                    }
                }
            });
        }

        private void RefreshViewForAccounts()
        {
            if (_refreshScreenComponent != null)
            {
                if (_refreshScreenComponent.GetView() != null)
                {
                    _refreshScreenComponent.GetView().RemoveFromSuperview();
                    _refreshScreenComponent = null;
                }
            }
            _isRefreshScreenEnabled = false;
            CGRect frame = ImageViewGradientImage.Frame;
            frame.Y = 0;
            ImageViewGradientImage.Frame = frame;
            SetAccountsCardViewController();
            ReloadAccountsTable();
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
