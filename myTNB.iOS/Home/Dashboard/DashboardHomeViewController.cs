using CoreGraphics;
using Foundation;
using myTNB.Dashboard;
using myTNB.Dashboard.DashboardComponents;
using myTNB.DataManager;
using myTNB.Enums;
using myTNB.Model;
using myTNB.PushNotification;
using myTNB.Registration.CustomerAccounts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UIKit;

namespace myTNB
{
    public partial class DashboardHomeViewController : CustomUIViewController
    {
        GradientViewComponent _gradientViewComponent;
        GreetingComponent _greetingComponent;
        TitleBarComponent _titleBarComponent;
        SystemDownComponent _sysDownComponent;
        UIView _gradientView, _greetingView, _sysDownView, _viewHeader
            , _viewFooter, _viewLoadMore;
        UIButton btnAdd;
        UILabel _lblLoadMore;
        Dictionary<string, List<DueAmountDataModel>> displayedAccounts = new Dictionary<string, List<DueAmountDataModel>>();
        int loadedAccountsCount;

        const int MaxAccountsPerCall = 5;

        int verticalMargin = 24;
        nfloat addHeight = 44.0f;
        nfloat addAccountHeight;
        nfloat headerHeight;
        nfloat maxHeaderHeight = 128f;
        nfloat minHeaderHeight = 0.1f;
        nfloat previousScrollOffset;
        bool isViewDidLoad = false;
        bool isAnimating = false;
        bool isBcrmAvailable = true;
        bool isTimeOut = false;
        List<string> accountsToRefresh;
        bool isRefreshing = false;

        public DashboardHomeViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Initialize();
            SetEvents();
            isViewDidLoad = true;
            DataManager.DataManager.SharedInstance.SummaryNeedsRefresh = true;
            LoadContents();
            NSNotificationCenter.DefaultCenter.AddObserver((NSString)"LanguageDidChange", LanguageDidChange);
            NSNotificationCenter.DefaultCenter.AddObserver((NSString)"NotificationDidChange", NotificationDidChange);
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillEnterForegroundNotification, HandleAppWillEnterForeground);
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        DataManager.DataManager.SharedInstance.IsLoadingFromDashboard = true;
                        await PushNotificationHelper.GetNotifications();
                        UpdateNotificationIcon();
                        NSNotificationCenter.DefaultCenter.PostNotificationName("OnReceiveNotificationFromDashboard", new NSObject());
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        public void NotificationDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> SUMMARY DASHBOARD NotificationDidChange");
            _titleBarComponent?.SetPrimaryImage(PushNotificationHelper.GetNotificationImage());
            PushNotificationHelper.UpdateApplicationBadge();
        }

        public void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> SUMMARY DASHBOARD LanguageDidChange");
            _titleBarComponent.SetTitle("Dashboard_AllAccounts".Translate());
            _lblLoadMore.Text = "Dashboard_LoadMoreAccounts".Translate();
            btnAdd.SetTitle("Common_AddAnotherAccount".Translate(), UIControlState.Normal);
            DataManager.DataManager.SharedInstance.SummaryNeedsRefresh = true;
        }

        private void HandleAppWillEnterForeground(NSNotification notification)
        {
            if (DataManager.DataManager.SharedInstance.IsLoggedIn())
            {
                //RefreshScreen();
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            isBcrmAvailable = DataManager.DataManager.SharedInstance.IsBcrmAvailable;
            UpdateHeader();
            UpdateNotificationIcon();

            if (isViewDidLoad)
            {
                ShowToast();
                isViewDidLoad = false;
            }
            else
            {
                LoadContents(true);
            }

        }
        /// <summary>
        /// Updates the notification icon.
        /// </summary>
        private void UpdateNotificationIcon()
        {
            _titleBarComponent?.SetPrimaryImage(
                    DataManager.DataManager.SharedInstance.HasNewNotification ? "Notification-New" : "Notification");
        }

        private void Initialize()
        {
            loadedAccountsCount = 0;

            var percentage = 1.0f;
            _gradientViewComponent = new GradientViewComponent(View, percentage);
            _gradientView = _gradientViewComponent.GetUI();

            _titleBarComponent = new TitleBarComponent(_gradientView);
            UIView titleBarView = _titleBarComponent.GetUI();
            _titleBarComponent.SetTitle("Dashboard_AllAccounts".Translate());
            _titleBarComponent.SetPrimaryVisibility(false);

            _gradientView.AddSubview(titleBarView);

            View.AddSubview(_gradientView);
            var tabBarHeight = TabBarController.TabBar.Frame.Height;
            var tbvHeight = View.Bounds.Height - titleBarView.Frame.GetMaxY() - tabBarHeight;

            if (DeviceHelper.IsIphoneXUpResolution())
            {
                tbvHeight -= 35.0f;
            }

            tableViewAccounts.Frame = new CGRect(0, titleBarView.Frame.GetMaxY() + 1
                , View.Bounds.Width, tbvHeight);
            tableViewAccounts.RowHeight = UITableView.AutomaticDimension;
            tableViewAccounts.EstimatedRowHeight = 66;
            tableViewAccounts.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableViewAccounts.Bounces = false;
            tableViewAccounts.SectionFooterHeight = 0;
            View.BringSubviewToFront(tableViewAccounts);

            AddHeader();
            AddFooter();
        }

        private void SetEvents()
        {
            if (_titleBarComponent != null)
            {
                UITapGestureRecognizer notificationTap = new UITapGestureRecognizer(() =>
                {
                    UIStoryboard storyBoard = UIStoryboard.FromName("PushNotification", null);
                    var viewController = storyBoard.InstantiateViewController("PushNotificationViewController") as PushNotificationViewController;
                    var navController = new UINavigationController(viewController);
                    PresentViewController(navController, true, null);
                });
                _titleBarComponent.SetPrimaryAction(notificationTap);
            }
        }

        /// <summary>
        /// Loads the contents.
        /// </summary>
        /// <param name="fromWillAppear">If set to <c>true</c> from will appear.</param>
        private void LoadContents(bool fromWillAppear = false, bool pullDown = false)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        if (DataManager.DataManager.SharedInstance.SummaryNeedsRefresh)
                        {
                            loadedAccountsCount = 0;
                            displayedAccounts.Clear();
                            DataManager.DataManager.SharedInstance.SummaryNeedsRefresh = false;
                        }

                        int removedCount = await UpdateDues();
                        // limit if displayed accounts is greater than max per call
                        int accountsToAdd = (loadedAccountsCount < MaxAccountsPerCall) ? 0
                            : Math.Max(DataManager.DataManager.SharedInstance.AccountsAddedCount, removedCount);

                        if (!fromWillAppear || (fromWillAppear && (loadedAccountsCount < MaxAccountsPerCall || accountsToAdd > 0)))
                        {
                            await LoadDues(accountsToAdd, pullDown);
                            if (DataManager.DataManager.SharedInstance.AccountsAddedCount > 0)
                            {
                                DataManager.DataManager.SharedInstance.AccountsAddedCount = 0;
                            }
                        }
                    }
                });
            });
        }

        /// <summary>
        /// Adds the header.
        /// </summary>
        private void AddHeader()
        {
            _viewHeader = new UIView
            {
                ClipsToBounds = true
            };

            _greetingComponent = new GreetingComponent(tableViewAccounts);
            _greetingView = _greetingComponent.GetUI();
            _greetingComponent.OnRefresh = OnRefresh;

            _sysDownComponent = new SystemDownComponent(tableViewAccounts, true);
            _sysDownView = _sysDownComponent.GetUI();
        }

        /// <summary>
        /// Updates the header.
        /// </summary>
        private void UpdateHeader(BaseModel baseModelResponse = null)
        {
            ViewHelper.RemoveAllSubviews(_viewHeader);

            if (isBcrmAvailable)
            {
                _greetingView = _greetingComponent.GetUI(isTimeOut, baseModelResponse);
                SetGreeting();
                maxHeaderHeight = _greetingView.Frame.Height + 1f;
                _viewHeader.Frame = new CGRect(0, 0, _greetingView.Frame.Width, _greetingView.Frame.Height + 1f);
                _viewHeader.AddSubview(_greetingView);
            }
            else
            {
                _sysDownComponent = new SystemDownComponent(tableViewAccounts, true);
                _sysDownView = _sysDownComponent.GetUI();
                maxHeaderHeight = _sysDownView.Frame.Height + 1f;
                _viewHeader.Frame = new CGRect(0, 0, _sysDownView.Frame.Width, _sysDownView.Frame.Height + 1f);
                _viewHeader.AddSubview(_sysDownView);
            }

            headerHeight = maxHeaderHeight;
            tableViewAccounts.TableHeaderView = _viewHeader;
        }

        /// <summary>
        /// Sets the greeting.
        /// </summary>
        private void SetGreeting()
        {
            GreetingMode textMode = default(GreetingMode);
            GreetingMode imageMode = default(GreetingMode);

            var now = DateTime.Now;

            if (now.Hour < 6)
            {
                textMode = GreetingMode.Morning;
                imageMode = GreetingMode.Evening;
            }
            else if (now.Hour < 12)
            {
                textMode = GreetingMode.Morning;
                imageMode = GreetingMode.Morning;
            }
            else if (now.Hour < 18)
            {
                textMode = GreetingMode.Afternoon;
                imageMode = GreetingMode.Afternoon;
            }
            else
            {
                textMode = GreetingMode.Evening;
                imageMode = GreetingMode.Evening;
            }

            var displayName = string.Empty;
            if (DataManager.DataManager.SharedInstance.UserEntity?.Count > 0)
            {
                displayName = DataManager.DataManager.SharedInstance.UserEntity[0]?.displayName;
            }
            _greetingComponent?.SetMode(textMode, imageMode, displayName);

        }

        /// <summary>
        /// Adds the footer.
        /// </summary>
        private void AddFooter()
        {
            int horizontalMargin = 18;
            nfloat footerHeight = 0f;
            _viewFooter = new UIView();

            UIView viewFooterLine = new UIView
            {
                Frame = new CGRect(0, 0, tableViewAccounts.Frame.Width, 1),
                BackgroundColor = UIColor.FromWhiteAlpha(1, 0.2f)
            };

            // load more
            _lblLoadMore = new UILabel(new CGRect(0, 0, tableViewAccounts.Frame.Width, 64))
            {
                Text = "Dashboard_LoadMoreAccounts".Translate(),
                TextColor = UIColor.White,
                Font = MyTNBFont.MuseoSans14_300,
                TextAlignment = UITextAlignment.Center
            };

            UIView viewLine = new UIView
            {
                Frame = new CGRect(0, 64, tableViewAccounts.Frame.Width, 1),
                BackgroundColor = UIColor.FromWhiteAlpha(1, 0.2f)
            };

            _viewLoadMore = new UIView(new CGRect(0, 1, tableViewAccounts.Frame.Width, 65));
            _viewLoadMore.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                OnLoadMore();
            }));
            _viewLoadMore.AddSubviews(new UIView[] { _lblLoadMore, viewLine });
            _viewLoadMore.Hidden = true;
            _viewFooter.AddSubview(_viewLoadMore);

            // add account button

            btnAdd = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(horizontalMargin, footerHeight + verticalMargin, tableViewAccounts.Frame.Width - horizontalMargin * 2, addHeight)
            };
            btnAdd.Layer.CornerRadius = 4;
            btnAdd.Layer.BorderColor = UIColor.White.CGColor;
            btnAdd.BackgroundColor = UIColor.Clear;
            btnAdd.Layer.BorderWidth = 1;
            btnAdd.SetTitle("Common_AddAnotherAccount".Translate(), UIControlState.Normal);
            btnAdd.Font = MyTNBFont.MuseoSans16_300;
            btnAdd.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnAdd.TouchUpInside += (sender, e) =>
            {
               /* UIStoryboard storyBoard = UIStoryboard.FromName("AccountRecords", null);
                var viewController = storyBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
                if (viewController != null)
                {
                    viewController.isDashboardFlow = true;
                    viewController._needsUpdate = true;
                    var navController = new UINavigationController(viewController);
                    PresentViewController(navController, true, null);
                }*/

                UIStoryboard onboardingStoryboard = UIStoryboard.FromName("Onboarding", null);
                GenericPageRootViewController onboardingVC = onboardingStoryboard.InstantiateViewController("GenericPageRootViewController") as GenericPageRootViewController;
                onboardingVC.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
                onboardingVC.PageType = GenericPageViewEnum.Type.SSMR;
                PresentViewController(onboardingVC, true, null);
            };

            _viewFooter.AddSubview(btnAdd);

            addAccountHeight = (addHeight + verticalMargin * 2);
            footerHeight += addAccountHeight;
            _viewFooter.Frame = new CGRect(0, 0, tableViewAccounts.Frame.Width, footerHeight);

            tableViewAccounts.TableFooterView = _viewFooter;
        }

        /// <summary>
        /// Handles the table view accounts scrolled event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event args.</param>
        public void OnTableViewAccountsScrolled(object sender, EventArgs e)
        {
            var scrollDiff = tableViewAccounts.ContentOffset.Y - previousScrollOffset;
            var isScrollingDown = scrollDiff > 0;
            var isScrollingUp = scrollDiff < 0;

            var newHeight = headerHeight;

            if (tableViewAccounts.ContentOffset.Y == 0)
            {
                newHeight = maxHeaderHeight;
            }
            else if (isScrollingDown)
            {
                newHeight = (float)Math.Max(minHeaderHeight, headerHeight - Math.Abs(scrollDiff));
            }
            else if (isScrollingUp)
            {
                newHeight = (float)Math.Min(maxHeaderHeight, headerHeight + Math.Abs(scrollDiff));
            }

            if (newHeight != headerHeight)
            {
                headerHeight = newHeight;
                ViewHelper.AdjustFrameSetHeight(_viewHeader, headerHeight);
                tableViewAccounts.TableHeaderView = _viewHeader;
            }

            previousScrollOffset = tableViewAccounts.ContentOffset.Y;
        }

        /// <summary>
        /// Handles the load more.
        /// </summary>
        private void OnLoadMore()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        await UpdateDues();
                        await LoadDues();
                        ActivityIndicator.Hide();
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        /// <summary>
        /// On refresh.
        /// </summary>
        private void OnRefresh()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        await GetAccountsSummary(accountsToRefresh, false);
                        ActivityIndicator.Hide();
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        /// <summary>
        /// Updates the load more display.
        /// </summary>
        private void UpdateLoadMoreDisplay()
        {
            if (loadedAccountsCount >= DataManager.DataManager.SharedInstance.GetAccountsCount()
                || DataManager.DataManager.SharedInstance.GetAccountsCount() < MaxAccountsPerCall)
            {
                // hide load more
                _viewLoadMore.Hidden = true;
                ViewHelper.AdjustFrameSetY(btnAdd, verticalMargin);
                ViewHelper.AdjustFrameSetHeight(_viewFooter, addAccountHeight);
            }
            else
            {
                _viewLoadMore.Hidden = false;
                ViewHelper.AdjustFrameSetY(btnAdd, _viewLoadMore.Frame.GetMaxY() + verticalMargin);
                ViewHelper.AdjustFrameSetHeight(_viewFooter, _viewLoadMore.Frame.GetMaxY() + addAccountHeight);
            }
            tableViewAccounts.TableFooterView = _viewFooter;
        }

        /// <summary>
        /// Loads the dues.
        /// </summary>
        /// <returns>The dues.</returns>
        private async Task LoadDues(int accountsToAdd = 0, bool pullDown = false)
        {
            var accounts = GetAccountsToLoad(accountsToAdd);
            await GetAccountsSummary(accounts, true, pullDown);
        }

        /// <summary>
        /// Gets the accounts summary.
        /// </summary>
        /// <returns>The accounts summary.</returns>
        /// <param name="accounts">Accounts.</param>
        private async Task<bool> GetAccountsSummary(List<string> accounts, bool willGetNew = false, bool pullDown = false)
        {
            bool res = false;
            if (accounts?.Count > 0)
            {
                if (!pullDown)
                {
                    ActivityIndicator.Show();
                }
                var response = await ServiceCall.GetLinkedAccountsSummaryInfo(accounts);
                res = response.didSucceed;

                if (willGetNew)
                {
                    loadedAccountsCount += accounts.Count;
                }

                if (response.didSucceed && response.AccountDues?.Count > 0)
                {
                    UpdateDisplayedAccounts(response.AccountDues);
                    isTimeOut = false;
                    if (accountsToRefresh != null)
                    {
                        if (accountsToRefresh.Count > 0)
                        {
                            accountsToRefresh.Clear();
                        }
                    }
                    UpdateHeader();
                    UpdateLoadMoreDisplay();
                }
                else
                {
                    if (accountsToRefresh != null)
                    {
                        List<string> combinedList = accountsToRefresh.Union(accounts).ToList();
                        accountsToRefresh = combinedList;
                    }
                    else
                    {
                        accountsToRefresh = accounts;
                    }
                    isTimeOut = true;
                    UpdateHeader(response);
                    _viewLoadMore.Hidden = false;
                    ViewHelper.AdjustFrameSetY(btnAdd, _viewLoadMore.Frame.GetMaxY() + verticalMargin);
                    ViewHelper.AdjustFrameSetHeight(_viewFooter, _viewLoadMore.Frame.GetMaxY() + addAccountHeight);
                }

                tableViewAccounts.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
                tableViewAccounts.SeparatorColor = UIColor.FromWhiteAlpha(1, 0.4f);
                InitializeAccountsTable(isTimeOut);
                ActivityIndicator.Hide();
                isRefreshing = false;
            }
            return res;
        }

        /// <summary>
        /// Initializes the accounts table.
        /// </summary>
        private void InitializeAccountsTable(bool timeOut = false)
        {
            tableViewAccounts.Source = new DashboardAccountsDataSource(displayedAccounts, OnAccountRowSelected, OnTableViewAccountsScrolled, timeOut);
            tableViewAccounts.ReloadData();
        }

        /// <summary>
        /// Handles the account row selected.
        /// </summary>
        /// <param name="account">Account.</param>
        private void OnAccountRowSelected(DueAmountDataModel account)
        {
            var index = DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.FindIndex(x => x.accNum == account.accNum) ?? -1;

            if (index >= 0)
            {
                var selected = DataManager.DataManager.SharedInstance.AccountRecordsList.d[index];
                DataManager.DataManager.SharedInstance.SelectAccount(selected.accNum);
                DataManager.DataManager.SharedInstance.IsSameAccount = false;
                UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                var vc = storyBoard.InstantiateViewController("DashboardViewController") as DashboardViewController;
                if (vc != null)
                {
                    vc.ShouldShowBackButton = true;
                    ShowViewController(vc, null);
                }
            }
        }

        /// <summary>
        /// Gets the accounts to load.
        /// </summary>
        /// <returns>The accounts to load.</returns>
        private List<string> GetAccountsToLoad(int addAccountsCount = 0)
        {
            var accountsList = DataManager.DataManager.SharedInstance.AccountRecordsList.d;

            //var results = from a in accountsList
            //group a.accNum by a.IsREAccount into g
            //select new { IsREAccount = g.Key, accNums = g.ToList() };
            var results = accountsList.GroupBy(x => x.IsREAccount);
            var accounts = new List<string>();

            int inputAdd = (addAccountsCount > 0 && addAccountsCount < MaxAccountsPerCall)
                            ? addAccountsCount : MaxAccountsPerCall;

            int accountsToGet = Math.Min(inputAdd, accountsList.Count - loadedAccountsCount);


            if (results != null && results?.Count() > 0)
            {
                var reAccts = results.Where(x => x.Key == true).SelectMany(y => y).OrderBy(o => o.accountNickName).ToList();
                var normalAccts = results.Where(x => x.Key == false).SelectMany(y => y).OrderBy(o => o.accountNickName).ToList();

                var accountsToAdd = new List<DueAmountDataModel>();
                var accountsToAddRe = new List<DueAmountDataModel>();
                int added = 0;

                // check if new RE accounts added since last load
                var reKey = "Dashboard_RESectionHeader".Translate();
                GetRequestedAccountsByType(reKey, reAccts, accountsToGet, ref added, accounts, accountsToAdd, accountsToAddRe);

                // add normal
                var normalKey = "Dashboard_SectionHeader".Translate();
                GetRequestedAccountsByType(normalKey, normalAccts, accountsToGet, ref added, accounts, accountsToAdd, accountsToAddRe);

                AddToDisplayedAccounts(reKey, accountsToAddRe);
                AddToDisplayedAccounts(normalKey, accountsToAdd);
            }

            return accounts;
        }

        /// <summary>
        /// Gets the requested accounts by its type.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="allAcctsByType">All accts by type.</param>
        /// <param name="accountsToGet">Accounts to get.</param>
        /// <param name="added">Added.</param>
        /// <param name="accounts">Accounts.</param>
        /// <param name="accountsToAdd">Accounts to add.</param>
        /// <param name="accountsToAddRe">Accounts to add re.</param>
        private void GetRequestedAccountsByType(string key, List<CustomerAccountRecordModel> allAcctsByType, int accountsToGet
            , ref int added, List<string> accounts, List<DueAmountDataModel> accountsToAdd, List<DueAmountDataModel> accountsToAddRe)
        {
            List<DueAmountDataModel> currAccts = new List<DueAmountDataModel>();
            if (displayedAccounts.ContainsKey(key))
            {
                currAccts = displayedAccounts[key];
            }

            for (int i = 0; i < allAcctsByType.Count() && added < accountsToGet; i++)
            {
                var acct = allAcctsByType[i];
                var index = currAccts.FindIndex(x => x.accNum == acct.accNum);
                if (index < 0)
                {
                    accounts.Add(acct.accNum);
                    AddToRequestedAccounts(acct, accountsToAdd, accountsToAddRe);
                    added++;
                }
            }
        }

        /// <summary>
        /// Adds to requested accounts.
        /// </summary>
        /// <param name="acct">Acct.</param>
        /// <param name="accountsToAdd">Accounts to add.</param>
        /// <param name="accountsToAddRe">Accounts to add re.</param>
        private void AddToRequestedAccounts(CustomerAccountRecordModel acct
            , List<DueAmountDataModel> accountsToAdd, List<DueAmountDataModel> accountsToAddRe)
        {
            var item = new DueAmountDataModel
            {
                accNum = acct.accNum,
                accNickName = acct.accountNickName,
                IsReAccount = acct.IsREAccount
            };
            if (acct.IsREAccount)
            {
                accountsToAddRe.Add(item);
            }
            else
            {
                accountsToAdd.Add(item);
            }
        }

        /// <summary>
        /// Adds to displayed accounts.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="accountsToAdd">Accounts to add.</param>
        private void AddToDisplayedAccounts(string key, List<DueAmountDataModel> accountsToAdd)
        {

            if (accountsToAdd?.Count > 0)
            {
                if (displayedAccounts.ContainsKey(key))
                {
                    var section = displayedAccounts[key];
                    section.AddRange(accountsToAdd);
                    displayedAccounts[key] = section.OrderBy(x => x.accNickName).ToList();
                }
                else
                {
                    var reKey = "Dashboard_RESectionHeader".Translate();
                    if (string.Compare(key, reKey) == 0 && displayedAccounts.Keys.Count > 0)
                    {
                        var firstKey = displayedAccounts.Keys.ElementAt(0);
                        var section = displayedAccounts[firstKey];
                        displayedAccounts.Remove(firstKey);

                        displayedAccounts = new Dictionary<string, List<DueAmountDataModel>>
                        {
                            { key.Translate(), accountsToAdd },
                            { firstKey, section }
                        };
                    }
                    else
                    {
                        displayedAccounts.Add(key.Translate(), accountsToAdd);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the displayed accounts.
        /// </summary>
        /// <param name="accountDetails">Account details.</param>
        private void UpdateDisplayedAccounts(List<DueAmountDataModel> accountDetails)
        {
            foreach (var acct in accountDetails)
            {
                foreach (var key in displayedAccounts.Keys)
                {
                    var accts = displayedAccounts[key];
                    var index = accts.FindIndex(x => x.accNum == acct.accNum);
                    if (index >= 0)
                    {
                        var item = accts[index];
                        item.UpdateValues(acct);
                        DataManager.DataManager.SharedInstance.SaveDue(item);
                        break;
                    }
                } // key
            } // account
        }

        /// <summary>
        /// Updates the dues.
        /// </summary>
        private async Task<int> UpdateDues()
        {
            bool shouldReload = false;
            int removedCount = 0;
            var accounts = GetAccountsToUpdate(ref shouldReload, ref removedCount);

            if (accounts?.Count > 0)
            {
                await GetAccountsSummary(accounts);
            }
            else if (shouldReload)
            {
                if (!isTimeOut)
                {
                    UpdateLoadMoreDisplay();
                }
                InitializeAccountsTable();
            }
            return removedCount;
        }

        /// <summary>
        /// Gets the accounts to update.
        /// </summary>
        /// <returns>The accounts to update.</returns>
        private List<string> GetAccountsToUpdate(ref bool shouldReload, ref int removedCount)
        {
            var acctsToGetLatestDues = new List<string>();
            removedCount = RemoveDeletedAccounts();
            shouldReload = removedCount > 0;

            foreach (var key in displayedAccounts.Keys)
            {
                var accts = displayedAccounts[key];

                // cache updates
                for (int i = 0; i < accts.Count; i++)
                {
                    var acct = accts[i];

                    var acctCached = DataManager.DataManager.SharedInstance.GetDue(acct.accNum);
                    if (acctCached == null || DataManager.DataManager.SharedInstance.IsPaidAccountNumber(acct.accNum))
                    {
                        // get latest if not in cache
                        acctsToGetLatestDues.Add(acct.accNum);
                    }
                    else if (acct.amountDue != acctCached.amountDue
                        || string.Compare(acct.accNickName, acctCached.accNickName) != 0)
                    {
                        // update nickname
                        acct.amountDue = acctCached.amountDue;
                        acct.accNickName = acctCached.accNickName;
                        accts[i] = acct;
                        shouldReload = true;
                    }
                }

            } // key
            DataManager.DataManager.SharedInstance.ClearPaidList();
            return acctsToGetLatestDues;
        }

        /// <summary>
        /// Removes the deleted accounts.
        /// </summary>
        /// <returns>The deleted accounts.</returns>
        private int RemoveDeletedAccounts()
        {
            int removedAccounts = 0;
            List<string> keysToDelete = new List<string>();
            var accountsList = DataManager.DataManager.SharedInstance.AccountRecordsList.d;

            foreach (var key in displayedAccounts.Keys)
            {
                var accts = displayedAccounts[key];

                // remove deleted accounts
                foreach (var delAccNum in DataManager.DataManager.SharedInstance.AccountsDeleted)
                {
                    var deleteIndex = accts.FindIndex(x => x.accNum == delAccNum);
                    if (deleteIndex > -1)
                    {
                        accts.RemoveAt(deleteIndex);
                        removedAccounts++;
                    }
                }

                // for accounts deleted in backend or encountered remove error
                var acctsToDelete = new List<string>();
                foreach (var item in accts)
                {
                    // delete later if cannot find in main list
                    var index = accountsList?.FindIndex(x => x.accNum == item.accNum);
                    if (index < 0)
                    {
                        acctsToDelete.Add(item.accNum);
                    }
                }

                foreach (var delAccNum in acctsToDelete)
                {
                    var deleteIndex = accts.FindIndex(x => x.accNum == delAccNum);
                    if (deleteIndex > -1)
                    {
                        accts.RemoveAt(deleteIndex);
                        removedAccounts++;
                    }
                }

                if (accts.Count == 0)
                {
                    keysToDelete.Add(key);
                }
            }

            if (removedAccounts > 0)
            {
                foreach (var item in keysToDelete)
                {
                    displayedAccounts.Remove(item);
                }

                DataManager.DataManager.SharedInstance.AccountsDeleted.Clear();
                loadedAccountsCount -= removedAccounts;
                if (loadedAccountsCount < 0)
                {
                    loadedAccountsCount = 0;
                }
            }
            return removedAccounts;
        }

        /// <summary>
        /// Shows the toast.
        /// </summary>
        private void ShowToast()
        {
            if (!isBcrmAvailable)
            {
                var status = DataManager.DataManager.SharedInstance.SystemStatus?.Find(x => x.SystemType == SystemEnum.BCRM);
                if (status != null && !string.IsNullOrEmpty(status?.DowntimeTextMessage))
                {
                    View.BringSubviewToFront(toastView);
                    ToastHelper.ShowToast(toastView, ref isAnimating, status?.DowntimeTextMessage);
                }
            }
        }

        /// <summary>
        /// Pulls down to refresh.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void PullDownTorefresh(object sender, EventArgs e)
        {
            if (!isRefreshing)
            {
                RefreshScreen();
            }
        }

        /// <summary>
        /// Refreshes the screen.
        /// </summary>
        /// <returns>The screen.</returns>
        private void RefreshScreen()
        {
            isRefreshing = true;
            DataManager.DataManager.SharedInstance.SummaryNeedsRefresh = true;

            var baseVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            var topVc = AppDelegate.GetTopViewController(baseVc);
            bool hideLoadingIndicator = false;
            if (!(topVc is DashboardHomeViewController))
            {
                hideLoadingIndicator = true;
            }
            LoadContents(false, hideLoadingIndicator);
        }
    }
}