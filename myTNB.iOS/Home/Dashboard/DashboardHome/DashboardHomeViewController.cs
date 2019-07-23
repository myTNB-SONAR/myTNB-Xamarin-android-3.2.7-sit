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

namespace myTNB
{
    public partial class DashboardHomeViewController : CustomUIViewController
    {
        public DashboardHomeViewController(IntPtr handle) : base(handle) { }

        DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();

        private UITableView _homeTableView;
        private AccountsCardContentViewController _accountsCardContentViewController;
        private ServicesResponseModel _services;
        private List<HelpModel> _helpList;
        private nfloat _previousScrollOffset;
        private nfloat _imageGradientHeight;

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
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillEnterForegroundNotification, OnEnterForeground);
            _imageGradientHeight = IsGradientImageRequired ? ImageViewGradientImage.Frame.Height : 0;
            _services = new ServicesResponseModel();
            _helpList = new List<HelpModel>();
            SetActionsDictionary();
            SetStatusBarNoOverlap();
            AddTableView();
            _dashboardHomeHelper.GroupAccountsList(DataManager.DataManager.SharedInstance.AccountRecordsList.d);
            SetAccountsCardViewController();
            InitializeTableView();
            OnUpdateNotification();
        }

        private void SetAccountsCardViewController()
        {
            if (_accountsCardContentViewController != null)
            {
                _accountsCardContentViewController.View.RemoveFromSuperview();
            }
            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
            _accountsCardContentViewController = storyBoard.InstantiateViewController("AccountsCardContentViewController") as AccountsCardContentViewController;
            _accountsCardContentViewController._groupAccountList = DataManager.DataManager.SharedInstance.AccountsGroupList;
            _accountsCardContentViewController._homeViewController = this;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (DataManager.DataManager.SharedInstance.SummaryNeedsRefresh)
            {
                SetAccountsCardViewController();
                ReloadAccountsTable();
            }
            OnLoadHomeData();
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

        private void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> SUMMARY DASHBOARD LanguageDidChange");
        }

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
                    OnGetServices();
                    InvokeInBackground(() =>
                    {
                        OnGetHelpInfo().ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                _helpList = new HelpEntity().GetAllItems();
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
            _homeTableView.Source = new DashboardHomeDataSource(this, _accountsCardContentViewController, _services, _helpList);
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

        public void UpdateAccountsTableViewCell()
        {
            _homeTableView.BeginUpdates();
            _homeTableView.Source = new DashboardHomeDataSource(this, _accountsCardContentViewController, _services, _helpList);
            NSIndexPath indexPath = NSIndexPath.Create(0, 0);
            _homeTableView.ReloadRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.None);
            _homeTableView.EndUpdates();
        }

        private void ReloadAccountsTable()
        {
            _homeTableView.Source = new DashboardHomeDataSource(this, _accountsCardContentViewController, _services, _helpList);
            _homeTableView.ReloadData();
        }

        private void OnGetServices()
        {
            InvokeInBackground(async () =>
            {
                _services = await GetServices();
                InvokeOnMainThread(() =>
                {
                    if (_services != null && _services.d != null && _services.d.IsSuccess)
                    {
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
            object usrInf = new
            {
                eid = DataManager.DataManager.SharedInstance.User.Email,
                sspuid = DataManager.DataManager.SharedInstance.User.UserID,
                did = DataManager.DataManager.SharedInstance.UDID,
                ft = DataManager.DataManager.SharedInstance.FCMToken,
                lang = TNBGlobal.DEFAULT_LANGUAGE,
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

        private void OnUpdateCell(int row)
        {
            _homeTableView.BeginUpdates();
            _homeTableView.Source = new DashboardHomeDataSource(this, _accountsCardContentViewController, _services, _helpList);
            NSIndexPath indexPath = NSIndexPath.Create(0, row);
            _homeTableView.ReloadRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.None);
            _homeTableView.EndUpdates();
        }
    }
}