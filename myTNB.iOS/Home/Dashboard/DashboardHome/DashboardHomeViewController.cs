using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
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
using myTNB.DataManager;
using static myTNB.HomeTutorialOverlay;
using myTNB.SitecoreCMS;

namespace myTNB
{
    public partial class DashboardHomeViewController : CustomUIViewController
    {
        public DashboardHomeViewController(IntPtr handle) : base(handle) { }

        public DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();

        public UITableView _homeTableView;
        private AccountListViewController _accountListViewController;
        DashboardHomeHeader _dashboardHomeHeader;
        RefreshScreenComponent _refreshScreenComponent;
        UIStoryboard _usageStoryBoard;
        public ServicesResponseModel _services;
        public List<HelpModel> _helpList;
        private List<PromotionsModelV2> _promotions;
        public nfloat _previousScrollOffset;
        internal Dictionary<string, Action> _servicesActionDictionary;
        public bool _accountListIsShimmering = true;
        private bool _servicesIsShimmering = true;
        private bool _helpIsShimmering = true;
        public bool _isRefreshScreenEnabled, _isGetServicesFailed;
        private GetIsSmrApplyAllowedResponseModel _isSMRApplyAllowedResponse;
        private UIImageView _footerImageBG;
        private UIView _tutorialContainer;
        private bool _isBCRMAvailable, _isBCRMPopupDisplayed;
        public string RearrangeSuccessMsg;
        public bool IsRearrangeSaved;

        public override void ViewDidLoad()
        {
            foreach (UIView v in View.Subviews)
            {
                v.RemoveFromSuperview();
            }

            PageName = DashboardHomeConstants.PageName;
            IsNewGradientRequired = true;
            base.ViewDidLoad();
            AddFooterBG();
            _isBCRMAvailable = true;// DataManager.DataManager.SharedInstance.IsBcrmAvailable;
            var accNum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum;
            if (DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count > 0 && accNum != null && !string.IsNullOrEmpty(accNum) && !string.IsNullOrWhiteSpace(accNum))
            {
                DataManager.DataManager.SharedInstance.SelectedAccount = DataManager.DataManager.SharedInstance.AccountRecordsList.d[0];
            }
            DataManager.DataManager.SharedInstance.CurrentAccountList = DataManager.DataManager.SharedInstance.AccountRecordsList?.d;
            NotifCenterUtility.AddObserver((NSString)"NotificationDidChange", NotificationDidChange);
            NotifCenterUtility.AddObserver((NSString)"OnReceiveNotificationFromDashboard", NotificationDidChange);
            NotifCenterUtility.AddObserver(UIApplication.WillEnterForegroundNotification, OnEnterForeground);
            NotifCenterUtility.AddObserver(UIApplication.WillChangeStatusBarFrameNotification, OnChangeStatusBarFrame);
            _services = new ServicesResponseModel();
            _helpList = new List<HelpModel>();
            SetActionsDictionary();
            SetStatusBarNoOverlap();
            AddTableView();
            PrepareTableView();
            SetGreetingView();
            PrepareUsageStoryBoard();
        }

        private void AddFooterBG()
        {
            _footerImageBG = new UIImageView(new CGRect(0, ViewHeight + DeviceHelper.GetStatusBarHeight() - GetScaledHeight(1000F), ViewWidth, GetScaledHeight(1000F)))
            {
                Image = UIImage.FromBundle("Home-Footer-BG")
            };
            View.AddSubview(_footerImageBG);
        }

        private void PrepareUsageStoryBoard()
        {
            _usageStoryBoard = UIStoryboard.FromName("Usage", null);
        }

        private void PrepareTableView()
        {
            if (_isBCRMAvailable)
            {
                SetAccountListViewController();
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

        private void SetAccountListViewController()
        {
            if (_accountListViewController != null)
            {
                _accountListViewController.View.RemoveFromSuperview();
                _accountListViewController = null;
            }
            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
            _accountListViewController = storyBoard.InstantiateViewController("AccountListViewController") as AccountListViewController;
            _accountListViewController._homeViewController = this;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            UpdateGreeting(GetGreeting());
            if (DataManager.DataManager.SharedInstance.SummaryNeedsRefresh)
            {
                DataManager.DataManager.SharedInstance.ServicesList = new List<ServiceItemModel>();
                DataManager.DataManager.SharedInstance.HelpList = new List<HelpModel>();
                if (_accountListViewController != null)
                {
                    DataManager.DataManager.SharedInstance.AccountListIsLoaded = false;
                    AmountDueCache.Reset();
                    _accountListViewController.PrepareAccountList(null, false, true);
                }
                DataManager.DataManager.SharedInstance.SummaryNeedsRefresh = false;
            }
            //SSMRAccounts.SetEligibleAccounts();
            OnLoadHomeData();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (IsRearrangeSaved)
            {
                OnRearrangeSuccess(RearrangeSuccessMsg);
                IsRearrangeSaved = false;
            }
            bool isBRCRMAvailable = DataManager.DataManager.SharedInstance.IsBcrmAvailable;
            if (!isBRCRMAvailable && !AppLaunchMasterCache.IsBCRMPopupDisplayed)
            {
                _isBCRMPopupDisplayed = true;
                DowntimeDataModel status = DataManager.DataManager.SharedInstance.SystemStatus?.Find(x => x.SystemType == Enums.SystemEnum.BCRM);
                string errMsg = GetErrorI18NValue(Constants.Error_DefaultServiceErrorMessage);
                string errorTitle = GetCommonI18NValue(Constants.Common_WellBeBack);
                if (status != null)
                {
                    if (!string.IsNullOrEmpty(status?.DowntimeMessage) && !string.IsNullOrWhiteSpace(status?.DowntimeMessage))
                    {
                        errMsg = status.DowntimeMessage;
                    }
                    if (!string.IsNullOrEmpty(status?.DowntimeTextMessage) && !string.IsNullOrWhiteSpace(status?.DowntimeTextMessage))
                    {
                        errorTitle = status.DowntimeTextMessage;
                    }
                }

                DisplayCustomAlert(errorTitle, errMsg
                    , new Dictionary<string, Action> { { GetCommonI18NValue(Constants.Common_GotIt), ()=> {
                        AppLaunchMasterCache.IsBCRMPopupDisplayed = true;
                        _isBCRMPopupDisplayed = false;
                        CheckTutorialOverlay();
                    }}}
                    , UIImage.FromBundle(DashboardHomeConstants.IMG_BCRMDownPopup));
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        public void UpdateGreeting(string greeting)
        {
            if (_dashboardHomeHeader != null)
            {
                _dashboardHomeHeader.UpdateGreetingText(greeting);
            }
        }

        public override void SetStatusBarNoOverlap()
        {
            base.SetStatusBarNoOverlap();
            _statusBarView.BackgroundColor = MyTNBColor.ClearBlue;
            _statusBarView.Hidden = true;
        }

        #region Tutorial Overlay Methods
        public void CheckTutorialOverlay()
        {
            if (_isBCRMPopupDisplayed) { return; }
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var tutorialOverlayHasShown = sharedPreference.BoolForKey(DashboardHomeConstants.Pref_TutorialOverlay);

            if (tutorialOverlayHasShown) { return; }

            if (!_accountListIsShimmering && !_servicesIsShimmering && !_isGetServicesFailed && !_helpIsShimmering)
            {
                InvokeOnMainThread(() =>
                {
                    var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                    var topVc = AppDelegate.GetTopViewController(baseRootVc);
                    if (topVc != null)
                    {
                        if (topVc is DashboardHomeViewController)
                        {
                            ShowTutorialOverlay();
                        }
                        else
                        {
                            if (_tutorialContainer != null)
                            {
                                _tutorialContainer.RemoveFromSuperview();
                            }
                        }
                    }
                });
            }
        }

        private void ShowTutorialOverlay()
        {
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;

            if (_tutorialContainer != null && _tutorialContainer.IsDescendantOfView(currentWindow)) { return; }

            ScrollTableToTheTop();
            ResetTableView();
            nfloat width = currentWindow.Frame.Width;
            nfloat height = currentWindow.Frame.Height;

            _tutorialContainer = new UIView(new CGRect(0, 0, width, height))
            {
                BackgroundColor = UIColor.Clear,
                Tag = 1001
            };

            HomeTutorialEnum tutorialType;

            if (!_dashboardHomeHelper.HasAccounts)
            {
                tutorialType = HomeTutorialEnum.NOACCOUNT;
            }
            else if (_dashboardHomeHelper.HasMoreThanThreeAccts)
            {
                tutorialType = HomeTutorialEnum.MORETHANTHREEACCOUNTS;
            }
            else
            {
                tutorialType = HomeTutorialEnum.LESSTHANFOURACCOUNTS;
            }

            HomeTutorialOverlay tutorialView = new HomeTutorialOverlay(_tutorialContainer, this)
            {
                TutorialType = tutorialType,
                OnDismissAction = HideTutorialOverlay,
                ScrollTableToTheTop = ScrollTableToTheTop,
                ScrollTableToTheBottom = ScrollTableToTheBottom,
                GetI18NValue = GetI18NValue
            };
            var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            var topVc = AppDelegate.GetTopViewController(baseRootVc);
            if (topVc != null)
            {
                if (topVc is DashboardHomeViewController && _tutorialContainer != null && !_tutorialContainer.IsDescendantOfView(currentWindow))
                {
                    foreach (UIView view in currentWindow.Subviews)
                    {
                        if (view.Tag == 1001)
                        {
                            view.RemoveFromSuperview();
                            break;
                        }
                    }

                    _tutorialContainer.AddSubview(tutorialView.GetView());
                    currentWindow.AddSubview(_tutorialContainer);
                }
                else
                {
                    if (_tutorialContainer != null)
                    {
                        _tutorialContainer.RemoveFromSuperview();
                    }
                }
            }
        }

        private void HideTutorialOverlay()
        {
            ScrollTableToTheTop();
            if (_tutorialContainer != null)
            {
                _tutorialContainer.Alpha = 1F;
                _tutorialContainer.Transform = CGAffineTransform.MakeIdentity();
                UIView.Animate(0.3, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
                {
                    _tutorialContainer.Alpha = 0F;
                }, _tutorialContainer.RemoveFromSuperview);

                var sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetBool(true, DashboardHomeConstants.Pref_TutorialOverlay);
                sharedPreference.Synchronize();
            }
        }

        public void ScrollTableToTheBottom()
        {
            _homeTableView.ScrollToRow(NSIndexPath.Create(0, DashboardHomeConstants.CellIndex_Help), UITableViewScrollPosition.Bottom, false);
        }

        public void ScrollTableToTheTop()
        {
            _homeTableView.SetContentOffset(new CGPoint(0, 0), false);
        }

        private void ResetTableView()
        {
            if (DataManager.DataManager.SharedInstance.ActiveAccountList.Count <= 3)
                return;

            DataManager.DataManager.SharedInstance.AccountListIsLoaded = false;
            OnUpdateCellWithoutReload(DashboardHomeConstants.CellIndex_Services);
            if (_accountListViewController != null)
            {
                _accountListViewController.PrepareAccountList(DataManager.DataManager.SharedInstance.CurrentAccountList);
            }
        }
        #endregion

        #region Observer Methods
        private void NotificationDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> SUMMARY DASHBOARD NotificationDidChange");
            if (_dashboardHomeHeader != null)
            {
                _dashboardHomeHeader.SetNotificationImage(PushNotificationHelper.GetNotificationImage());
                _dashboardHomeHeader.BadgeValue = PushNotificationHelper.GetNotificationCount();
            }
            PushNotificationHelper.UpdateApplicationBadge();
        }

        protected override void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> Home LanguageDidChange");
            base.LanguageDidChange(notification);
            if (_homeTableView != null && _dashboardHomeHeader != null)
            {
                string greeting = GetGreeting();
                UILabel lblGreeting = _homeTableView.TableHeaderView.ViewWithTag(9001) as UILabel;
                if (lblGreeting != null)
                {
                    lblGreeting.Text = greeting;
                }
            }
            if (_accountListViewController != null)
            {
                DataManager.DataManager.SharedInstance.AccountListIsLoaded = false;
                AmountDueCache.Reset();
                _accountListViewController.PrepareAccountList();
            }
            DataManager.DataManager.SharedInstance.ServicesList = null;
            DataManager.DataManager.SharedInstance.HelpList = null;
            if (DataManager.DataManager.SharedInstance.IsLoggedIn())
            {
                OnLoadHomeData();
            }
        }

        private void OnEnterForeground(NSNotification notification)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                if (NetworkUtility.isReachable)
                {
                    InvokeOnMainThread(() =>
                    {
                        var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                        var topVc = AppDelegate.GetTopViewController(baseRootVc);
                        if (topVc != null)
                        {
                            if (topVc is DashboardHomeViewController)
                            {
                                UpdateGreeting(GetGreeting());
                                OnChangeStatusBarFrame(null);
                                if (_accountListViewController != null)
                                {
                                    DataManager.DataManager.SharedInstance.AccountListIsLoaded = false;
                                    AmountDueCache.Reset();
                                    _accountListViewController.PrepareAccountList();
                                }
                                OnLoadHomeData();
                            }
                        }
                    });
                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        DisplayNoDataAlert();
                    });
                }
            });
        }

        private void OnChangeStatusBarFrame(NSNotification notification)
        {
            if (DeviceHelper.IsIphoneXUpResolution())
                return;

            nfloat yPos = DeviceHelper.GetStatusBarHeight();
            if (DeviceHelper.GetStatusBarHeight() > 20)
            {
                yPos = 0;
            }

            ViewHelper.AdjustFrameSetY(_homeTableView, yPos);
            UpdateFooterBG();
        }
        #endregion
        private void OnLoadHomeData()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                if (NetworkUtility.isReachable)
                {
                    if (DataManager.DataManager.SharedInstance.ServicesList == null ||
                        DataManager.DataManager.SharedInstance.ServicesList.Count == 0)
                    {
                        _services = new ServicesResponseModel();
                        OnGetServices();
                    }
                    else
                    {
                        _servicesIsShimmering = false;
                        CheckTutorialOverlay();
                    }

                    if (DataManager.DataManager.SharedInstance.HelpList == null ||
                        DataManager.DataManager.SharedInstance.HelpList.Count == 0)
                    {
                        _helpList = new List<HelpModel>();
                        InvokeOnMainThread(() =>
                        {
                            _helpIsShimmering = true;
                            OnUpdateTable();
                            SitecoreServices.Instance.LoadNeedHelp().ContinueWith(task =>
                            {
                                InvokeOnMainThread(() =>
                                {
                                    _helpList = new HelpEntity().GetAllItems();
                                    DataManager.DataManager.SharedInstance.HelpList = _helpList;
                                    _helpIsShimmering = false;
                                    OnUpdateTable();
                                    CheckTutorialOverlay();
                                });
                            });
                        });
                    }
                    else
                    {
                        _helpIsShimmering = false;
                        CheckTutorialOverlay();
                    }
                    OnUpdateNotification();
                }
                else
                {
                    DisplayNoDataAlert();
                }
            });
        }

        // <summary>
        // Initializes the table view.
        // </summary>
        private void InitializeTableView()
        {
            _homeTableView.Source = new DashboardHomeDataSource(this, _accountListViewController
                , DataManager.DataManager.SharedInstance.ServicesList, _promotions, _helpList
                , _servicesIsShimmering, _helpIsShimmering, _isRefreshScreenEnabled, _refreshScreenComponent
                , OnReload, OnServicesRefresh);
            _homeTableView.ReloadData();
            UpdateFooterBG();
        }

        private void AddTableView()
        {
            nfloat yPos = DeviceHelper.GetStatusBarHeight();
            nfloat addtlHeight = 0;
            if (DeviceHelper.GetStatusBarHeight() > 20 && !DeviceHelper.IsIphoneXUpResolution())
            {
                yPos = 0;
                addtlHeight = DeviceHelper.GetStatusBarHeight();
            }
            _homeTableView = new UITableView(new CGRect(0, yPos
                , ViewWidth, ViewHeight + addtlHeight))
            { BackgroundColor = UIColor.Clear };
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
            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(navController, true, null);
        }

        public void OnAddAccountAction()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("AccountRecords", null);
            var viewController = storyBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
            viewController.isDashboardFlow = true;
            viewController._needsUpdate = true;
            UINavigationController navController = new UINavigationController(viewController);
            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(navController, true, null);
        }

        public void OnRearrangeAccountAction()
        {
            RearrangeAccountViewController rearrangeAccountView = new RearrangeAccountViewController(this);
            UINavigationController navController = new UINavigationController(rearrangeAccountView);
            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(navController, true, null);
        }

        private void OnRearrangeSuccess(string message)
        {
            DisplayToast(message);
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
                await PushNotificationHelper.GetNotifications(false);
                InvokeOnMainThread(() =>
                {
                    PushNotificationHelper.UpdateApplicationBadge();
                    NotifCenterUtility.PostNotificationName("OnReceiveNotificationFromDashboard", new NSObject());
                });
            });
        }

        internal void OnTableViewScroll(UIScrollView scrollView)
        {
            nfloat scrollDiff = scrollView.ContentOffset.Y - _previousScrollOffset;
            CGRect frame = _footerImageBG.Frame;
            ViewHelper.AdjustFrameSetY(_footerImageBG, frame.Y - scrollDiff);
            _previousScrollOffset = scrollView.ContentOffset.Y;
        }

        private void OnGetServices()
        {
            InvokeOnMainThread(() =>
            {
                DataManager.DataManager.SharedInstance.ActiveServicesList = new List<ServiceItemModel>();
                _servicesIsShimmering = true;
                OnUpdateTable();
                bool hasExistingSSMR = false;
                InvokeInBackground(async () =>
                {
                    if (!AppLaunchMasterCache.IsSMRFeatureDisabled)
                    {
                        List<string> contractAccounts = _dashboardHomeHelper.GetOwnedAccountsList(DataManager.DataManager.SharedInstance.AccountRecordsList.d);
                        if (contractAccounts != null && contractAccounts.Count > 0)
                        {
                            SMRAccountStatusResponseModel response = await ServiceCall.GetAccountsSMRStatus(contractAccounts);
                            InvokeOnMainThread(() =>
                            {
                                if (response != null &&
                                    response.d != null &&
                                    response.d.IsSuccess &&
                                    response.d.data != null &&
                                    response.d.data.Count > 0)
                                {
                                    List<SMRAccountStatusModel> smrList = new List<SMRAccountStatusModel>(response.d.data);
                                    if (smrList != null && smrList.Count > 0)
                                    {
                                        foreach (var item in smrList)
                                        {
                                            DataManager.DataManager.SharedInstance.UpdateDueIsSSMR(item.ContractAccount, item.IsTaggedSMR);
                                            if (item.isTaggedSMR)
                                            {
                                                hasExistingSSMR = true;
                                            }
                                        }
                                    }
                                }
                            });
                        }
                        List<Task> taskList = new List<Task>();
                        try
                        {
                            taskList.Add(GetServices());
                            if (!hasExistingSSMR && contractAccounts != null && contractAccounts.Count > 0)
                            {
                                taskList.Add(BatchCallForSSMRApplyAllowed());
                            }
                            Task.WaitAll(taskList.ToArray());
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("Error in services: " + e.Message);
                        }
                        InvokeOnMainThread(() =>
                        {
                            _servicesIsShimmering = false;
                            if (_services != null &&
                                _services.d != null &&
                                _services.d.IsSuccess &&
                                _services.d.data != null)
                            {
                                List<ServiceItemModel> services = new List<ServiceItemModel>(_services.d.data.services);
                                if (!hasExistingSSMR && contractAccounts != null && contractAccounts.Count > 0)
                                {
                                    if (_isSMRApplyAllowedResponse != null &&
                                        _isSMRApplyAllowedResponse.d != null &&
                                        _isSMRApplyAllowedResponse.d.IsSuccess &&
                                        _isSMRApplyAllowedResponse.d.data != null &&
                                        _isSMRApplyAllowedResponse.d.data.Count > 0)
                                    {
                                        if (!_isSMRApplyAllowedResponse.d.data[0].AllowApply)
                                        {
                                            services.RemoveAt(ServiceItemIndexToRemove(services));
                                        }
                                    }
                                    else
                                    {
                                        services.RemoveAt(ServiceItemIndexToRemove(services));
                                    }
                                }
                                else if (!hasExistingSSMR)
                                {
                                    services.RemoveAt(ServiceItemIndexToRemove(services));
                                }
                                DataManager.DataManager.SharedInstance.ServicesList = services;
                                _isGetServicesFailed = false;
                            }
                            else
                            {
                                DataManager.DataManager.SharedInstance.ServicesList?.Clear();
                                _isGetServicesFailed = true;
                            }
                            OnUpdateTable();
                            CheckTutorialOverlay();
                        });
                    }
                    else
                    {
                        await GetServices();
                        InvokeOnMainThread(() =>
                        {
                            _servicesIsShimmering = false;
                            if (_services != null &&
                                _services.d != null &&
                                _services.d.IsSuccess &&
                                _services.d.data != null)
                            {
                                List<ServiceItemModel> services = new List<ServiceItemModel>(_services.d.data.services);
                                services.RemoveAt(ServiceItemIndexToRemove(services));
                                DataManager.DataManager.SharedInstance.ServicesList = services;
                                _isGetServicesFailed = false;
                            }
                            else
                            {
                                DataManager.DataManager.SharedInstance.ServicesList?.Clear();
                                _isGetServicesFailed = true;
                            }
                            OnUpdateTable();
                            CheckTutorialOverlay();
                        });
                    }
                });
            });
        }

        private Task OnGetHelpInfo()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , string.Empty, TNBGlobal.SITECORE_URL, TNBGlobal.APP_LANGUAGE);
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
                await SitecoreServices.Instance.LoadPromotions();
                PromotionsEntity entity = new PromotionsEntity();
                _promotions = entity.GetAllItemsV2();
                InvokeOnMainThread(() =>
                {
                    OnUpdateTable();
                });
            });
        }

        private async Task<ServicesResponseModel> GetServices()
        {
            ServiceManager serviceManager = new ServiceManager();
            object request = new { serviceManager.usrInf };
            ServicesResponseModel response = serviceManager.OnExecuteAPIV6<ServicesResponseModel>("GetServices", request);
            _services = response;
            return response;
        }

        private async Task<GetIsSmrApplyAllowedResponseModel> GetIsSmrApplyAllowed(List<string> accounts)
        {
            ServiceManager serviceManager = new ServiceManager();
            List<string> contractAccounts = accounts;
            object request = new { serviceManager.usrInf, contractAccounts };
            GetIsSmrApplyAllowedResponseModel response = serviceManager.OnExecuteAPIV6<GetIsSmrApplyAllowedResponseModel>("GetIsSmrApplyAllowed", request);
            return response;
        }

        private async Task<bool> BatchCallForSSMRApplyAllowed()
        {
            SSMRAccounts.SetFilteredEligibleAccounts();
            List<string> allAccounts = SSMRAccounts.GetFilteredAccountNumberList();

            bool res = false;
            if (allAccounts != null)
            {
                if (allAccounts.Count > 5)
                {
                    var batchNoMax = Math.Ceiling((double)allAccounts.Count / 5);
                    for (int batchNo = 0; batchNo < batchNoMax; batchNo++)
                    {
                        List<string> batchAccounts = GetBatchAccounts(allAccounts, batchNo);
                        if (!IsShowApplySSMR() && batchAccounts != null && batchAccounts.Count > 0)
                        {
                            _isSMRApplyAllowedResponse = await GetIsSmrApplyAllowed(batchAccounts);
                        }
                    }
                    res = true;
                }
                else if (allAccounts.Count > 0)
                {
                    _isSMRApplyAllowedResponse = await GetIsSmrApplyAllowed(allAccounts);
                    res = true;
                }
            }
            return res;
        }

        private List<string> GetBatchAccounts(List<string> allAccts, int batchNo)
        {
            List<string> batchAccounts = new List<string>();
            int maxLimit = 5;
            int indx = batchNo * maxLimit;

            for (; indx < allAccts.Count; indx++)
            {
                if (batchAccounts.Count < maxLimit)
                {
                    batchAccounts.Add(allAccts[indx]);
                }
            }
            return batchAccounts;
        }

        private bool IsShowApplySSMR()
        {
            bool res = false;
            if (_isSMRApplyAllowedResponse != null &&
                _isSMRApplyAllowedResponse.d != null &&
                _isSMRApplyAllowedResponse.d.IsSuccess &&
                _isSMRApplyAllowedResponse.d.data != null &&
                _isSMRApplyAllowedResponse.d.data.Count > 0)
            {
                res = _isSMRApplyAllowedResponse.d.data[0].AllowApply;
            }
            return res;
        }

        private int ServiceItemIndexToRemove(List<ServiceItemModel> services)
        {
            int index = -1;
            if (services != null && services.Count > 0)
            {
                var indx = services.FindIndex(x => x.ServiceType == ServiceEnum.SELFMETERREADING);

                if (indx > -1 && indx < services.Count)
                {
                    index = indx;
                }
            }
            return index;
        }

        private void NavigateToUsageView()
        {
            if (_usageStoryBoard != null)
            {
                UsageViewController viewController = _usageStoryBoard.InstantiateViewController("UsageViewController") as UsageViewController;
                if (viewController != null)
                {
                    NavigationController.PushViewController(viewController, true);
                }
            }
            ActivityIndicator.Hide();
        }

        public void OnAccountCardSelected(DueAmountDataModel model)
        {
            var index = DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.FindIndex(x => x.accNum == model.accNum) ?? -1;
            if (index > -1)
            {
                var selected = DataManager.DataManager.SharedInstance.AccountRecordsList.d[index];
                DataManager.DataManager.SharedInstance.SelectAccount(selected.accNum);
                DataManager.DataManager.SharedInstance.IsSameAccount = false;
                DataManager.DataManager.SharedInstance.AccountIsSSMR = _dashboardHomeHelper.IsSSMR(DataManager.DataManager.SharedInstance.SelectedAccount);
                NavigateToUsageView();
            }
        }

        private void SetActionsDictionary()
        {
            DashboardHomeActions actions = new DashboardHomeActions(this);
            _servicesActionDictionary = actions.GetActionsDictionary();
        }

        public void OnUpdateTable()
        {
            _homeTableView.Source = new DashboardHomeDataSource(this, _accountListViewController
                , DataManager.DataManager.SharedInstance.ServicesList, _promotions, _helpList
                , _servicesIsShimmering, _helpIsShimmering, _isRefreshScreenEnabled, _refreshScreenComponent
                , OnReload, OnServicesRefresh);
            _homeTableView.ReloadData();
            UpdateFooterBG();
        }

        internal void OnReload()
        {
            _homeTableView.ReloadData();
            UpdateFooterBG();
        }

        private void OnServicesRefresh()
        {
            Debug.WriteLine("OnServicesRefresh");
            _servicesIsShimmering = true;
            _isGetServicesFailed = false;
            _homeTableView.Source = new DashboardHomeDataSource(this, _accountListViewController
                , DataManager.DataManager.SharedInstance.ServicesList, _promotions, _helpList
                , _servicesIsShimmering, _helpIsShimmering, _isRefreshScreenEnabled, _refreshScreenComponent
                , OnReload, OnServicesRefresh);
            _homeTableView.ReloadData();
            OnGetServices();
        }

        public void OnUpdateCellWithoutReload(int row)
        {
            _homeTableView.BeginUpdates();
            NSIndexPath indexPath = NSIndexPath.Create(0, row);
            _homeTableView.ReloadRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.None);
            _homeTableView.EndUpdates();
            UpdateFooterBG();
        }

        private void UpdateFooterBG()
        {
            nfloat yPosBG = 0;
            if (DeviceHelper.GetStatusBarHeight() > 20 && !DeviceHelper.IsIphoneXUpResolution())
            {
                yPosBG -= 20;
            }
            CGRect servicesCellRect = _homeTableView.RectForRowAtIndexPath(NSIndexPath.Create(0, DashboardHomeConstants.CellIndex_Services));
            ViewHelper.AdjustFrameSetY(_footerImageBG, DeviceHelper.GetStatusBarHeight() + servicesCellRect.Y + (servicesCellRect.Height * 0.40F) - _previousScrollOffset + yPosBG);
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
                    var bcrm = DataManager.DataManager.SharedInstance.SystemStatus?.Find(x => x.SystemType == Enums.SystemEnum.BCRM);
                    var bcrmMsg = bcrm?.DowntimeMessage ?? GetCommonI18NValue(Constants.Common_BCRMMessage);
                    string desc = _isBCRMAvailable ? model?.RefreshMessage ?? string.Empty : bcrmMsg;

                    _refreshScreenComponent = new RefreshScreenComponent(View, GetScaledHeight(24f));
                    _refreshScreenComponent.SetIsBCRMDown(!_isBCRMAvailable);
                    _refreshScreenComponent.SetRefreshButtonHidden(!_isBCRMAvailable);
                    _refreshScreenComponent.SetButtonText(model?.RefreshBtnText ?? string.Empty);
                    _refreshScreenComponent.SetDescription(desc);
                    _refreshScreenComponent.CreateComponent();
                    _refreshScreenComponent.OnButtonTap = RefreshViewForAccounts;

                    _homeTableView.BeginUpdates();
                    _homeTableView.Source = new DashboardHomeDataSource(this, null, DataManager.DataManager.SharedInstance.ServicesList
                        , _promotions, _helpList, _servicesIsShimmering, _helpIsShimmering, _isRefreshScreenEnabled, _refreshScreenComponent
                        , OnReload, OnServicesRefresh);
                    NSIndexPath indexPath = NSIndexPath.Create(0, 0);
                    _homeTableView.ReloadRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.None);
                    _homeTableView.EndUpdates();
                    UpdateFooterBG();

                    if (_accountListViewController != null)
                    {
                        _accountListViewController.View.RemoveFromSuperview();
                        _accountListViewController = null;
                    }
                }
            });
        }

        private void RefreshViewForAccounts()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                if (NetworkUtility.isReachable)
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
                        _isRefreshScreenEnabled = false;
                        SetAccountListViewController();
                        if (_accountListViewController != null)
                        {
                            DataManager.DataManager.SharedInstance.AccountListIsLoaded = false;
                            AmountDueCache.Reset();
                            _accountListViewController.PrepareAccountList(null, false, true);
                        }
                        OnUpdateTable();
                    });
                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        DisplayNoDataAlert();
                    });
                }
            });
        }

        public void DismissActiveKeyboard()
        {
            if (_accountListViewController != null)
            {
                _accountListViewController.DismissActiveKeyboard();
            }
        }
    }
}
