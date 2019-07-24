using CoreGraphics;
using Foundation;
using myTNB.DataManager;
using myTNB.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UIKit;

namespace myTNB
{
    public partial class AccountsCardContentViewController : CustomUIViewController
    {
        public AccountsCardContentViewController(IntPtr handle) : base(handle) { }

        public DashboardHomeViewController _homeViewController;
        DashboardHomeHeader _dashboardHomeHeader;
        DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();
        public List<List<DueAmountDataModel>> _groupAccountList;
        TextFieldHelper _textFieldHelper = new TextFieldHelper();
        private List<List<DueAmountDataModel>> _unfilteredAccountList = new List<List<DueAmountDataModel>>();

        nfloat padding = 8f;
        nfloat searchPadding = 16f;
        nfloat labelHeight = 24f;
        nfloat imageWidth = 24f;
        nfloat imageHeight = 24f;

        UIPageControl _pageControl;
        UIScrollView _accountsCardScrollView;
        int _currentPageIndex;
        UIView _parentView, _searchView, _textFieldView;
        UILabel _headerTitle;
        UIImageView _searchIcon, _addAccountIcon;
        UITextField _textFieldSearch;
        bool _isSearchMode = false;
        bool _isUpdating = true;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NSNotificationCenter.DefaultCenter.AddObserver((NSString)"NotificationDidChange", NotificationDidChange);
            NSNotificationCenter.DefaultCenter.AddObserver((NSString)"OnReceiveNotificationFromDashboard", NotificationDidChange);
            _unfilteredAccountList = _dashboardHomeHelper.GetGroupAccountsList(DataManager.DataManager.SharedInstance.AccountRecordsList.d);
            SetParentView();
            SetGreetingView();
            SetSearchView();
            SetCardScrollView();
            SetScrollViewSubViews();
            LoadAccountsWithDues();
        }

        public override void ViewWillAppear(bool animated)
        {
            if (DataManager.DataManager.SharedInstance.SummaryNeedsRefresh)
            {
                ResetAccountCardsView(DataManager.DataManager.SharedInstance.AccountRecordsList.d);
                DataManager.DataManager.SharedInstance.SummaryNeedsRefresh = false;
            }
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
        #endregion

        #region View Initialization Methods
        private void SetParentView()
        {
            _parentView = new UIView(new CGRect(0,
                0, View.Frame.Width,
                _dashboardHomeHelper.GetHeightForAccountCards()))
            {
                BackgroundColor = UIColor.Clear,
                UserInteractionEnabled = true
            };
            View.AddSubview(_parentView);
        }

        private void SetGreetingView()
        {
            _dashboardHomeHeader = new DashboardHomeHeader(_parentView);
            _dashboardHomeHeader.SetGreetingText(_homeViewController.GetGreeting());
            _dashboardHomeHeader.SetNameText(_dashboardHomeHelper.GetDisplayName());
            _parentView.AddSubview(_dashboardHomeHeader.GetUI());
        }

        private void SetSearchView()
        {
            _searchView = new UIView(new CGRect(0, _dashboardHomeHeader.GetView().Frame.GetMaxY(), _parentView.Frame.Width, DashboardHomeConstants.SearchViewHeight))
            {
                BackgroundColor = UIColor.Clear
            };

            _headerTitle = new UILabel(new CGRect(searchPadding, 0, 186f, labelHeight))
            {
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = UIColor.White,
                Text = "Dashboard_MyAccounts".Translate()
            };

            var sideIconXValue = _searchView.Frame.Width - imageWidth - searchPadding;
            _searchIcon = new UIImageView(new CGRect(sideIconXValue, 0, imageWidth, imageHeight))
            {
                Image = UIImage.FromBundle("Search-Icon"),
                Hidden = NoPaginationNeeded()
            };

            _addAccountIcon = new UIImageView(new CGRect(NoPaginationNeeded() ? sideIconXValue : sideIconXValue - imageWidth - 8f, 0, imageWidth, imageHeight))
            {
                Image = UIImage.FromBundle("Add-Account-Icon")
            };

            var spacing = searchPadding + imageWidth + 8f;
            _textFieldView = new UIView(new CGRect(spacing, 0, _searchView.Frame.Width - spacing - searchPadding, 24f))
            {
                BackgroundColor = UIColor.White
            };
            _textFieldView.Layer.CornerRadius = 12f;
            _textFieldSearch = new UITextField(new CGRect(12f, 0, _textFieldView.Frame.Width - 24f - imageWidth / 2, 24f))
            {
                AttributedPlaceholder = new NSAttributedString(
                    "Dashboard_SearchPlacehoder".Translate()
                    , font: MyTNBFont.MuseoSans12_500
                    , foregroundColor: MyTNBColor.WhiteTwo
                    , strokeWidth: 0
                ),
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans14_500
            };
            _textFieldHelper.SetKeyboard(_textFieldSearch);
            _textFieldSearch.ReturnKeyType = UIReturnKeyType.Search;
            _textFieldView.Hidden = true;

            _textFieldView.AddSubview(_textFieldSearch);
            if (NoPaginationNeeded())
            {
                _searchView.AddSubviews(new UIView { _headerTitle, _addAccountIcon });
            }
            else
            {
                _searchView.AddSubviews(new UIView { _headerTitle, _textFieldView, _addAccountIcon, _searchIcon });
                SetTextFieldEvents(_textFieldSearch);
            }
            _parentView.AddSubview(_searchView);
        }

        private void AdjustParentFrame()
        {
            CGRect frame = _parentView.Frame;
            frame.Height = _dashboardHomeHelper.GetHeightForAccountCards();
            _parentView.Frame = frame;
        }

        private void SetCardScrollView()
        {
            if (_accountsCardScrollView != null)
            {
                _accountsCardScrollView.RemoveFromSuperview();
            }
            _accountsCardScrollView = new UIScrollView(new CGRect(0, _searchView.Frame.GetMaxY(), _parentView.Frame.Width, _dashboardHomeHelper.GetHeightForAccountCardsOnly()))
            {
                Delegate = new AccountsScrollViewDelegate(this),
                PagingEnabled = true,
                ShowsHorizontalScrollIndicator = false,
                ShowsVerticalScrollIndicator = false,
                ClipsToBounds = false,
                BackgroundColor = UIColor.Clear,
                Hidden = false
            };

            AdjustFrame(_accountsCardScrollView, padding, 0, -padding * 3, 0);
            _parentView.AddSubview(_accountsCardScrollView);
        }

        private void AddPageControl()
        {
            if (_pageControl != null)
            {
                _pageControl.RemoveFromSuperview();
            }
            _pageControl = new UIPageControl(new CGRect(8, _accountsCardScrollView.Frame.GetMaxY(), _parentView.Frame.Width - 16f, DashboardHomeConstants.PageControlHeight))
            {
                BackgroundColor = UIColor.Clear,
                TintColor = MyTNBColor.WaterBlue,
                PageIndicatorTintColor = MyTNBColor.VeryLightPinkTwo,
                CurrentPageIndicatorTintColor = MyTNBColor.WaterBlue,
                UserInteractionEnabled = false
            };
            _parentView.AddSubview(_pageControl);
        }
        #endregion

        #region Search Methods
        private void SetViewForActiveSearch(bool isSearchMode)
        {
            if (isSearchMode)
            {
                _textFieldSearch.BecomeFirstResponder();
            }
            else
            {
                _textFieldSearch.ResignFirstResponder();
            }
            _headerTitle.Hidden = isSearchMode;
            _textFieldView.Hidden = !isSearchMode;
            CGRect frame = _addAccountIcon.Frame;
            var sideIconXValue = NoPaginationNeeded() ? _searchView.Frame.Width - imageWidth - searchPadding : _searchIcon.Frame.GetMinX() - imageWidth - 8f;
            frame.X = isSearchMode ? searchPadding : sideIconXValue;
            _addAccountIcon.Frame = frame;
        }

        private void SetTextFieldEvents(UITextField textField)
        {
            textField.EditingChanged += (sender, e) =>
            {
                SearchFromAccountList(textField.Text);
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
        }

        private void SearchFromAccountList(string searchString)
        {
            var accountsList = DataManager.DataManager.SharedInstance.AccountRecordsList.d;
            var searchResults = accountsList.FindAll(x => x.accountNickName.ToLower().Contains(searchString.ToLower()) || x.accNum.Contains(searchString));
            ResetAccountCardsView(searchResults);
        }

        private void ResetAccountCardsView(List<CustomerAccountRecordModel> accountsList)
        {
            DataManager.DataManager.SharedInstance.AccountsGroupList.Clear();
            _dashboardHomeHelper.GroupAccountsList(accountsList);
            _groupAccountList.Clear();
            _groupAccountList = DataManager.DataManager.SharedInstance.AccountsGroupList;
            _homeViewController.OnReloadTableForSearch();
            ClearScrollViewSubViews();
            AdjustParentFrame();
            SetCardScrollView();
            SetScrollViewSubViews();
            LoadAccountsWithDues();
        }

        public void ResetAccountCardsView()
        {
            _groupAccountList.Clear();
            _groupAccountList = DataManager.DataManager.SharedInstance.AccountsGroupList;
            ClearScrollViewSubViews();
            AdjustParentFrame();
            SetCardScrollView();
            SetScrollViewSubViews();
            LoadAccountsWithDues();
        }
        #endregion

        #region Touch Methods
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            var touch = touches.AnyObject as UITouch;

            if (_searchIcon.Frame.Contains(touch.LocationInView(_searchView)) && !_searchIcon.Hidden && _searchIcon.Superview != null)
            {
                if (!NoPaginationNeeded())
                {
                    OnSearchAction();
                }
            }
            else if (_addAccountIcon.Frame.Contains(touch.LocationInView(_searchView)))
            {
                OnAddAccountAction();
            }
            else if (_textFieldView.Frame.Contains(touch.LocationInView(_searchView)) && !_textFieldView.Hidden && _textFieldView.Superview != null)
            {
                if (!NoPaginationNeeded())
                {
                    OnTypeSearchAction();
                }
            }
            else if (_dashboardHomeHeader._notificationView.Frame.Contains(touch.LocationInView(_dashboardHomeHeader.GetView())))
            {
                OnNotificationAction();
            }
            else
            {
                if (_isSearchMode)
                {
                    _textFieldSearch.ResignFirstResponder();
                    _isSearchMode = false;
                    SetViewForActiveSearch(_isSearchMode);
                }
            }
        }
        #endregion

        #region Action Methods
        private void OnAddAccountAction()
        {
            _homeViewController.OnAddAccountAction();
        }

        private void OnSearchAction()
        {
            _isSearchMode = !_isSearchMode;
            SetViewForActiveSearch(_isSearchMode);
        }

        private void OnTypeSearchAction()
        {
            _isSearchMode = true;
            _textFieldSearch.BecomeFirstResponder();
        }

        private void OnNotificationAction()
        {
            _homeViewController.OnNotificationAction();
        }
        #endregion

        /// <summary>
        /// Loads the Accounts with Dues
        /// </summary>
        private void LoadAccountsWithDues()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        bool shouldReload = false;
                        var accounts = GetAccountsToUpdate(ref shouldReload);

                        if (accounts?.Count > 0)
                        {
                            await GetAccountsSummary(accounts);
                            _isUpdating = false;
                            UpdateCardsWithTag(_currentPageIndex);
                        }
                        else if (shouldReload)
                        {
                            _isUpdating = false;
                            UpdateCardsWithTag(_currentPageIndex);
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private void UpdateDueForDisplayedAccounts(List<DueAmountDataModel> dueDetails)
        {
            if (_groupAccountList.Count <= 0)
                return;

            if (_currentPageIndex > -1 && _currentPageIndex < _groupAccountList.Count)
            {
                var groupAccountList = _groupAccountList[_currentPageIndex];

                foreach (var due in dueDetails)
                {
                    foreach (var account in groupAccountList)
                    {
                        if (account.accNum == due.accNum)
                        {
                            var item = account;
                            item.UpdateValues(due);
                            DataManager.DataManager.SharedInstance.SaveDue(item);
                            break;
                        }
                    }
                }
            }
        }

        private async Task<bool> GetAccountsSummary(List<string> accounts)
        {
            bool res = false;

            var response = await ServiceCall.GetLinkedAccountsSummaryInfo(accounts);
            res = response.didSucceed;

            if (response.didSucceed && response.AccountDues?.Count > 0)
            {
                UpdateDueForDisplayedAccounts(response.AccountDues);
            }
            else
            {
                //FAIL scenarios here...
            }
            return res;
        }

        /// <summary>
        /// Gets the accounts to update.
        /// </summary>
        /// <returns>The accounts to update.</returns>
        private List<string> GetAccountsToUpdate(int index)
        {
            var acctsToGetLatestDues = new List<string>();

            if (_groupAccountList.Count <= 0)
                return acctsToGetLatestDues;

            if (index > -1 && index < _groupAccountList.Count)
            {
                var groupAccountList = _groupAccountList[index];

                // cache updates
                for (int i = 0; i < groupAccountList.Count; i++)
                {
                    if (i > -1 && i < groupAccountList.Count)
                    {
                        var account = groupAccountList[i];
                        var acctCached = DataManager.DataManager.SharedInstance.GetDue(account.accNum);
                        if (acctCached == null)
                        {
                            // get latest if not in cache
                            acctsToGetLatestDues.Add(account.accNum);
                        }
                        else if (account.amountDue != acctCached.amountDue
                               || string.Compare(account.accNickName, acctCached.accNickName) != 0)
                        {
                            // update nickname
                            account.amountDue = acctCached.amountDue;
                            account.accNickName = acctCached.accNickName;
                            groupAccountList[i] = account;
                        }
                    }
                }
            }

            return acctsToGetLatestDues;
        }

        /// <summary>
        /// Gets the accounts to update.
        /// </summary>
        /// <returns>The accounts to update.</returns>
        private List<string> GetAccountsToUpdate(ref bool shouldReload)
        {
            var acctsToGetLatestDues = new List<string>();

            if (_groupAccountList.Count <= 0)
                return acctsToGetLatestDues;

            shouldReload = RemoveDeletedAccounts() > 0;

            if (_currentPageIndex > -1 && _currentPageIndex < _groupAccountList.Count)
            {
                var groupAccountList = _groupAccountList[_currentPageIndex];

                // cache updates
                for (int i = 0; i < groupAccountList.Count; i++)
                {
                    if (i > -1 && i < groupAccountList.Count)
                    {
                        var account = groupAccountList[i];
                        var acctCached = DataManager.DataManager.SharedInstance.GetDue(account.accNum);
                        if (acctCached == null)
                        {
                            // get latest if not in cache
                            acctsToGetLatestDues.Add(account.accNum);
                        }
                        else if (account.amountDue != acctCached.amountDue
                               || string.Compare(account.accNickName, acctCached.accNickName) != 0)
                        {
                            // update nickname
                            account.amountDue = acctCached.amountDue;
                            account.accNickName = acctCached.accNickName;
                            groupAccountList[i] = account;
                            shouldReload = true;
                        }
                    }
                }
            }

            return acctsToGetLatestDues;
        }

        /// <summary>
        /// Removes the deleted accounts.
        /// </summary>
        /// <returns>The deleted accounts.</returns>
        private int RemoveDeletedAccounts()
        {
            int removedAccounts = 0;

            if (_groupAccountList.Count <= 0)
                return removedAccounts;

            if (_currentPageIndex > -1 && _currentPageIndex < _groupAccountList.Count)
            {
                List<string> keysToDelete = new List<string>();
                var accountsList = DataManager.DataManager.SharedInstance.AccountRecordsList.d;
                var groupAccountList = _groupAccountList[_currentPageIndex];

                // remove deleted accounts
                foreach (var delAccNum in DataManager.DataManager.SharedInstance.AccountsDeleted)
                {
                    var deleteIndex = groupAccountList.FindIndex(x => x.accNum == delAccNum);
                    if (deleteIndex > -1)
                    {
                        groupAccountList.RemoveAt(deleteIndex);
                        removedAccounts++;
                    }
                }

                // for accounts deleted in backend or encountered remove error
                var acctsToDelete = new List<string>();
                foreach (var item in groupAccountList)
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
                    var deleteIndex = groupAccountList.FindIndex(x => x.accNum == delAccNum);
                    if (deleteIndex > -1)
                    {
                        groupAccountList.RemoveAt(deleteIndex);
                        removedAccounts++;
                    }
                }

                if (removedAccounts > 0)
                {
                    DataManager.DataManager.SharedInstance.AccountsDeleted.Clear();
                }
            }

            return removedAccounts;
        }

        private CGRect AdjustFrame(CGRect f, nfloat x, nfloat y, nfloat w, nfloat h)
        {
            f.X = f.X + x;
            f.Y = f.Y + y;
            f.Width = f.Width + w;
            f.Height = f.Height + h;

            return f;
        }

        private void AdjustFrame(UIView view, nfloat x, nfloat y, nfloat w, nfloat h)
        {
            view.Frame = AdjustFrame(view.Frame, x, y, w, h);
        }

        private void ClearScrollViewSubViews()
        {
            var subviews = _accountsCardScrollView.Subviews;
            foreach (var view in subviews)
            {
                if (view != null)
                {
                    view.RemoveFromSuperview();
                }
            }
        }

        private void SetScrollViewSubViews()
        {
            nfloat width = _accountsCardScrollView.Frame.Width;
            for (int i = 0; i < _groupAccountList.Count; i++)
            {
                UIView _viewContainer = new UIView(_accountsCardScrollView.Bounds);
                _viewContainer.BackgroundColor = UIColor.Clear;
                _viewContainer.Tag = i;

                CGRect frame = _viewContainer.Frame;
                frame.X = (i * width) + padding;
                frame.Y = (_accountsCardScrollView.Frame.Height - frame.Height) / 2;
                frame.Width = width - (padding * 1);
                _viewContainer.Frame = frame;
                _accountsCardScrollView.AddSubview(_viewContainer);
                var accounts = GetAccountsToUpdate(i);
                AddAccountsCardInContainerView(_viewContainer, i, accounts?.Count > 0);
            }
            _accountsCardScrollView.ContentSize = new CGSize(_accountsCardScrollView.Frame.Width * _groupAccountList.Count, _accountsCardScrollView.Frame.Height);
            if (_groupAccountList.Count > 1)
            {
                AddPageControl();
                UpdatePageControl(_pageControl, _currentPageIndex, _groupAccountList.Count, GetContainerViewWithTag(_currentPageIndex));
            }
            else
            {
                _pageControl.Hidden = true;
            }
        }

        private UIView GetContainerViewWithTag(int tag)
        {
            var subviews = _accountsCardScrollView.Subviews;
            UIView containerView = null;
            foreach (var views in subviews)
            {
                var view = views as UIView;
                if (view != null)
                {
                    if (view.Tag == tag)
                    {
                        containerView = view;
                        break;
                    }
                }
            }
            return containerView;
        }

        private void UpdateCardsWithTag(int tag)
        {
            var subviews = _accountsCardScrollView.Subviews;
            foreach (var views in subviews)
            {
                var view = views as UIView;
                if (view != null)
                {
                    if (view.Tag == tag)
                    {
                        RemoveAccountCardsFromView(view);
                        AddAccountsCardInContainerView(view, tag, _isUpdating);
                        break;
                    }
                }
            }
        }

        private void RemoveAccountCardsFromView(UIView containerView)
        {
            var subviews = containerView.Subviews;
            foreach (var view in subviews)
            {
                view.RemoveFromSuperview();
            }
        }

        private void AddAccountsCardInContainerView(UIView containerView, int pageIndex, bool isUpdating)
        {
            var groupAccountList = _groupAccountList[pageIndex];
            for (int i = 0; i < groupAccountList.Count; i++)
            {
                DashboardHomeAccountCard _homeAccountCard = new DashboardHomeAccountCard(this, containerView, 68f * i);
                string iconName = "Accounts-Smart-Meter-Icon";
                if (groupAccountList[i].IsReAccount)
                {
                    iconName = "Accounts-RE-Icon";
                }
                else if (groupAccountList[i].IsNormalAccount && !groupAccountList[i].IsSSMR)
                {
                    iconName = "Accounts-Normal-Icon";
                }
                else if (groupAccountList[i].IsNormalAccount && groupAccountList[i].IsSSMR)
                {
                    iconName = "SMR-Icon";
                }
                _homeAccountCard.SetAccountIcon(iconName);
                _homeAccountCard.SetNickname(groupAccountList[i].accNickName);
                _homeAccountCard.SetAccountNo(groupAccountList[i].accNum);
                _homeAccountCard.IsUpdating = isUpdating;
                _homeAccountCard.SetModel(groupAccountList[i]);
                containerView.AddSubview(_homeAccountCard.GetUI());

            }
        }

        private void UpdatePageControl(UIPageControl pageControl, int current, int pages, UIView currentView)
        {
            pageControl.CurrentPage = current;
            pageControl.Pages = pages;
            pageControl.UpdateCurrentPageDisplay();
        }

        private void ScrollViewHasPaginated()
        {
            UpdatePageControl(_pageControl, _currentPageIndex, _groupAccountList.Count, GetContainerViewWithTag(_currentPageIndex));
            LoadAccountsWithDues();
        }

        public void OnAccountCardSelected(DueAmountDataModel model)
        {
            _homeViewController.OnAccountCardSelected(model);
        }

        private bool NoPaginationNeeded()
        {
            return _unfilteredAccountList?.Count <= 1;
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            View.BackgroundColor = UIColor.Clear;
        }

        private class AccountsScrollViewDelegate : UIScrollViewDelegate
        {
            AccountsCardContentViewController _controller;
            public AccountsScrollViewDelegate(AccountsCardContentViewController controller)
            {
                _controller = controller;
            }
            public override void Scrolled(UIScrollView scrollView)
            {
                int newPageIndex = (int)Math.Round(_controller._accountsCardScrollView.ContentOffset.X / _controller._accountsCardScrollView.Frame.Width);
                if (newPageIndex == _controller._currentPageIndex)
                    return;

                _controller._currentPageIndex = newPageIndex;
                _controller.ScrollViewHasPaginated();
            }
        }
    }
}
